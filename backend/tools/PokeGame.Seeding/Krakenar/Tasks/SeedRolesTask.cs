using Krakenar.Contracts.Roles;
using MediatR;
using PokeGame.Seeding.Krakenar.Payloads;

namespace PokeGame.Seeding.Krakenar.Tasks;

internal class SeedRolesTask : SeedingTask
{
  public override string? Description => "Seeds the Roles into Krakenar.";
}

internal class SeedRolesTaskHandler : INotificationHandler<SeedRolesTask>
{
  private readonly ILogger<SeedRolesTaskHandler> _logger;
  private readonly IRoleService _roleService;

  public SeedRolesTaskHandler(IRoleService roleService, ILogger<SeedRolesTaskHandler> logger)
  {
    _roleService = roleService;
    _logger = logger;
  }

  public async Task Handle(SeedRolesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Krakenar/data/roles.json", Encoding.UTF8, cancellationToken);
    IEnumerable<RolePayload>? payloads = SeedingSerializer.Deserialize<IEnumerable<RolePayload>>(json);
    if (payloads is not null)
    {
      foreach (RolePayload payload in payloads)
      {
        CreateOrReplaceRoleResult result = await _roleService.CreateOrReplaceAsync(payload, payload.Id, version: null, cancellationToken);
        Role role = result.Role ?? throw new InvalidOperationException($"'RoleService.CreateOrReplaceAsync' returned null for role 'Id={payload.Id}'.");
        _logger.LogInformation("The role '{Role}' was {Action}.", role.DisplayName ?? role.UniqueName, result.Created ? "created" : "replaced");
      }
    }
  }
}
