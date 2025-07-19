using PokeGame.Core.Pokemon;

namespace PokeGame.Infrastructure.Converters;

internal class NicknameConverter : JsonConverter<Nickname>
{
  public override Nickname? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new Nickname(value);
  }

  public override void Write(Utf8JsonWriter writer, Nickname nickname, JsonSerializerOptions options)
  {
    writer.WriteStringValue(nickname.Value);
  }
}
