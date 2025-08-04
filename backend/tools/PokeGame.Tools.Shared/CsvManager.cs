using CsvHelper;
using CsvHelper.Configuration;

namespace PokeGame.Tools.Shared;

public interface ICsvManager
{
  Task<IReadOnlyCollection<T>> ExtractAsync<T>(string path, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<T>> ExtractAsync<T>(string path, Encoding encoding, CultureInfo culture, CancellationToken cancellationToken = default);

  Task SaveAsync<T>(IEnumerable<T> records, string path, CancellationToken cancellationToken = default);
  Task SaveAsync<T>(IEnumerable<T> records, string path, Encoding encoding, CultureInfo culture, CancellationToken cancellationToken = default);
}

public class CsvManager : ICsvManager
{
  private readonly IEnumerable<ClassMap> _maps;

  public CsvManager(IEnumerable<ClassMap> maps)
  {
    _maps = maps;
  }

  public async Task<IReadOnlyCollection<T>> ExtractAsync<T>(string path, CancellationToken cancellationToken)
  {
    return await ExtractAsync<T>(path, Encoding.UTF8, CultureInfo.InvariantCulture, cancellationToken);
  }
  public async Task<IReadOnlyCollection<T>> ExtractAsync<T>(string path, Encoding encoding, CultureInfo culture, CancellationToken cancellationToken)
  {
    using StreamReader reader = new(path, encoding);
    using CsvReader csv = new(reader, culture);

    foreach (ClassMap map in _maps)
    {
      csv.Context.RegisterClassMap(map);
    }

    IAsyncEnumerable<T> records = csv.GetRecordsAsync<T>(cancellationToken);

    List<T> results = [];
    await foreach (T record in records)
    {
      results.Add(record);
    }
    return results.AsReadOnly();
  }

  public async Task SaveAsync<T>(IEnumerable<T> records, string path, CancellationToken cancellationToken)
  {
    await SaveAsync(records, path, Encoding.UTF8, CultureInfo.InvariantCulture, cancellationToken);
  }
  public async Task SaveAsync<T>(IEnumerable<T> records, string path, Encoding encoding, CultureInfo culture, CancellationToken cancellationToken)
  {
    using StreamWriter writer = new(path, encoding, new FileStreamOptions
    {
      Access = FileAccess.Write
    });
    using CsvWriter csv = new(writer, culture);

    foreach (ClassMap map in _maps)
    {
      csv.Context.RegisterClassMap(map);
    }

    await csv.WriteRecordsAsync(records, cancellationToken);
  }
}
