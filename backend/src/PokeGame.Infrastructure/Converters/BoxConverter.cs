using PokeGame.Core.Pokemon;

namespace PokeGame.Infrastructure.Converters;

internal class BoxConverter : JsonConverter<Box>
{
  public override Box? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Box(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Box box, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(box.Value);
  }
}
