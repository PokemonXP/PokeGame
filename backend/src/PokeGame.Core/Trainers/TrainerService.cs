using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Trainers.Commands;
using PokeGame.Core.Trainers.Models;
using PokeGame.Core.Trainers.Queries;

namespace PokeGame.Core.Trainers;

public interface ITrainerService
{
  Task<CreateOrReplaceTrainerResult> CreateOrReplaceAsync(CreateOrReplaceTrainerPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<TrainerModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TrainerModel?> ReadAsync(Guid? id = null, string? uniqueName = null, string? license = null, CancellationToken cancellationToken = default);
  Task<SearchResults<TrainerModel>> SearchAsync(SearchTrainersPayload payload, CancellationToken cancellationToken = default);
  Task<TrainerModel?> UpdateAsync(Guid id, UpdateTrainerPayload payload, CancellationToken cancellationToken = default);
}

internal class TrainerService : ITrainerService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ITrainerService, TrainerService>();
    services.AddTransient<ITrainerManager, TrainerManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceTrainer, CreateOrReplaceTrainerResult>, CreateOrReplaceTrainerHandler>();
    services.AddTransient<ICommandHandler<DeleteTrainer, TrainerModel?>, DeleteTrainerHandler>();
    services.AddTransient<ICommandHandler<UpdateTrainer, TrainerModel?>, UpdateTrainerHandler>();
    services.AddTransient<IQueryHandler<ReadTrainer, TrainerModel?>, ReadTrainerHandler>();
    services.AddTransient<IQueryHandler<SearchTrainers, SearchResults<TrainerModel>>, SearchTrainersHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public TrainerService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceTrainerResult> CreateOrReplaceAsync(CreateOrReplaceTrainerPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceTrainer command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<TrainerModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteTrainer command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<TrainerModel?> ReadAsync(Guid? id, string? uniqueName, string? license, CancellationToken cancellationToken)
  {
    ReadTrainer query = new(id, uniqueName, license);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<TrainerModel>> SearchAsync(SearchTrainersPayload payload, CancellationToken cancellationToken)
  {
    SearchTrainers query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<TrainerModel?> UpdateAsync(Guid id, UpdateTrainerPayload payload, CancellationToken cancellationToken)
  {
    UpdateTrainer command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
