using MediatR;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedMedicinesTask : SeedingTask
{
  public override string? Description => "Seeds Medicine contents into Krakenar.";
}

internal class SeedMedicinesTaskHandler : INotificationHandler<SeedMedicinesTask>
{
  private readonly IItemService _itemService;
  private readonly ILogger<SeedMedicinesTaskHandler> _logger;

  public SeedMedicinesTaskHandler(IItemService itemService, ILogger<SeedMedicinesTaskHandler> logger)
  {
    _itemService = itemService;
    _logger = logger;
  }

  public async Task Handle(SeedMedicinesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedMedicinePayload> medicines = await CsvHelper.ExtractAsync<SeedMedicinePayload>("Game/data/items/medicines.csv", cancellationToken);
    foreach (SeedMedicinePayload medicine in medicines)
    {
      CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(medicine, medicine.Id, cancellationToken);
      _logger.LogInformation("The medicine '{Item}' was {Status}.", result.Item, result.Created ? "created" : "updated");
    }
  }
}
