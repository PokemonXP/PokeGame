using Microsoft.Extensions.Primitives;

namespace PokeGame.Api.Extensions;

internal static class StringValuesExtensions
{
  public static IReadOnlyCollection<string> Sanitize(this StringValues values)
  {
    HashSet<string> sanitized = new(capacity: values.Count);
    foreach (string? value in values)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        sanitized.Add(value.Trim());
      }
    }
    return sanitized.ToList().AsReadOnly();
  }
}
