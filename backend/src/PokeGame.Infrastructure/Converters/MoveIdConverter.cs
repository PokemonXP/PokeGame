using PokeGame.Core.Moves;

namespace PokeGame.Infrastructure.Converters;

internal class MoveIdConverter : JsonConverter<MoveId>
{
  public override MoveId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new MoveId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, MoveId moveId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(moveId.Value);
  }
}
