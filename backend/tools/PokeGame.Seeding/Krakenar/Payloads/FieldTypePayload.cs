using Krakenar.Contracts.Fields;

namespace PokeGame.Seeding.Krakenar.Payloads;

internal record FieldTypePayload : CreateOrReplaceFieldTypePayload
{
  public Guid Id { get; set; }
}
