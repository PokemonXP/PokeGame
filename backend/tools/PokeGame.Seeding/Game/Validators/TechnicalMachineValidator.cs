﻿using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class TechnicalMachineValidator : AbstractValidator<TechnicalMachinePayload>
{
  public TechnicalMachineValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

    RuleFor(x => x.Move).NotEmpty();

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
