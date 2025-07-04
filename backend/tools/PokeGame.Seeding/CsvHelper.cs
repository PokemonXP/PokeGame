using CsvHelper;
using PokeGame.Seeding.Game.Payloads;
using System.Globalization;

namespace PokeGame.Seeding;

internal static class CsvHelper
{
  public static async Task<IReadOnlyCollection<T>> ExtractAsync<T>(string path, CancellationToken cancellationToken)
  {
    return await ExtractAsync<T>(path, Encoding.UTF8, CultureInfo.InvariantCulture, cancellationToken);
  }
  public static async Task<IReadOnlyCollection<T>> ExtractAsync<T>(string path, Encoding encoding, CultureInfo culture, CancellationToken cancellationToken)
  {
    using StreamReader reader = new(path, encoding);
    using CsvReader csv = new(reader, culture);

    csv.Context.RegisterClassMap<AbilityPayload.Map>();
    csv.Context.RegisterClassMap<MovePayload.Map>();
    csv.Context.RegisterClassMap<RegionPayload.Map>();
    csv.Context.RegisterClassMap<SpeciesPayload.Map>();
    csv.Context.RegisterClassMap<VarietyPayload.Map>();

    IAsyncEnumerable<T> records = csv.GetRecordsAsync<T>(cancellationToken);

    List<T> results = [];
    await foreach (T record in records)
    {
      results.Add(record);
    }
    return results.AsReadOnly();
  }
}
