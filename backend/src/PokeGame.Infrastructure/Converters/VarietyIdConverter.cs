using PokeGame.Core.Varieties;

namespace PokeGame.Infrastructure.Converters;

internal class VarietyIdConverter : JsonConverter<VarietyId>
{
  public override VarietyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new VarietyId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, VarietyId varietyId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(varietyId.Value);
  }
}
