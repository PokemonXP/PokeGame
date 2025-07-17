using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities.Commands;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Abilities.Queries;

namespace PokeGame.Core.Abilities;

public interface IAbilityService
{
  Task<CreateOrReplaceAbilityResult> CreateOrReplaceAsync(CreateOrReplaceAbilityPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<AbilityModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<AbilityModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<AbilityModel>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken = default);
  Task<AbilityModel?> UpdateAsync(Guid id, UpdateAbilityPayload payload, CancellationToken cancellationToken = default);
}

internal class AbilityService : IAbilityService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IAbilityService, AbilityService>()
      .AddTransient<IAbilityManager, AbilityManager>()
      .AddTransient<ICommandHandler<CreateOrReplaceAbility, CreateOrReplaceAbilityResult>, CreateOrReplaceAbilityHandler>()
      .AddTransient<ICommandHandler<DeleteAbility, AbilityModel?>, DeleteAbilityHandler>()
      .AddTransient<ICommandHandler<UpdateAbility, AbilityModel?>, UpdateAbilityHandler>()
      .AddTransient<IQueryHandler<ReadAbility, AbilityModel?>, ReadAbilityHandler>()
      .AddTransient<IQueryHandler<SearchAbilities, SearchResults<AbilityModel>>, SearchAbilitiesHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public AbilityService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceAbilityResult> CreateOrReplaceAsync(CreateOrReplaceAbilityPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceAbility command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<AbilityModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteAbility command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<AbilityModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadAbility query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<AbilityModel>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken)
  {
    SearchAbilities query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<AbilityModel?> UpdateAsync(Guid id, UpdateAbilityPayload payload, CancellationToken cancellationToken)
  {
    UpdateAbility command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
