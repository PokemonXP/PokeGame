using PokeGame.Core.Species;

namespace PokeGame.Infrastructure.Converters;

internal class CatchRateConverter : JsonConverter<CatchRate>
{
  public override CatchRate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetByte(out byte value) ? new CatchRate(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, CatchRate catchRate, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(catchRate.Value);
  }
}
