using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
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
/// <exception cref="MovesNotFoundException"></exception>
/// <exception cref="PokemonUniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreatePokemonHandler : ICommandHandler<CreatePokemon, PokemonModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemRepository _itemRepository;
  private readonly IMoveManager _moveManager;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  public CreatePokemonHandler(
    IApplicationContext applicationContext,
    IItemRepository itemRepository,
    IMoveManager moveManager,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _itemRepository = itemRepository;
    _moveManager = moveManager;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
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

    FormModel formModel = await _pokemonManager.FindFormAsync(payload.Form, nameof(payload.Form), cancellationToken);

    PokemonSpecies species = formModel.Variety.Species.ToPokemonSpecies();
    Variety variety = formModel.Variety.ToVariety(species);
    Form form = formModel.ToForm(variety);
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
