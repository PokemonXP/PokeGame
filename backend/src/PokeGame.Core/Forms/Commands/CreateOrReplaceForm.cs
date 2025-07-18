using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Forms.Validators;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Forms.Commands;

internal record CreateOrReplaceForm(CreateOrReplaceFormPayload Payload, Guid? Id) : ICommand<CreateOrReplaceFormResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceFormHandler : ICommandHandler<CreateOrReplaceForm, CreateOrReplaceFormResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormManager _formManager;
  private readonly IFormQuerier _formQuerier;
  private readonly IFormRepository _formRepository;
  private readonly IVarietyRepository _varietyRepository;

  public CreateOrReplaceFormHandler(
    IApplicationContext applicationContext,
    IFormManager formManager,
    IFormQuerier formQuerier,
    IFormRepository formRepository,
    IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _formManager = formManager;
    _formQuerier = formQuerier;
    _formRepository = formRepository;
    _varietyRepository = varietyRepository;
  }

  public async Task<CreateOrReplaceFormResult> HandleAsync(CreateOrReplaceForm command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;
    IUniqueNameSettings uniqueNameSettings = _applicationContext.UniqueNameSettings;

    CreateOrReplaceFormPayload payload = command.Payload;
    new CreateOrReplaceFormValidator(uniqueNameSettings).ValidateAndThrow(payload);

    FormId formId = FormId.NewId();
    Form? form = null;
    if (command.Id.HasValue)
    {
      formId = new(command.Id.Value);
      form = await _formRepository.LoadAsync(formId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    Height height = new(payload.Height);
    Weight weight = new(payload.Weight);
    FormTypes types = new(payload.Types);
    FormAbilities abilities = await _formManager.FindAbilitiesAsync(payload.Abilities, nameof(payload.Abilities), cancellationToken);
    BaseStatistics baseStatistics = new(payload.BaseStatistics);
    Yield yield = new(payload.Yield);
    Sprites sprites = new(
      new Url(payload.Sprites.Default),
      new Url(payload.Sprites.Shiny),
      Url.TryCreate(payload.Sprites.Alternative),
      Url.TryCreate(payload.Sprites.AlternativeShiny));

    bool created = false;
    if (form is null)
    {
      Variety variety = await _varietyRepository.LoadAsync(payload.Variety, cancellationToken)
        ?? throw new VarietyNotFoundException(payload.Variety, nameof(payload.Variety));

      form = new(variety, uniqueName, types, abilities, baseStatistics, yield, sprites, payload.IsDefault, payload.IsBattleOnly, payload.IsMega, height, weight, actorId, formId);
      created = true;
    }
    else
    {
      form.SetUniqueName(uniqueName, actorId);

      form.IsBattleOnly = payload.IsBattleOnly;
      form.IsMega = payload.IsMega;

      form.Height = height;
      form.Weight = weight;

      form.Types = types;
      form.Abilities = abilities;
      form.BaseStatistics = baseStatistics;
      form.Yield = yield;
      form.Sprites = sprites;
    }

    form.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    form.Description = Description.TryCreate(payload.Description);

    form.Url = Url.TryCreate(payload.Url);
    form.Notes = Notes.TryCreate(payload.Notes);

    form.Update(actorId);
    await _formManager.SaveAsync(form, cancellationToken);

    FormModel model = await _formQuerier.ReadAsync(form, cancellationToken);
    return new CreateOrReplaceFormResult(model, created);
  }
}
