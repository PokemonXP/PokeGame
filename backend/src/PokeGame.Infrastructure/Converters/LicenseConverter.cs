using PokeGame.Core.Trainers;

namespace PokeGame.Infrastructure.Converters;

internal class LicenseConverter : JsonConverter<License>
{
  public override License? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new License(value);
  }

  public override void Write(Utf8JsonWriter writer, License license, JsonSerializerOptions options)
  {
    writer.WriteStringValue(license.Value);
  }
}
