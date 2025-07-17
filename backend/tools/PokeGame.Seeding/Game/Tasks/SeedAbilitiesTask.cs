using MediatR;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Models;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedAbilitiesTask : SeedingTask
{
  public override string? Description => "Seeds Ability contents into Krakenar.";
  public string Language { get; }

  public SeedAbilitiesTask(string language)
  {
    Language = language;
  }
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
    IReadOnlyCollection<SeedAbilityPayload> abilities = await CsvHelper.ExtractAsync<SeedAbilityPayload>("Game/data/abilities.csv", cancellationToken);
    foreach (SeedAbilityPayload ability in abilities)
    {
      CreateOrReplaceAbilityResult result = await _abilityService.CreateOrReplaceAsync(ability, ability.Id, cancellationToken);
      _logger.LogInformation("The ability '{Ability}' was {Status}.", result.Ability, result.Created ? "created" : "updated");
    }
  }
}
