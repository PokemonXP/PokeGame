using MediatR;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedTechnicalMachinesTask : SeedingTask
{
  public override string? Description => "Seeds Technical Machine contents into Krakenar.";
}

internal class SeedTechnicalMachinesTaskHandler : INotificationHandler<SeedTechnicalMachinesTask>
{
  private readonly IItemService _itemService;
  private readonly ILogger<SeedTechnicalMachinesTaskHandler> _logger;

  public SeedTechnicalMachinesTaskHandler(IItemService itemService, ILogger<SeedTechnicalMachinesTaskHandler> logger)
  {
    _itemService = itemService;
    _logger = logger;
  }

  public async Task Handle(SeedTechnicalMachinesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedTechnicalMachinePayload> technicalMachines = await CsvHelper.ExtractAsync<SeedTechnicalMachinePayload>("Game/data/items/technical_machines.csv", cancellationToken);
    foreach (SeedTechnicalMachinePayload technicalMachine in technicalMachines)
    {
      CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(technicalMachine, technicalMachine.Id, cancellationToken);
      _logger.LogInformation("The technical machine (TM) '{Item}' was {Status}.", result.Item, result.Created ? "created" : "updated");
    }
  }
}
