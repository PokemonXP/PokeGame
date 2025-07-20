using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Pokemon.Models;

namespace PokeGame.Core.Pokemon.Validators;

internal class UpdatePokemonValidator : AbstractValidator<UpdatePokemonPayload>
{
  public UpdatePokemonValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.Nickname?.Value), () => RuleFor(x => x.Nickname!.Value!).Nickname());

    RuleFor(x => x.Vitality).InclusiveBetween(0, 999);
    RuleFor(x => x.Stamina).InclusiveBetween(0, 999);
    When(x => x.StatusCondition is not null, () => RuleFor(x => x.StatusCondition!.Value).IsInEnum());

    When(x => !string.IsNullOrWhiteSpace(x.Sprite?.Value), () => RuleFor(x => x.Sprite!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
  }
}
