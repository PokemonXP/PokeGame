using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Forms.Validators;

namespace PokeGame.Core.Forms.Commands;

internal record UpdateForm(Guid Id, UpdateFormPayload Payload) : ICommand<FormModel?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateFormHandler : ICommandHandler<UpdateForm, FormModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormManager _formManager;
  private readonly IFormQuerier _formQuerier;
  private readonly IFormRepository _formRepository;

  public UpdateFormHandler(
    IApplicationContext applicationContext,
    IFormManager formManager,
    IFormQuerier formQuerier,
    IFormRepository formRepository)
  {
    _applicationContext = applicationContext;
    _formManager = formManager;
    _formQuerier = formQuerier;
    _formRepository = formRepository;
  }

  public async Task<FormModel?> HandleAsync(UpdateForm command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    UpdateFormPayload payload = command.Payload;
    new UpdateFormValidator(uniqueNameSettings).ValidateAndThrow(payload);

    FormId formId = new(command.Id);
    Form? form = await _formRepository.LoadAsync(formId, cancellationToken);
    if (form is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      form.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      form.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      form.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.IsBattleOnly.HasValue)
    {
      form.IsBattleOnly = payload.IsBattleOnly.Value;
    }
    if (payload.IsMega.HasValue)
    {
      form.IsMega = payload.IsMega.Value;
    }

    if (payload.Height.HasValue)
    {
      form.Height = new Height(payload.Height.Value);
    }
    if (payload.Weight.HasValue)
    {
      form.Weight = new Weight(payload.Weight.Value);
    }

    if (payload.Url is not null)
    {
      form.Url = Url.TryCreate(payload.Url.Value);
    }
    if (payload.Notes is not null)
    {
      form.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    if (payload.Types is not null)
    {
      form.Types = new FormTypes(payload.Types);
    }
    if (payload.Abilities is not null)
    {
      FormAbilities abilities = await _formManager.FindAbilitiesAsync(payload.Abilities, nameof(payload.Abilities), cancellationToken);
      form.Abilities = abilities;
    }
    if (payload.BaseStatistics is not null)
    {
      form.BaseStatistics = new BaseStatistics(payload.BaseStatistics);
    }
    if (payload.Yield is not null)
    {
      form.Yield = new Yield(payload.Yield);
    }
    if (payload.Sprites is not null)
    {
      form.Sprites = new(
        new Url(payload.Sprites.Default),
        new Url(payload.Sprites.Shiny),
        Url.TryCreate(payload.Sprites.Alternative),
        Url.TryCreate(payload.Sprites.AlternativeShiny));
    }

    form.Update(_applicationContext.ActorId);
    await _formManager.SaveAsync(form, cancellationToken);

    return await _formQuerier.ReadAsync(form, cancellationToken);
  }
}
