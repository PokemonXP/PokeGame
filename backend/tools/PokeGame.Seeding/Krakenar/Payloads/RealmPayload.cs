using Krakenar.Contracts.Realms;

namespace PokeGame.Seeding.Krakenar.Payloads;

internal record RealmPayload : CreateOrReplaceRealmPayload
{
  public Guid Id { get; set; }
}
