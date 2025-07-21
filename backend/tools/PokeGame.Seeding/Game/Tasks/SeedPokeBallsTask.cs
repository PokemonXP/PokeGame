using MediatR;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedPokeBallsTask : SeedingTask
{
  public override string? Description => "Seeds PokeBall contents into Krakenar.";
}

internal class SeedPokeBallsTaskHandler : INotificationHandler<SeedPokeBallsTask>
{
  private readonly IItemService _itemService;
  private readonly ILogger<SeedPokeBallsTaskHandler> _logger;

  public SeedPokeBallsTaskHandler(IItemService itemService, ILogger<SeedPokeBallsTaskHandler> logger)
  {
    _itemService = itemService;
    _logger = logger;
  }

  public async Task Handle(SeedPokeBallsTask task, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<SeedPokeBallPayload> pokeBalls = await CsvHelper.ExtractAsync<SeedPokeBallPayload>("Game/data/items/poke_balls.csv", cancellationToken);
    foreach (SeedPokeBallPayload pokeBall in pokeBalls)
    {
      CreateOrReplaceItemResult result = await _itemService.CreateOrReplaceAsync(pokeBall, pokeBall.Id, cancellationToken);
      _logger.LogInformation("The Poké Ball '{Item}' was {Status}.", result.Item, result.Created ? "created" : "updated");
    }
  }
}
