using Krakenar.Contracts.Contents;

namespace PokeGame.Seeding.Krakenar.Payloads;

internal record ContentTypePayload : CreateOrReplaceContentTypePayload
{
  public Guid Id { get; set; }

  public List<FieldDefinitionPayload> Fields { get; set; } = [];
}
