using Logitar.EventSourcing;
using EventStream = Logitar.EventSourcing.Stream;

namespace PokeGame.EntityFrameworkCore;

internal static class EventStoreExtensions
{
  public static async Task<IReadOnlyCollection<DomainEvent>> FetchAsync(this IEventStore store, DomainEvent latest, CancellationToken cancellationToken = default)
  {
    FetchOptions options = new()
    {
      ToVersion = latest.Version - 1
    };

    EventStream stream = await store.FetchAsync(latest.StreamId, options, cancellationToken)
      ?? throw new InvalidOperationException($"The event stream 'Id={latest.StreamId}' was not found.");

    List<DomainEvent> events = new(capacity: stream.Events.Count + 1);
    foreach (Event @event in stream.Events)
    {
      if (@event.Data is DomainEvent domainEvent)
      {
        events.Add(domainEvent);
      }
    }
    events.Add(latest);

    return events.OrderBy(e => e.Version).ToList().AsReadOnly();
  }
}
