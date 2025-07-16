using PokeGame.Core;

namespace PokeGame.Infrastructure.Converters;

internal class FriendshipConverter : JsonConverter<Friendship>
{
  public override Friendship? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetByte(out byte value) ? new Friendship(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Friendship friendship, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(friendship.Value);
  }
}
