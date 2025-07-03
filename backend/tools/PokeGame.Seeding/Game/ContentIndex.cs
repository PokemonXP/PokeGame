using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace PokeGame.Seeding.Game;

internal record ContentIndex
{
  private readonly Dictionary<Guid, ContentLocale> _localesById;
  private readonly Dictionary<string, ContentLocale> _localesByUniqueName;

  public ContentIndex()
  {
    _localesById = [];
    _localesByUniqueName = [];
  }
  public ContentIndex(SearchResults<ContentLocale> results) : this(results.Items)
  {
  }
  public ContentIndex(IEnumerable<ContentLocale> locales)
  {
    int capacity = locales.Count();
    _localesById = new(capacity);
    _localesByUniqueName = new(capacity);

    foreach (ContentLocale locale in locales)
    {
      _localesById[locale.Content.Id] = locale;
      _localesByUniqueName[Normalize(locale.UniqueName)] = locale;
    }
  }

  public ContentLocale Find(string idOrUniqueName) => Get(idOrUniqueName) ?? throw new InvalidOperationException($"The content locale '{idOrUniqueName}' was not found.");
  public ContentLocale? Get(string idOrUniqueName)
  {
    ContentLocale? locale;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      if (_localesById.TryGetValue(id, out locale))
      {
        return locale;
      }
    }

    string uniqueName = Normalize(idOrUniqueName);
    _localesByUniqueName.TryGetValue(uniqueName, out locale);

    return locale;
  }

  private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
