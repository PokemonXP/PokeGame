using PokeGame.Core.Varieties;

namespace PokeGame.Infrastructure.Converters;

internal class GenderRatioConverter : JsonConverter<GenderRatio>
{
  public override GenderRatio? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new GenderRatio(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, GenderRatio genderRatio, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(genderRatio.Value);
  }
}
