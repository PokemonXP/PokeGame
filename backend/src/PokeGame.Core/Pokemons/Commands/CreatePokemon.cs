using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Validators;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemons.Commands;

internal record CreatePokemon(CreatePokemonPayload Payload) : ICommand<PokemonModel>;

/// <exception cref="FormNotFoundException"></exception>
/// <exception cref="IdAlreadyUsedException{T}"></exception>
/// <exception cref="ItemNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreatePokemonHandler : ICommandHandler<CreatePokemon, PokemonModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormRepository _formRepository;
  private readonly IItemRepository _itemRepository;
  private readonly IMoveRepository _moveRepository;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;
  private readonly ISpeciesRepository _speciesRepository;
  private readonly IVarietyRepository _varietyRepository;

  public CreatePokemonHandler(
    IApplicationContext applicationContext,
    IFormRepository formRepository,
    IItemRepository itemRepository,
    IMoveRepository moveRepository,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    ISpeciesRepository speciesRepository,
    IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _formRepository = formRepository;
    _itemRepository = itemRepository;
    _moveRepository = moveRepository;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _speciesRepository = speciesRepository;
    _varietyRepository = varietyRepository;
  }

  public async Task<PokemonModel> HandleAsync(CreatePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    RealmId? realmId = _applicationContext.RealmId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreatePokemonPayload payload = command.Payload;
    new CreatePokemonValidator(uniqueNameSettings).ValidateAndThrow(payload);

    PokemonId pokemonId = PokemonId.NewId();
    Pokemon2? pokemon;
    if (payload.Id.HasValue)
    {
      pokemonId = new(payload.Id.Value);
      pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
      if (pokemon is not null)
      {
        throw new IdAlreadyUsedException<Pokemon2>(realmId, payload.Id.Value, nameof(payload.Id));
      }
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

    IReadOnlyCollection<Move> moves = await _moveRepository.LoadAsync(variety.Moves.Keys, cancellationToken);
    List<LearnedMove> learnedMoves = new(capacity: variety.Moves.Count);
    foreach (Move move in moves)
    {
      Level? level = variety.Moves[move.Id];
      if (level is null || level.Value <= pokemon.Level)
      {
        string order = string.Join('_', (level?.Value ?? 0).ToString("D3"), move.DisplayName?.Value ?? move.UniqueName.Value);
        learnedMoves.Add(new LearnedMove(move, level, order));
      }
    }
    learnedMoves = learnedMoves.OrderBy(x => x.Order).ToList();
    for (int i = 0; i < learnedMoves.Count; i++)
    {
      LearnedMove learned = learnedMoves[i];
      int position = Math.Max(i + Pokemon2.MoveLimit - learnedMoves.Count, 0);
      pokemon.LearnMove(learned.Move, position, learned.Level, notes: null, actorId);
      i++;
    }

    if (learnedMoves.Count == 5)
    {
      pokemon.RelearnMove(learnedMoves[1].Move, position: 0, actorId);
      pokemon.RelearnMove(learnedMoves[2].Move, position: 1, actorId);
      pokemon.RelearnMove(learnedMoves[3].Move, position: 2, actorId);
    }
    else if (learnedMoves.Count == 6)
    {
      pokemon.RelearnMove(learnedMoves[2].Move, position: 0, actorId);
      pokemon.RelearnMove(learnedMoves[3].Move, position: 1, actorId);
    }
    else if (learnedMoves.Count == 7)
    {
      pokemon.RelearnMove(learnedMoves[3].Move, position: 0, actorId);
    }

    pokemon.Update(actorId);
    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }

  private record LearnedMove(Move Move, Level? Level, string Order);
}
