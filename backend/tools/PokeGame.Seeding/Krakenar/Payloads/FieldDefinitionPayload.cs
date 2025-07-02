using Krakenar.Contracts.Fields;

namespace PokeGame.Seeding.Krakenar.Payloads;

internal record FieldDefinitionPayload : CreateOrReplaceFieldDefinitionPayload
{
  public Guid Id { get; set; }
}
