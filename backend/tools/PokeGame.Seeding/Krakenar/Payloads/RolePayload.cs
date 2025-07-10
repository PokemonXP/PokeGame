using Krakenar.Contracts.Roles;

namespace PokeGame.Seeding.Krakenar.Payloads;

internal record RolePayload : CreateOrReplaceRolePayload
{
  public Guid Id { get; set; }
}
