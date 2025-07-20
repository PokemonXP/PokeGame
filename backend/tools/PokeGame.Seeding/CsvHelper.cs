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

    csv.Context.RegisterClassMap<SeedAbilityPayload.Map>();
    csv.Context.RegisterClassMap<SeedFormPayload.Map>();
    csv.Context.RegisterClassMap<SeedMedicinePayload.Map>();
    csv.Context.RegisterClassMap<SeedMovePayload.Map>();
    csv.Context.RegisterClassMap<SeedRegionPayload.Map>();
    csv.Context.RegisterClassMap<SeedSpeciesPayload.Map>();
    csv.Context.RegisterClassMap<SeedTrainerPayload.Map>();
    csv.Context.RegisterClassMap<SeedVarietyMovePayload.Map>();
    csv.Context.RegisterClassMap<SeedVarietyPayload.Map>();

    csv.Context.RegisterClassMap<BattleItemPayload.Map>();
    csv.Context.RegisterClassMap<BerryPayload.Map>();
    csv.Context.RegisterClassMap<ItemPayload.Map>();
    csv.Context.RegisterClassMap<PokeBallPayload.Map>();
    csv.Context.RegisterClassMap<TechnicalMachinePayload.Map>();

    IAsyncEnumerable<T> records = csv.GetRecordsAsync<T>(cancellationToken);

    List<T> results = [];
    await foreach (T record in records)
    {
      results.Add(record);
    }
    return results.AsReadOnly();
  }
}
