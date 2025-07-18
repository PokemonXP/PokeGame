using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Commands;

internal record DeleteForm(Guid Id) : ICommand<FormModel?>;

internal class DeleteFormHandler : ICommandHandler<DeleteForm, FormModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormQuerier _formQuerier;
  private readonly IFormRepository _formRepository;

  public DeleteFormHandler(IApplicationContext applicationContext, IFormQuerier formQuerier, IFormRepository formRepository)
  {
    _applicationContext = applicationContext;
    _formQuerier = formQuerier;
    _formRepository = formRepository;
  }

  public async Task<FormModel?> HandleAsync(DeleteForm command, CancellationToken cancellationToken)
  {
    FormId formId = new(command.Id);
    Form? form = await _formRepository.LoadAsync(formId, cancellationToken);
    if (form is null)
    {
      return null;
    }
    FormModel model = await _formQuerier.ReadAsync(form, cancellationToken);

    form.Delete(_applicationContext.ActorId);
    await _formRepository.SaveAsync(form, cancellationToken);

    return model;
  }
}
