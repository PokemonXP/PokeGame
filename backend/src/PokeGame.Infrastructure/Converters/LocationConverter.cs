using PokeGame.Core.Regions;

namespace PokeGame.Infrastructure.Converters;

internal class LocationConverter : JsonConverter<Location>
{
  public override Location? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new Location(value);
  }

  public override void Write(Utf8JsonWriter writer, Location location, JsonSerializerOptions options)
  {
    writer.WriteStringValue(location.Value);
  }
}
