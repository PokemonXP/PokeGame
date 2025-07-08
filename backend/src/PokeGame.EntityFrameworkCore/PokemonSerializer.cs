namespace PokeGame.EntityFrameworkCore;

internal interface IPokemonSerializer
{
  T? Deserialize<T>(string json);
  string Serialize<T>(T value);
}

internal class PokemonSerializer : IPokemonSerializer
{
  private static IPokemonSerializer? _instance = null;
  public static IPokemonSerializer Instance
  {
    get
    {
      _instance ??= new PokemonSerializer();
      return _instance;
    }
  }

  private readonly JsonSerializerOptions _serializerOptions = new();

  private PokemonSerializer()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _serializerOptions);
  public string Serialize<T>(T value) => JsonSerializer.Serialize<T>(value, _serializerOptions);
}
