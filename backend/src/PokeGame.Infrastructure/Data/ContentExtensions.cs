using Krakenar.Core.Contents;
using Krakenar.Core.Fields;

namespace PokeGame.Infrastructure.Data;

public static class ContentExtensions
{
  public static bool FindBooleanValue(this ContentLocale locale, Guid id)
  {
    return TryGetBooleanValue(locale, id) ?? throw new InvalidOperationException($"The field value 'Id={id}' was not found.");
  }
  public static bool GetBooleanValue(this ContentLocale locale, Guid id, bool defaultValue = false)
  {
    return TryGetBooleanValue(locale, id) ?? defaultValue;
  }
  public static bool? TryGetBooleanValue(this ContentLocale locale, Guid id)
  {
    string? value = TryGetFieldValue(locale, id)?.Value;
    return string.IsNullOrWhiteSpace(value) ? null : bool.Parse(value);
  }

  public static double FindNumberValue(this ContentLocale locale, Guid id)
  {
    return TryGetNumberValue(locale, id) ?? throw new InvalidOperationException($"The field value 'Id={id}' was not found.");
  }
  public static double GetNumberValue(this ContentLocale locale, Guid id, double defaultValue = 0.0)
  {
    return TryGetNumberValue(locale, id) ?? defaultValue;
  }
  public static double? TryGetNumberValue(this ContentLocale locale, Guid id)
  {
    string? value = TryGetFieldValue(locale, id)?.Value;
    return string.IsNullOrWhiteSpace(value) ? null : double.Parse(value);
  }

  public static IReadOnlyCollection<Guid> FindRelatedContentValue(this ContentLocale locale, Guid id)
  {
    return TryGetRelatedContentValue(locale, id) ?? throw new InvalidOperationException($"The field value 'Id={id}' was not found.");
  }
  public static IReadOnlyCollection<Guid> GetRelatedContentValue(this ContentLocale locale, Guid id)
  {
    return TryGetRelatedContentValue(locale, id) ?? [];
  }
  public static IReadOnlyCollection<Guid>? TryGetRelatedContentValue(this ContentLocale locale, Guid id)
  {
    string? value = TryGetFieldValue(locale, id)?.Value;
    return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(value);
  }

  public static IReadOnlyCollection<string> FindSelectValue(this ContentLocale locale, Guid id)
  {
    return TryGetSelectValue(locale, id) ?? throw new InvalidOperationException($"The field value 'Id={id}' was not found.");
  }
  public static IReadOnlyCollection<string> GetSelectValue(this ContentLocale locale, Guid id)
  {
    return TryGetSelectValue(locale, id) ?? [];
  }
  public static IReadOnlyCollection<string>? TryGetSelectValue(this ContentLocale locale, Guid id)
  {
    string? value = TryGetFieldValue(locale, id)?.Value;
    return string.IsNullOrWhiteSpace(value) ? null : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(value);
  }

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
    return TryGetFieldValue(locale, id)?.Value;
  }

  private static FieldValue? TryGetFieldValue(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? value) ? value : null;
  }
}
