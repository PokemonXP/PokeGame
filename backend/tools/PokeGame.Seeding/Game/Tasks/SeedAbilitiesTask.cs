using MediatR;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Models;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedAbilitiesTask : SeedingTask
{
  public override string? Description => "Seeds Ability contents into Krakenar.";
}

internal class SeedAbilitiesTaskHandler : INotificationHandler<SeedAbilitiesTask>
{
  private readonly IAbilityService _abilityService;
  private readonly ILogger<SeedAbilitiesTaskHandler> _logger;

  public SeedAbilitiesTaskHandler(IAbilityService abilityService, ILogger<SeedAbilitiesTaskHandler> logger)
  {
    _abilityService = abilityService;
    _logger = logger;
  }

  public async Task Handle(SeedAbilitiesTask task, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedAbilityPayload.Map()]);
    IReadOnlyCollection<SeedAbilityPayload> abilities = await csv.ExtractAsync<SeedAbilityPayload>("Game/data/abilities.csv", cancellationToken);
    foreach (SeedAbilityPayload ability in abilities)
    {
      CreateOrReplaceAbilityResult result = await _abilityService.CreateOrReplaceAsync(ability, ability.Id, cancellationToken);
      _logger.LogInformation("The ability '{Ability}' was {Status}.", result.Ability, result.Created ? "created" : "updated");
    }
  }
}
