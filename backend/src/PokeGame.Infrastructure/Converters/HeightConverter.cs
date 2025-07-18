using PokeGame.Core.Forms;

namespace PokeGame.Infrastructure.Converters;

internal class HeightConverter : JsonConverter<Height>
{
  public override Height? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Height(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Height height, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(height.Value);
  }
}
