using PokeGame.Core.Evolutions;

namespace PokeGame.Infrastructure.Converters;

internal class EvolutionIdConverter : JsonConverter<EvolutionId>
{
  public override EvolutionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new EvolutionId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, EvolutionId evolutionId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(evolutionId.Value);
  }
}
