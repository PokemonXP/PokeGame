using PokeGame.Core.Species;

namespace PokeGame.Infrastructure.Converters;

internal class EggCyclesConverter : JsonConverter<EggCycles>
{
  public override EggCycles? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetByte(out byte value) ? new EggCycles(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, EggCycles eggcycles, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(eggcycles.Value);
  }
}
