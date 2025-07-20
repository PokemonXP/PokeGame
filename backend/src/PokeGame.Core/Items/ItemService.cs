using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Items.Commands;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Items.Queries;

namespace PokeGame.Core.Items;

public interface IItemService
{
  Task<CreateOrReplaceItemResult> CreateOrReplaceAsync(CreateOrReplaceItemPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<ItemModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken = default);
  Task<ItemModel?> UpdateAsync(Guid id, UpdateItemPayload payload, CancellationToken cancellationToken = default);
}

internal class ItemService : IItemService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IItemService, ItemService>();
    services.AddTransient<IItemManager, ItemManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceItem, CreateOrReplaceItemResult>, CreateOrReplaceItemHandler>();
    services.AddTransient<ICommandHandler<DeleteItem, ItemModel?>, DeleteItemHandler>();
    services.AddTransient<ICommandHandler<UpdateItem, ItemModel?>, UpdateItemHandler>();
    services.AddTransient<IQueryHandler<ReadItem, ItemModel?>, ReadItemHandler>();
    services.AddTransient<IQueryHandler<SearchItems, SearchResults<ItemModel>>, SearchItemsHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public ItemService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceItemResult> CreateOrReplaceAsync(CreateOrReplaceItemPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceItem command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<ItemModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteItem command = new(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<ItemModel?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadItem query = new(id, uniqueName);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken)
  {
    SearchItems query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<ItemModel?> UpdateAsync(Guid id, UpdateItemPayload payload, CancellationToken cancellationToken)
  {
    UpdateItem command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}
