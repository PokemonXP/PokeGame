using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items.Validators;

internal class CreateOrReplaceItemValidator : AbstractValidator<CreateOrReplaceItemPayload>
{
  public CreateOrReplaceItemValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Price).GreaterThanOrEqualTo(0);

    When(x => x.BattleItem is not null, () => RuleFor(x => x.BattleItem!).SetValidator(new BattleItemValidator()));
    When(x => x.Berry is not null, () => RuleFor(x => x.Berry!).SetValidator(new BerryValidator()));
    When(x => x.KeyItem is not null, () => RuleFor(x => x.KeyItem!).SetValidator(new KeyItemValidator()));
    When(x => x.Material is not null, () => RuleFor(x => x.Material!).SetValidator(new MaterialValidator()));
    When(x => x.Medicine is not null, () => RuleFor(x => x.Medicine!).SetValidator(new MedicineValidator()));
    When(x => x.OtherItem is not null, () => RuleFor(x => x.OtherItem!).SetValidator(new OtherItemValidator()));
    When(x => x.PokeBall is not null, () => RuleFor(x => x.PokeBall!).SetValidator(new PokeBallValidator()));
    When(x => x.TechnicalMachine is not null, () => RuleFor(x => x.TechnicalMachine!).SetValidator(new TechnicalMachineValidator()));
    When(x => x.Treasure is not null, () => RuleFor(x => x.Treasure!).SetValidator(new TreasureValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Sprite), () => RuleFor(x => x.Sprite!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
