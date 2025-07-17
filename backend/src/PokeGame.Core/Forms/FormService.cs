using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Forms.Commands;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Forms.Queries;

namespace PokeGame.Core.Forms;

public interface IFormService
{
  Task<CreateOrReplaceFormResult> CreateOrReplaceAsync(CreateOrReplaceFormPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<FormModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FormModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<FormModel>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken = default);
  Task<FormModel?> UpdateAsync(Guid id, UpdateFormPayload payload, CancellationToken cancellationToken = default);
}

internal class FormService : IFormService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IFormService, FormService>();
    services.AddTransient<IFormManager, FormManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceForm, CreateOrReplaceFormResult>, CreateOrReplaceFormHandler>();
    services.AddTransient<ICommandHandler<DeleteForm, FormModel?>, DeleteFormHandler>();
    services.AddTransient<ICommandHandler<UpdateForm, FormModel?>, UpdateFormHandler>();
    services.AddTransient<IQueryHandler<ReadForm, FormModel?>, ReadFormHandler>();
    services.AddTransient<IQueryHandler<SearchForms, SearchResults<FormModel>>, SearchFormsHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public FormService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceFormResult> CreateOrReplaceAsync(CreateOrReplaceFormPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceForm command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<FormModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteForm command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<FormModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadForm query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<FormModel>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken)
  {
    SearchForms query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<FormModel?> UpdateAsync(Guid id, UpdateFormPayload payload, CancellationToken cancellationToken)
  {
    UpdateForm command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
