namespace PokeGame.Core.Forms;

public interface IFormRepository
{
  Task<Form?> LoadAsync(FormId formId, CancellationToken cancellationToken = default);
  Task<Form?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(Form form, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Form> forms, CancellationToken cancellationToken = default);
}
