using Krakenar.Contracts.Search;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms;

public interface IFormQuerier
{
  Task<FormModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FormModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FormModel>> SearchAsync(Guid varietyId, SearchFormsPayload payload, CancellationToken cancellationToken = default);
}
