using PokeGame.Core.Species;

namespace PokeGame.Infrastructure.Converters;

internal class NumberConverter : JsonConverter<Number>
{
  public override Number? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Number(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Number number, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(number.Value);
  }
}
