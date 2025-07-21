using PokeGame.Core.Pokemon;

namespace PokeGame.Infrastructure.Converters;

internal class PositionConverter : JsonConverter<Position>
{
  public override Position? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Position(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Position position, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(position.Value);
  }
}
