using CsvHelper;
using PokeGame.Seeding.Game.Payloads;

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

    csv.Context.RegisterClassMap<SeedBattleItemPayload.Map>();
    csv.Context.RegisterClassMap<SeedBerryPayload.Map>();
    csv.Context.RegisterClassMap<SeedEvolutionPayload.Map>();
    csv.Context.RegisterClassMap<SeedFormPayload.Map>();
    csv.Context.RegisterClassMap<SeedItemPayload.Map>();
    csv.Context.RegisterClassMap<SeedMedicinePayload.Map>();
    csv.Context.RegisterClassMap<SeedPokeBallPayload.Map>();
    csv.Context.RegisterClassMap<SeedRegionPayload.Map>();
    csv.Context.RegisterClassMap<SeedTechnicalMachinePayload.Map>();
    csv.Context.RegisterClassMap<SeedTrainerPayload.Map>();
    csv.Context.RegisterClassMap<SeedVarietyMovePayload.Map>();
    csv.Context.RegisterClassMap<SeedVarietyPayload.Map>();

    IAsyncEnumerable<T> records = csv.GetRecordsAsync<T>(cancellationToken);

    List<T> results = [];
    await foreach (T record in records)
    {
      results.Add(record);
    }
    return results.AsReadOnly();
  }
}
