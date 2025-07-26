using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Evolutions.Models;

namespace PokeGame.Core.Evolutions.Commands;

internal record DeleteEvolution(Guid Id) : ICommand<EvolutionModel?>;

internal class DeleteEvolutionHandler : ICommandHandler<DeleteEvolution, EvolutionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IEvolutionQuerier _evolutionQuerier;
  private readonly IEvolutionRepository _evolutionRepository;

  public DeleteEvolutionHandler(IApplicationContext applicationContext, IEvolutionQuerier evolutionQuerier, IEvolutionRepository evolutionRepository)
  {
    _applicationContext = applicationContext;
    _evolutionQuerier = evolutionQuerier;
    _evolutionRepository = evolutionRepository;
  }

  public async Task<EvolutionModel?> HandleAsync(DeleteEvolution command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    EvolutionId evolutionId = new(command.Id);
    Evolution? evolution = await _evolutionRepository.LoadAsync(evolutionId, cancellationToken);
    if (evolution is null)
    {
      return null;
    }
    EvolutionModel model = await _evolutionQuerier.ReadAsync(evolution, cancellationToken);

    evolution.Delete(actorId);
    await _evolutionRepository.SaveAsync(evolution, cancellationToken);

    return model;
  }
}
