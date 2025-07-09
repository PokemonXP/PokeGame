using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Validators;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Pokemons.Commands;

internal record CreatePokemon(CreatePokemonPayload Payload) : ICommand<PokemonModel>;

/// <exception cref="FormNotFoundException"></exception>
/// <exception cref="IdAlreadyUsedException{T}"></exception>
/// <exception cref="PokemonUniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreatePokemonHandler : ICommandHandler<CreatePokemon, PokemonModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  public CreatePokemonHandler(
    IApplicationContext applicationContext,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel> HandleAsync(CreatePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreatePokemonPayload payload = command.Payload;

    PokemonId pokemonId = PokemonId.NewId();
    Pokemon? pokemon;
    if (payload.Id.HasValue)
    {
      pokemonId = new(payload.Id.Value);
      pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
      if (pokemon is not null)
      {
        throw new IdAlreadyUsedException<Pokemon>(realmId: null, payload.Id.Value, nameof(payload.Id));
      }
    }

    FormModel form = await _pokemonManager.FindFormAsync(payload.Form, nameof(payload.Form), cancellationToken);
    VarietyModel variety = form.Variety;
    SpeciesModel species = variety.Species;
    FormId formId = new(form.Id);

    new CreatePokemonValidator(uniqueNameSettings, form).ValidateAndThrow(payload);

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    PokemonType teraType = payload.TeraType ?? form.Types.Primary;
    PokemonSize size = payload.Size is null ? _randomizer.PokemonSize() : new(payload.Size);
    PokemonGender? gender = payload.Gender ?? (variety.GenderRatio.HasValue ? _randomizer.PokemonGender(variety.GenderRatio.Value) : null);
    AbilitySlot abilitySlot = payload.AbilitySlot ?? _randomizer.AbilitySlot(form.Abilities);
    PokemonNature nature = string.IsNullOrWhiteSpace(payload.Nature) ? _randomizer.PokemonNature() : PokemonNatures.Instance.Find(payload.Nature);
    BaseStatistics baseStatistics = new(form.BaseStatistics);
    IndividualValues individualValues = payload.IndividualValues is null ? _randomizer.IndividualValues() : new(payload.IndividualValues);
    EffortValues effortValues = payload.EffortValues is null ? new() : new(payload.EffortValues);
    byte friendship = payload.Friendship ?? (byte)species.BaseFriendship;

    pokemon = new(
      formId,
      uniqueName,
      teraType,
      size,
      nature,
      baseStatistics,
      gender,
      abilitySlot,
      individualValues,
      effortValues,
      species.GrowthRate,
      payload.Experience,
      payload.Vitality,
      payload.Stamina,
      friendship,
      actorId,
      pokemonId)
    {
      Sprite = Url.TryCreate(payload.Sprite),
      Url = Url.TryCreate(payload.Url),
      Notes = Description.TryCreate(payload.Notes)
    };
    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
