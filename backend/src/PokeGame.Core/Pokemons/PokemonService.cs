using Krakenar.Core;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core.Pokemons;

public interface IPokemonService
{
  Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken = default);
}

internal class PokemonService : IPokemonService
{
  private readonly ICommandHandler<CreatePokemon, PokemonModel> _createPokemon;

  public PokemonService(ICommandHandler<CreatePokemon, PokemonModel> createPokemon)
  {
    _createPokemon = createPokemon;
  }

  public async Task<PokemonModel> CreateAsync(CreatePokemonPayload payload, CancellationToken cancellationToken)
  {
    CreatePokemon command = new(payload);
    return await _createPokemon.HandleAsync(command, cancellationToken);
  }
}
