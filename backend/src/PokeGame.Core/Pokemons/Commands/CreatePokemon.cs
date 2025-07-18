using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Validators;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemons.Commands;

internal record CreatePokemon(CreatePokemonPayload Payload) : ICommand<PokemonModel>;

/// <exception cref="FormNotFoundException"></exception>
/// <exception cref="IdAlreadyUsedException{T}"></exception>
/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="MovesNotFoundException"></exception>
/// <exception cref="PokemonUniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreatePokemonHandler : ICommandHandler<CreatePokemon, PokemonModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormRepository _formRepository;
  private readonly IItemRepository _itemRepository;
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly ISpeciesRepository _speciesRepository;
  private readonly IVarietyRepository _varietyRepository;

  public CreatePokemonHandler(
    IApplicationContext applicationContext,
    IFormRepository formRepository,
    IItemRepository itemRepository,
    ISpeciesRepository speciesRepository,
    IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _formRepository = formRepository;
    _itemRepository = itemRepository;
    _speciesRepository = speciesRepository;
    _varietyRepository = varietyRepository;
  }

  public async Task<PokemonModel> HandleAsync(CreatePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreatePokemonPayload payload = command.Payload;
    new CreatePokemonValidator(uniqueNameSettings).ValidateAndThrow(payload);

    PokemonId pokemonId = PokemonId.NewId();
    Pokemon2? pokemon;
    if (payload.Id.HasValue)
    {
      pokemonId = new(payload.Id.Value);
      //pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
      //if (pokemon is not null)
      //{
      //  throw new IdAlreadyUsedException<Pokemon>(realmId, payload.Id.Value, nameof(payload.Id));
      //} // TODO(fpion): uncomment
    }

    Form form = await _formRepository.LoadAsync(payload.Form, cancellationToken)
      ?? throw new FormNotFoundException(payload.Form, nameof(payload.Form));
    Variety variety = await _varietyRepository.LoadAsync(form.VarietyId, cancellationToken)
      ?? throw new InvalidOperationException($"The variety 'Id={form.VarietyId}' was not loaded.");
    PokemonSpecies species = await _speciesRepository.LoadAsync(variety.SpeciesId, cancellationToken)
      ?? throw new InvalidOperationException($"The species 'Id={variety.SpeciesId}' was not loaded.");
    new CreatePokemonValidator(variety, form).ValidateAndThrow(payload);

    UniqueName uniqueName = string.IsNullOrWhiteSpace(payload.UniqueName) ? species.UniqueName : new(uniqueNameSettings, payload.UniqueName);
    PokemonSize size = payload.Size is null ? _randomizer.PokemonSize() : new(payload.Size);
    PokemonNature nature = string.IsNullOrWhiteSpace(payload.Nature) ? _randomizer.PokemonNature() : PokemonNatures.Instance.Find(payload.Nature);
    IndividualValues individualValues = payload.IndividualValues is null ? _randomizer.IndividualValues() : new(payload.IndividualValues);
    EffortValues? effortValues = payload.EffortValues is null ? null : new(payload.EffortValues);
    PokemonGender? gender = payload.Gender;
    if (!gender.HasValue && variety.GenderRatio is not null)
    {
      gender = _randomizer.PokemonGender(variety.GenderRatio);
    }
    AbilitySlot abilitySlot = payload.AbilitySlot ?? _randomizer.AbilitySlot(form.Abilities);
    Friendship? friendship = payload.Friendship.HasValue ? new(payload.Friendship.Value) : null;

    pokemon = new(species, variety, form, uniqueName, size, nature, individualValues, gender, payload.IsShiny, payload.TeraType,
      abilitySlot, payload.Experience, effortValues, payload.Vitality, payload.Stamina, friendship, actorId, pokemonId)
    {
      Sprite = Url.TryCreate(payload.Sprite),
      Url = Url.TryCreate(payload.Url),
      Notes = Notes.TryCreate(payload.Notes)
    };

    if (!string.IsNullOrWhiteSpace(payload.Nickname))
    {
      Nickname nickname = new(payload.Nickname);
      pokemon.SetNickname(nickname, actorId);
    }

    if (!string.IsNullOrWhiteSpace(payload.HeldItem))
    {
      Item item = await _itemRepository.LoadAsync(payload.HeldItem, cancellationToken) ?? throw new ItemNotFoundException(payload.HeldItem, nameof(payload.HeldItem));
      pokemon.HoldItem(item, actorId);
    }

    //IReadOnlyCollection<MoveModel> moves = await _moveManager.FindAsync(payload.Moves, nameof(payload.Moves), cancellationToken);
    //MoveId moveId;
    //for (int i = 0; i < moves.Count; i++)
    //{
    //  MoveModel move = moves.ElementAt(i);
    //  moveId = move.GetMoveId(realmId);
    //  PowerPoints powerPoints = new(move.PowerPoints);
    //  int position = Math.Max(i + Pokemon.MoveLimit - moves.Count, 0);
    //  pokemon.LearnMove(moveId, powerPoints, position, actorId);
    //}
    //if (moves.Count == 5)
    //{
    //  MoveModel move = moves.ElementAt(4 - 1);
    //  moveId = move.GetMoveId(realmId);
    //  pokemon.RelearnMove(moveId, position: 2, actorId);

    //  move = moves.ElementAt(3 - 1);
    //  moveId = move.GetMoveId(realmId);
    //  pokemon.RelearnMove(moveId, position: 1, actorId);

    //  move = moves.ElementAt(2 - 1);
    //  moveId = move.GetMoveId(realmId);
    //  pokemon.RelearnMove(moveId, position: 0, actorId);
    //}
    //if (moves.Count == 6)
    //{
    //  MoveModel move = moves.ElementAt(4 - 1);
    //  moveId = move.GetMoveId(realmId);
    //  pokemon.RelearnMove(moveId, position: 1, actorId);

    //  move = moves.ElementAt(3 - 1);
    //  moveId = move.GetMoveId(realmId);
    //  pokemon.RelearnMove(moveId, position: 0, actorId);
    //}
    //if (moves.Count == 7)
    //{
    //  MoveModel move = moves.ElementAt(4 - 1);
    //  moveId = move.GetMoveId(realmId);
    //  pokemon.RelearnMove(moveId, position: 0, actorId);
    //}

    pokemon.Update(actorId);
    //await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    //return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
