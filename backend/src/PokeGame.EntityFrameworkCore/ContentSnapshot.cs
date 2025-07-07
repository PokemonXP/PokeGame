using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Localization;
using Logitar.EventSourcing;

namespace PokeGame.EntityFrameworkCore;

internal class ContentSnapshot
{
  public ContentId Id { get; }
  public long Version { get; }

  public ActorId? CreatedBy { get; }
  public DateTime CreatedOn { get; }

  public ActorId? UpdatedBy { get; }
  public DateTime UpdatedOn { get; }

  public bool IsDeleted { get; }

  private readonly ContentLocale? _latestInvariant = null;
  public ContentLocale LatestInvariant => _latestInvariant ?? throw new InvalidOperationException("The content creation event is missing.");
  private readonly Dictionary<LanguageId, ContentLocale> _latestLocales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocale> LatestLocales => _latestLocales.AsReadOnly();

  public ContentLocale? PublishedInvariant { get; }
  private readonly Dictionary<LanguageId, ContentLocale> _publishedLocales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocale> PublishedLocales => _publishedLocales.AsReadOnly();

  public ContentSnapshot(IEnumerable<DomainEvent> events)
  {
    IOrderedEnumerable<DomainEvent> ordered = events.OrderBy(e => e.Version);
    foreach (DomainEvent @event in ordered)
    {
      if (@event.Version == 1)
      {
        Id = new ContentId(@event.StreamId);

        CreatedBy = @event.ActorId;
        CreatedOn = @event.OccurredOn;
      }

      Version = @event.Version;

      UpdatedBy = @event.ActorId;
      UpdatedOn = @event.OccurredOn;

      if (@event is ContentCreated created)
      {
        _latestInvariant = created.Invariant;
      }
      else if (@event is ContentDeleted)
      {
        IsDeleted = true;
      }
      else if (@event is ContentLocaleChanged changed)
      {
        if (changed.LanguageId.HasValue)
        {
          _latestLocales[changed.LanguageId.Value] = changed.Locale;
        }
        else
        {
          _latestInvariant = changed.Locale;
        }
      }
      else if (@event is ContentLocaleRemoved removed)
      {
        _latestLocales.Remove(removed.LanguageId);
        _publishedLocales.Remove(removed.LanguageId);
      }
      else if (@event is ContentLocalePublished published)
      {
        if (published.LanguageId.HasValue)
        {
          _publishedLocales[published.LanguageId.Value] = _latestLocales[published.LanguageId.Value];
        }
        else
        {
          PublishedInvariant = LatestInvariant;
        }
      }
      else if (@event is ContentLocaleUnpublished unpublished)
      {
        if (unpublished.LanguageId.HasValue)
        {
          _publishedLocales.Remove(unpublished.LanguageId.Value);
        }
        else
        {
          PublishedInvariant = null;
        }
      }
    }
  }

  public override bool Equals(object? obj) => obj is ContentSnapshot snapshot && snapshot.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
