using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Forms;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Repositories;

internal class FormRepository : Repository, IFormRepository
{
  private readonly DbSet<FormEntity> _forms;

  public FormRepository(PokemonContext context, IEventStore eventStore) : base(eventStore)
  {
    _forms = context.Forms;
  }

  public async Task<Form?> LoadAsync(FormId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Form>(id.StreamId, cancellationToken);
  }
  public async Task<Form?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    FormId formId;
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      formId = new(id);
      Form? variety = await LoadAsync(formId, cancellationToken);
      if (variety is not null)
      {
        return variety;
      }
    }

    string uniqueNameNormalized = Helper.Normalize(idOrUniqueName);
    string? streamId = await _forms.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(streamId))
    {
      return null;
    }

    formId = new(streamId);
    return await LoadAsync(formId, cancellationToken);
  }

  public async Task SaveAsync(Form form, CancellationToken cancellationToken)
  {
    await base.SaveAsync(form, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Form> forms, CancellationToken cancellationToken)
  {
    await base.SaveAsync(forms, cancellationToken);
  }
}
