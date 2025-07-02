namespace PokeGame.Seeding;

internal static class SeedingSerializer
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedingSerializer()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _serializerOptions);
  public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, _serializerOptions);
}
