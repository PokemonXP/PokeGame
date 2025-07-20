using MediatR;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedBattleItemsTask : SeedingTask
{
  public override string? Description => "Seeds Battle Item contents into Krakenar.";
}

internal class SeedBattleItemsTaskHandler : INotificationHandler<SeedBattleItemsTask>
{
  private readonly ILogger<SeedBattleItemsTaskHandler> _logger;

  public SeedBattleItemsTaskHandler(ILogger<SeedBattleItemsTaskHandler> logger)
  {
    _logger = logger;
  }

  public async Task Handle(SeedBattleItemsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<BattleItemPayload> battleItems = await CsvHelper.ExtractAsync<BattleItemPayload>("Game/data/items/battle.csv", cancellationToken);
    // TODO(fpion): implement
  }
}
