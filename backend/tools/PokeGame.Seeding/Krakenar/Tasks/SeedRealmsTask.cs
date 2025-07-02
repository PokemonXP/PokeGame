using Krakenar.Contracts.Realms;
using Krakenar.Core;
using MediatR;
using PokeGame.Seeding.Krakenar.Payloads;

namespace PokeGame.Seeding.Krakenar.Tasks;

internal class SeedRealmsTask : SeedingTask
{
  public override string? Description => "Seeds the Realms into Krakenar.";
}

internal class SeedRealmsTaskHandler : INotificationHandler<SeedRealmsTask>
{
  private readonly SeedingApplicationContext _applicationContext;
  private readonly ILogger<SeedRealmsTaskHandler> _logger;
  private readonly IRealmService _realmService;

  public SeedRealmsTaskHandler(IApplicationContext applicationContext, ILogger<SeedRealmsTaskHandler> logger, IRealmService realmService)
  {
    _applicationContext = (SeedingApplicationContext)applicationContext;
    _logger = logger;
    _realmService = realmService;
  }

  public async Task Handle(SeedRealmsTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Krakenar/data/realms.json", Encoding.UTF8, cancellationToken);
    IEnumerable<RealmPayload>? payloads = SeedingSerializer.Deserialize<IEnumerable<RealmPayload>>(json);
    if (payloads is not null)
    {
      foreach (RealmPayload payload in payloads)
      {
        CreateOrReplaceRealmResult result = await _realmService.CreateOrReplaceAsync(payload, payload.Id, version: null, cancellationToken);
        Realm realm = result.Realm ?? throw new InvalidOperationException($"'RealmService.CreateOrReplaceAsync' returned null for realm 'Id={payload.Id}'.");
        _logger.LogInformation("The realm '{Realm}' was '{Action}'.", realm.DisplayName ?? realm.UniqueSlug, result.Created ? "created" : "replaced");
        _applicationContext.Realm ??= realm;
      }
    }
  }
}
