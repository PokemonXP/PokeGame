using MediatR;
using PokeGame.ETL.Models;

namespace PokeGame.ETL.Tasks;

internal class ExtractAbilitiesTask : EtlTask
{
  public override string? Description => "Extract Pokémon abilities.";
}

internal class ExtractAbilitiesTaskHandler : IDisposable, INotificationHandler<ExtractAbilitiesTask>
{
  private readonly HttpClient _client;

  public ExtractAbilitiesTaskHandler()
  {
    _client = new HttpClient();
  }

  public void Dispose()
  {
    _client.Dispose();
    GC.SuppressFinalize(this);
  }

  public async Task Handle(ExtractAbilitiesTask _, CancellationToken cancellationToken)
  {
    ResultPage page = await ExtractAsync(cancellationToken);
    foreach (SearchResult result in page.Results)
    {
      Uri requestUri = new(result.Url, UriKind.Absolute);
      using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

      using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
      response.EnsureSuccessStatusCode();

      Ability ability = await response.Content.ReadFromJsonAsync<Ability>(cancellationToken)
        ?? throw new InvalidOperationException($"The ability was not deserialized: '{requestUri}'.");

      string? displayName = ability.DisplayNames.SingleOrDefault(x => x.Language.Name == "en")?.Name;
      FlavorText[] descriptions = ability.Descriptions.Where(x => x.Language.Name == "en").ToArray();
    }
  }

  private async Task<ResultPage> ExtractAsync(CancellationToken cancellationToken)
  {
    Uri requestUri = new("https://pokeapi.co/api/v2/ability?offset=0&limit=1000", UriKind.Absolute);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<ResultPage>(cancellationToken)
      ?? throw new InvalidOperationException($"The search result page was not deserialized: '{requestUri}'.");
  }
}
