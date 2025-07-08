using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Pokemons.Validators;

internal class CreatePokemonValidator : AbstractValidator<CreatePokemonPayload>
{
  public CreatePokemonValidator(IUniqueNameSettings uniqueNameSettings, FormModel form)
  {
    VarietyModel variety = form.Variety;

    RuleFor(x => x.Form).NotEmpty();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    switch (variety.GenderRatio)
    {
      case null:
        RuleFor(x => x.Gender).Null();
        break;
      case 0:
        When(x => x.Gender.HasValue, () => RuleFor(x => x.Gender).Equal(PokemonGender.Female));
        break;
      case 8:
        When(x => x.Gender.HasValue, () => RuleFor(x => x.Gender).Equal(PokemonGender.Male));
        break;
      default:
        RuleFor(x => x.Gender).IsInEnum();
        break;
    }

    RuleFor(x => x.TeraType).IsInEnum();
    RuleFor(x => x.AbilitySlot).IsInEnum();
    if (form.Abilities.Secondary is null)
    {
      RuleFor(x => x.AbilitySlot).NotEqual(AbilitySlot.Secondary);
    }
    if (form.Abilities.Hidden is null)
    {
      RuleFor(x => x.AbilitySlot).NotEqual(AbilitySlot.Hidden);
    }
    When(x => !string.IsNullOrWhiteSpace(x.Nature), () => RuleFor(x => x.Nature!).PokemonNature());

    When(x => x.IndividualValues is not null, () => RuleFor(x => x.IndividualValues!).SetValidator(new IndividualValuesValidator()));
    When(x => x.EffortValues is not null, () => RuleFor(x => x.EffortValues!).SetValidator(new EffortValuesValidator()));

    RuleFor(x => x.Experience).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Vitality).InclusiveBetween(0, 999);
    RuleFor(x => x.Stamina).InclusiveBetween(0, 999);

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
  }
}
