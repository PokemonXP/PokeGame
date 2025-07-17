using PokeGame.Core.Abilities;

namespace PokeGame.Infrastructure.Converters;

internal class AbilityIdConverter : JsonConverter<AbilityId>
{
  public override AbilityId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new AbilityId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, AbilityId abilityId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(abilityId.Value);
  }
}
