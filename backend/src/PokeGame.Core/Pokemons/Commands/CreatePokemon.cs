﻿using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Validators;
using PokeGame.Core.Species.Models;
using PokeGame.Core.Varieties.Models;

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
  private readonly IItemManager _itemManager;
  private readonly IMoveManager _moveManager;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  public CreatePokemonHandler(
    IApplicationContext applicationContext,
    IItemManager itemManager,
    IMoveManager moveManager,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _itemManager = itemManager;
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

    DisplayName? nickname = DisplayName.TryCreate(payload.Nickname);
    if (nickname is not null)
    {
      pokemon.SetNickname(nickname, actorId);
    }

    if (!string.IsNullOrWhiteSpace(payload.HeldItem))
    {
      ItemModel item = await _itemManager.FindAsync(payload.HeldItem, nameof(payload.HeldItem), cancellationToken);
      ItemId itemId = new(item.Id);
      pokemon.HoldItem(itemId, actorId);
    }

    IReadOnlyCollection<MoveModel> moves = await _moveManager.FindAsync(payload.Moves, nameof(payload.Moves), cancellationToken);
    for (int i = 0; i < moves.Count; i++)
    {
      MoveModel move = moves.ElementAt(i);
      MoveId moveId = new(move.Id);
      PowerPoints powerPoints = new(move.PowerPoints);
      int position = Math.Max(i + Pokemon.MoveLimit - moves.Count, 0);
      pokemon.LearnMove(moveId, powerPoints, position, actorId);
    }
    if (moves.Count == 5)
    {
      MoveModel move = moves.ElementAt(4 - 1);
      MoveId moveId = new(move.Id);
      pokemon.RelearnMove(moveId, position: 2, actorId);

      move = moves.ElementAt(3 - 1);
      moveId = new(move.Id);
      pokemon.RelearnMove(moveId, position: 1, actorId);

      move = moves.ElementAt(2 - 1);
      moveId = new(move.Id);
      pokemon.RelearnMove(moveId, position: 0, actorId);
    }
    if (moves.Count == 6)
    {
      MoveModel move = moves.ElementAt(4 - 1);
      MoveId moveId = new(move.Id);
      pokemon.RelearnMove(moveId, position: 1, actorId);

      move = moves.ElementAt(3 - 1);
      moveId = new(move.Id);
      pokemon.RelearnMove(moveId, position: 0, actorId);
    }
    if (moves.Count == 7)
    {
      MoveModel move = moves.ElementAt(4 - 1);
      MoveId moveId = new(move.Id);
      pokemon.RelearnMove(moveId, position: 0, actorId);
    }

    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
