using PokeGame.Core.Species;

namespace PokeGame.Infrastructure.Converters;

internal class SpeciesIdConverter : JsonConverter<SpeciesId>
{
  public override SpeciesId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SpeciesId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, SpeciesId speciesId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(speciesId.Value);
  }
}
