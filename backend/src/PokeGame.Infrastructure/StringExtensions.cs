namespace PokeGame.Infrastructure;

public static class StringExtensions
{
  public static string Capitalize(this string value)
  {
    if (string.IsNullOrEmpty(value))
    {
      return string.Empty;
    }

    return string.Concat(char.ToUpperInvariant(value.First()), value[1..]);
  }
}
