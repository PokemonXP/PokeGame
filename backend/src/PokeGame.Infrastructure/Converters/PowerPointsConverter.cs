using PokeGame.Core.Moves;

namespace PokeGame.Infrastructure.Converters;

internal class PowerPointsConverter : JsonConverter<PowerPoints>
{
  public override PowerPoints? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new PowerPoints(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, PowerPoints powerPoints, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(powerPoints.Value);
  }
}
