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

    RuleFor(x => x).Must(HaveValidProperties)
      .WithErrorCode("CreateOrReplaceItemValidator")
      .WithMessage(p => string.Concat(
        "Exactly one of the following must be provided: ",
        string.Join(", ", nameof(p.BattleItem), nameof(p.Berry), nameof(p.KeyItem), nameof(p.Material), nameof(p.Medicine), nameof(p.OtherItem), nameof(p.PokeBall), nameof(p.TechnicalMachine), nameof(p.Treasure)),
        '.'));
  }
  private static bool HaveValidProperties(CreateOrReplaceItemPayload payload)
  {
    int count = 0;
    if (payload.BattleItem is not null)
    {
      count++;
    }
    if (payload.Berry is not null)
    {
      count++;
    }
    if (payload.KeyItem is not null)
    {
      count++;
    }
    if (payload.Material is not null)
    {
      count++;
    }
    if (payload.Medicine is not null)
    {
      count++;
    }
    if (payload.OtherItem is not null)
    {
      count++;
    }
    if (payload.PokeBall is not null)
    {
      count++;
    }
    if (payload.TechnicalMachine is not null)
    {
      count++;
    }
    if (payload.Treasure is not null)
    {
      count++;
    }
    return count == 1;
  }

  public CreateOrReplaceItemValidator(ItemCategory category)
  {
    if (category == ItemCategory.BattleItem)
    {
      When(x => x.BattleItem is not null, () => RuleFor(x => x.BattleItem!).SetValidator(new BattleItemValidator()))
        .Otherwise(() => RuleFor(x => x.BattleItem).NotNull());
    }
    else
    {
      RuleFor(x => x.BattleItem).Null();
    }

    if (category == ItemCategory.Berry)
    {
      When(x => x.Berry is not null, () => RuleFor(x => x.Berry!).SetValidator(new BerryValidator()))
        .Otherwise(() => RuleFor(x => x.Berry).NotNull());
    }
    else
    {
      RuleFor(x => x.Berry).Null();
    }

    if (category == ItemCategory.KeyItem)
    {
      When(x => x.KeyItem is not null, () => RuleFor(x => x.KeyItem!).SetValidator(new KeyItemValidator()))
        .Otherwise(() => RuleFor(x => x.KeyItem).NotNull());
    }
    else
    {
      RuleFor(x => x.KeyItem).Null();
    }

    if (category == ItemCategory.Material)
    {
      When(x => x.Material is not null, () => RuleFor(x => x.Material!).SetValidator(new MaterialValidator()))
        .Otherwise(() => RuleFor(x => x.Material).NotNull());
    }
    else
    {
      RuleFor(x => x.Material).Null();
    }

    if (category == ItemCategory.Medicine)
    {
      When(x => x.Medicine is not null, () => RuleFor(x => x.Medicine!).SetValidator(new MedicineValidator()))
        .Otherwise(() => RuleFor(x => x.Medicine).NotNull());
    }
    else
    {
      RuleFor(x => x.Medicine).Null();
    }

    if (category == ItemCategory.OtherItem)
    {
      When(x => x.OtherItem is not null, () => RuleFor(x => x.OtherItem!).SetValidator(new OtherItemValidator()))
        .Otherwise(() => RuleFor(x => x.OtherItem).NotNull());
    }
    else
    {
      RuleFor(x => x.OtherItem).Null();
    }

    if (category == ItemCategory.PokeBall)
    {
      When(x => x.PokeBall is not null, () => RuleFor(x => x.PokeBall!).SetValidator(new PokeBallValidator()))
        .Otherwise(() => RuleFor(x => x.PokeBall).NotNull());
    }
    else
    {
      RuleFor(x => x.PokeBall).Null();
    }

    if (category == ItemCategory.TechnicalMachine)
    {
      When(x => x.TechnicalMachine is not null, () => RuleFor(x => x.TechnicalMachine!).SetValidator(new TechnicalMachineValidator()))
        .Otherwise(() => RuleFor(x => x.TechnicalMachine).NotNull());
    }
    else
    {
      RuleFor(x => x.TechnicalMachine).Null();
    }

    if (category == ItemCategory.Treasure)
    {
      When(x => x.Treasure is not null, () => RuleFor(x => x.Treasure!).SetValidator(new TreasureValidator()))
        .Otherwise(() => RuleFor(x => x.Treasure).NotNull());
    }
    else
    {
      RuleFor(x => x.Treasure).Null();
    }
  }
}
