using PokeGame.Core.Pokemons;

namespace PokeGame.Infrastructure.Converters;

internal class LevelConverter : JsonConverter<Level>
{
  public override Level? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Level(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Level level, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(level.Value);
  }
}
