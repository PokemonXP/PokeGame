namespace PokeGame.Core;

internal static class RandomExtensions
{
  public static T Pick<T>(this Random random, IReadOnlyCollection<T> items)
  {
    int index = random.Next(0, items.Count);
    return items.ElementAt(index);
  }
}
