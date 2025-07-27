using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Commands;

internal record EvolvePokemon(Guid PokemonId, Guid EvolutionId) : ICommand<PokemonModel?>;

/// <exception cref="EvolutionNotFoundException"></exception>
internal class EvolvePokemonHandler : ICommandHandler<EvolvePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IEvolutionRepository _evolutionRepository;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public EvolvePokemonHandler(
    IApplicationContext applicationContext,
    IEvolutionRepository evolutionRepository,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _evolutionRepository = evolutionRepository;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(EvolvePokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    PokemonId pokemonId = new(command.PokemonId);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    EvolutionId evolutionId = new(command.EvolutionId);
    Evolution evolution = await _evolutionRepository.LoadAsync(evolutionId, cancellationToken)
      ?? throw new EvolutionNotFoundException(command.EvolutionId.ToString(), nameof(command.EvolutionId));

    // TODO(fpion): implement

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
