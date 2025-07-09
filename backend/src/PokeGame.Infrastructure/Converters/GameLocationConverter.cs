using PokeGame.Core;

namespace PokeGame.Infrastructure.Converters;

internal class GameLocationConverter : JsonConverter<GameLocation>
{
  public override GameLocation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new GameLocation(value);
  }

  public override void Write(Utf8JsonWriter writer, GameLocation location, JsonSerializerOptions options)
  {
    writer.WriteStringValue(location.Value);
  }
}
