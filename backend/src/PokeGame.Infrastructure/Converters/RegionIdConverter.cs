using PokeGame.Core.Regions;

namespace PokeGame.Infrastructure.Converters;

internal class RegionIdConverter : JsonConverter<RegionId>
{
  public override RegionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RegionId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, RegionId regionId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(regionId.Value);
  }
}
