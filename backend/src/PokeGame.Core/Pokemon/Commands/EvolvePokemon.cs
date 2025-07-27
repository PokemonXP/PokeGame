using FluentValidation;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Evolutions;
using PokeGame.Core.Forms;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Species;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon.Commands;

internal record EvolvePokemon(Guid PokemonId, Guid EvolutionId) : ICommand<PokemonModel?>;

/// <exception cref="EvolutionNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class EvolvePokemonHandler : ICommandHandler<EvolvePokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IEvolutionRepository _evolutionRepository;
  private readonly IFormRepository _formRepository;
  private readonly IMoveRepository _moveRepository;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly ISpeciesRepository _speciesRepository;
  private readonly IVarietyRepository _varietyRepository;

  public EvolvePokemonHandler(
    IApplicationContext applicationContext,
    IEvolutionRepository evolutionRepository,
    IFormRepository formRepository,
    IMoveRepository moveRepository,
    IPokemonQuerier pokemonQuerier,
    ISpeciesRepository speciesRepository,
    IVarietyRepository varietyRepository,
    IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _evolutionRepository = evolutionRepository;
    _formRepository = formRepository;
    _moveRepository = moveRepository;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _speciesRepository = speciesRepository;
    _varietyRepository = varietyRepository;
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

    Form form = await _formRepository.LoadAsync(evolution.TargetId, cancellationToken)
      ?? throw new InvalidOperationException($"The form 'Id={evolution.TargetId}' was not loaded.");
    Variety variety = await _varietyRepository.LoadAsync(form.VarietyId, cancellationToken)
      ?? throw new InvalidOperationException($"The variety 'Id={form.VarietyId}' was not loaded.");
    PokemonSpecies species = await _speciesRepository.LoadAsync(variety.SpeciesId, cancellationToken)
      ?? throw new InvalidOperationException($"The species 'Id={variety.SpeciesId}' was not loaded.");

    pokemon.Evolve(species, variety, form, evolution, actorId);

    IEnumerable<MoveId> moveIds = variety.Moves.Where(x => x.Value is null).Select(x => x.Key);
    IReadOnlyCollection<Move> moves = await _moveRepository.LoadAsync(moveIds, cancellationToken);
    foreach (Move move in moves)
    {
      pokemon.LearnMove(move, position: null, level: null, MoveLearningMethod.Evolving, notes: null, actorId);
    }

    await _pokemonRepository.SaveAsync(pokemon, cancellationToken);

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
