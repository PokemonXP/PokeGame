using PokeGame.Core.Forms.Events;

namespace PokeGame.Core.Forms;

internal interface IFormManager
{
  Task SaveAsync(Form form, CancellationToken cancellationToken = default);
}

internal class FormManager : IFormManager
{
  private readonly IFormQuerier _formQuerier;
  private readonly IFormRepository _formRepository;

  public FormManager(IFormQuerier formQuerier, IFormRepository formRepository)
  {
    _formQuerier = formQuerier;
    _formRepository = formRepository;
  }

  public async Task SaveAsync(Form form, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = form.Changes.Any(change => change is FormCreated || change is FormUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      FormId? conflictId = await _formQuerier.FindIdAsync(form.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(form.Id))
      {
        throw new UniqueNameAlreadyUsedException(form, conflictId.Value);
      }
    }

    await _formRepository.SaveAsync(form, cancellationToken);
  }
}
