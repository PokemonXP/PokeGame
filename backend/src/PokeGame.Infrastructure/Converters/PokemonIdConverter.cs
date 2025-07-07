using PokeGame.Core.Pokemons;
using System.Text.Json.Serialization;

namespace PokeGame.Infrastructure.Converters;

internal class PokemonIdConverter : JsonConverter<PokemonId>
{
  public override PokemonId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new PokemonId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, PokemonId pokemonId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(pokemonId.Value);
  }
}
