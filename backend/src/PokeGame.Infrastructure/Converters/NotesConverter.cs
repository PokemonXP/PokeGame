using PokeGame.Core;

namespace PokeGame.Infrastructure.Converters;

internal class NotesConverter : JsonConverter<Notes>
{
  public override Notes? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new Notes(value);
  }

  public override void Write(Utf8JsonWriter writer, Notes notes, JsonSerializerOptions options)
  {
    writer.WriteStringValue(notes.Value);
  }
}
