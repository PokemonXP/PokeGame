using FluentValidation;
using Krakenar.Core;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons.Commands;

internal record UpdatePokemon(Guid Id, UpdatePokemonPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="PokemonUniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdatePokemonHandler : ICommandHandler<UpdatePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormQuerier _formQuerier;
  private readonly IPokemonManager _pokemonManager;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public UpdatePokemonHandler(
    IApplicationContext applicationContext,
    IFormQuerier formQuerier,
    IPokemonManager pokemonManager,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _formQuerier = formQuerier;
    _pokemonManager = pokemonManager;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(UpdatePokemon command, CancellationToken cancellationToken)
  {
    //ActorId? actorId = _applicationContext.ActorId;
    //IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    //UpdatePokemonPayload payload = command.Payload;
    //new UpdatePokemonValidator(uniqueNameSettings).ValidateAndThrow(payload);

    //PokemonId pokemonId = new(command.Id);
    //Pokemon? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    //if (pokemon is null)
    //{
    //  return null;
    //}

    //if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    //{
    //  UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    //  pokemon.SetUniqueName(uniqueName, actorId);
    //}
    //if (payload.Nickname is not null)
    //{
    //  DisplayName? nickname = DisplayName.TryCreate(payload.Nickname.Value);
    //  pokemon.SetNickname(nickname, actorId);
    //}
    //if (payload.Gender.HasValue)
    //{
    //  FormModel form = await _formQuerier.ReadAsync(pokemon.FormId, cancellationToken);
    //  new UpdatePokemonValidator(uniqueNameSettings, form).ValidateAndThrow(payload);

    //  pokemon.Gender = payload.Gender.Value;
    //}

    //if (payload.Vitality.HasValue)
    //{
    //  pokemon.Vitality = payload.Vitality.Value;
    //}
    //if (payload.Stamina.HasValue)
    //{
    //  pokemon.Stamina = payload.Stamina.Value;
    //}
    //if (payload.StatusCondition is not null)
    //{
    //  pokemon.StatusCondition = payload.StatusCondition.Value;
    //}

    //if (payload.Friendship.HasValue)
    //{
    //  pokemon.Friendship = payload.Friendship.Value;
    //}

    //if (payload.Sprite is not null)
    //{
    //  pokemon.Sprite = Url.TryCreate(payload.Sprite.Value);
    //}
    //if (payload.Url is not null)
    //{
    //  pokemon.Url = Url.TryCreate(payload.Url.Value);
    //}
    //if (payload.Notes is not null)
    //{
    //  pokemon.Notes = Description.TryCreate(payload.Notes.Value);
    //}

    //pokemon.Update(actorId);
    //await _pokemonManager.SaveAsync(pokemon, cancellationToken);

    //return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);

    await Task.Delay(1000, cancellationToken);
    return null;
  }
}
