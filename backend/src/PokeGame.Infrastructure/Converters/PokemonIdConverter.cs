using PokeGame.Core.Pokemon;

namespace PokeGame.Infrastructure.Converters;

internal class PokemonIdConverter : JsonConverter<PokemonId>
{
  public override PokemonId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new PokemonId() : new(value);
  }

  public override PokemonId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    PokemonId id = Read(ref reader, typeToConvert, options);
    if (string.IsNullOrWhiteSpace(id.Value))
    {
      throw new InvalidOperationException("The identifier could not be read.");
    }
    return id;
  }

  public override void Write(Utf8JsonWriter writer, PokemonId pokemonId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(pokemonId.Value);
  }

  public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] PokemonId pokemonId, JsonSerializerOptions options)
  {
    writer.WritePropertyName(pokemonId.Value);
  }
}
