using Krakenar.Core.Contents;
using Krakenar.Core.Fields;

namespace PokeGame.Infrastructure.Data;

public static class ContentExtensions
{
  public static string FindStringValue(this ContentLocale locale, Guid id)
  {
    return TryGetStringValue(locale, id) ?? throw new InvalidOperationException($"The field value 'Id={id}' was not found.");
  }
  public static string GetStringValue(this ContentLocale locale, Guid id, string defaultValue = "")
  {
    return TryGetStringValue(locale, id) ?? defaultValue;
  }
  public static string? TryGetStringValue(this ContentLocale locale, Guid id)
  {
    return TryGetFieldValue(locale, id)?.Value; ;
  }

  private static FieldValue? TryGetFieldValue(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? value) ? value : null;
  }
}
