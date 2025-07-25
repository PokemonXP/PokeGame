﻿using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;

namespace PokeGame.Core.Pokemon.Commands;

internal record UpdatePokemon(Guid Id, UpdatePokemonPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="ValidationException"></exception>
internal class UpdatePokemonHandler : ICommandHandler<UpdatePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IItemRepository _itemRepository;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public UpdatePokemonHandler(
    IApplicationContext applicationContext,
    IItemRepository itemRepository,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _itemRepository = itemRepository;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(UpdatePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdatePokemonPayload payload = command.Payload;
    new UpdatePokemonValidator(uniqueNameSettings).ValidateAndThrow(payload);

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      pokemon.SetUniqueName(uniqueName, actorId);
    }
    if (payload.Nickname is not null)
    {
      Nickname? nickname = Nickname.TryCreate(payload.Nickname.Value);
      pokemon.SetNickname(nickname, actorId);
    }
    if (payload.IsShiny.HasValue)
    {
      pokemon.IsShiny = payload.IsShiny.Value;
    }

    if (payload.Vitality.HasValue)
    {
      pokemon.Vitality = payload.Vitality.Value;
    }
    if (payload.Stamina.HasValue)
    {
      pokemon.Stamina = payload.Stamina.Value;
    }
    if (payload.StatusCondition is not null)
    {
      pokemon.StatusCondition = payload.StatusCondition.Value;
    }
    if (payload.Friendship.HasValue)
    {
      pokemon.Friendship = new Friendship(payload.Friendship.Value);
    }

    if (payload.HeldItem is not null)
    {
      if (string.IsNullOrWhiteSpace(payload.HeldItem.Value))
      {
        pokemon.RemoveItem(actorId);
      }
      else
      {
        Item item = await _itemRepository.LoadAsync(payload.HeldItem.Value, cancellationToken)
          ?? throw new ItemNotFoundException(payload.HeldItem.Value, nameof(payload.HeldItem));
        pokemon.HoldItem(item, actorId);
      }
    }

    if (payload.Sprite is not null)
    {
      pokemon.Sprite = Url.TryCreate(payload.Sprite.Value);
    }
    if (payload.Url is not null)
    {
      pokemon.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      pokemon.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    pokemon.Update(actorId);
    await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
