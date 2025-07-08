using PokeGame.Core.Pokemons;

namespace PokeGame.Infrastructure.Converters;

internal class PokemonCharacteristicConverter : JsonConverter<PokemonCharacteristic>
{
  public override PokemonCharacteristic? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? text = reader.GetString();
    return string.IsNullOrWhiteSpace(text) ? null : new PokemonCharacteristic(text);
  }

  public override void Write(Utf8JsonWriter writer, PokemonCharacteristic characteristic, JsonSerializerOptions options)
  {
    writer.WriteStringValue(characteristic.Text);
  }
}
