using Krakenar.Contracts;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Core.Forms.Queries;

internal record ReadForm(Guid? Id, string? UniqueName) : IQuery<FormModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadFormHandler : IQueryHandler<ReadForm, FormModel?>
{
  private readonly IFormQuerier _formQuerier;

  public ReadFormHandler(IFormQuerier formQuerier)
  {
    _formQuerier = formQuerier;
  }

  public async Task<FormModel?> HandleAsync(ReadForm query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, FormModel> forms = new(capacity: 2);

    if (query.Id.HasValue)
    {
      FormModel? form = await _formQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (form is not null)
      {
        forms[form.Id] = form;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      FormModel? form = await _formQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (form is not null)
      {
        forms[form.Id] = form;
      }
    }

    if (forms.Count > 1)
    {
      throw TooManyResultsException<FormModel>.ExpectedSingle(forms.Count);
    }

    return forms.SingleOrDefault().Value;
  }
}
