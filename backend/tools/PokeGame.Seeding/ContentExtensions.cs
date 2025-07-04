using Krakenar.Contracts.Fields;

namespace PokeGame.Seeding;

internal static class ContentExtensions
{
  public static void Add(this List<FieldValuePayload> fieldValues, Guid id, object? value)
  {
    FieldValuePayload fieldValue = new(id.ToString(), value?.ToString() ?? string.Empty);
    fieldValues.Add(fieldValue);
  }
}
