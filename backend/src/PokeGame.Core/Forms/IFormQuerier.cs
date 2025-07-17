using Krakenar.Contracts.Search;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms;

public interface IFormQuerier
{
  Task<FormId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<FormModel> ReadAsync(Form form, CancellationToken cancellationToken = default);
  Task<FormModel?> ReadAsync(FormId id, CancellationToken cancellationToken = default);
  Task<FormModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FormModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FormModel>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken = default);
}
