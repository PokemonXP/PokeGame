using MediatR;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedTechnicalMachinesTask : SeedingTask
{
  public override string? Description => "Seeds Technical Machine contents into Krakenar.";
}

internal class SeedTechnicalMachinesTaskHandler : INotificationHandler<SeedTechnicalMachinesTask>
{
  private readonly ILogger<SeedTechnicalMachinesTaskHandler> _logger;

  public SeedTechnicalMachinesTaskHandler(ILogger<SeedTechnicalMachinesTaskHandler> logger)
  {
    _logger = logger;
  }

  public async Task Handle(SeedTechnicalMachinesTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<TechnicalMachinePayload> technicalMachines = await CsvHelper.ExtractAsync<TechnicalMachinePayload>("Game/data/items/technical_machines.csv", cancellationToken);
    // TODO(fpion): implement
  }
}
