using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Items.Models;

namespace PokeGame.Core.Items.Validators;

internal class UpdateItemValidator : AbstractValidator<UpdateItemPayload>
{
  public UpdateItemValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

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

    When(x => !string.IsNullOrWhiteSpace(x.Sprite?.Value), () => RuleFor(x => x.Sprite!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
  }

  public UpdateItemValidator(ItemCategory category)
  {
    if (category == ItemCategory.BattleItem)
    {
      When(x => x.BattleItem is not null, () => RuleFor(x => x.BattleItem!).SetValidator(new BattleItemValidator()));
    }
    else
    {
      RuleFor(x => x.BattleItem).Null();
    }

    if (category == ItemCategory.Berry)
    {
      When(x => x.Berry is not null, () => RuleFor(x => x.Berry!).SetValidator(new BerryValidator()));
    }
    else
    {
      RuleFor(x => x.Berry).Null();
    }

    if (category == ItemCategory.KeyItem)
    {
      When(x => x.KeyItem is not null, () => RuleFor(x => x.KeyItem!).SetValidator(new KeyItemValidator()));
    }
    else
    {
      RuleFor(x => x.KeyItem).Null();
    }

    if (category == ItemCategory.Material)
    {
      When(x => x.Material is not null, () => RuleFor(x => x.Material!).SetValidator(new MaterialValidator()));
    }
    else
    {
      RuleFor(x => x.Material).Null();
    }

    if (category == ItemCategory.Medicine)
    {
      When(x => x.Medicine is not null, () => RuleFor(x => x.Medicine!).SetValidator(new MedicineValidator()));
    }
    else
    {
      RuleFor(x => x.Medicine).Null();
    }

    if (category == ItemCategory.OtherItem)
    {
      When(x => x.OtherItem is not null, () => RuleFor(x => x.OtherItem!).SetValidator(new OtherItemValidator()));
    }
    else
    {
      RuleFor(x => x.OtherItem).Null();
    }

    if (category == ItemCategory.PokeBall)
    {
      When(x => x.PokeBall is not null, () => RuleFor(x => x.PokeBall!).SetValidator(new PokeBallValidator()));
    }
    else
    {
      RuleFor(x => x.PokeBall).Null();
    }

    if (category == ItemCategory.TechnicalMachine)
    {
      When(x => x.TechnicalMachine is not null, () => RuleFor(x => x.TechnicalMachine!).SetValidator(new TechnicalMachineValidator()));
    }
    else
    {
      RuleFor(x => x.TechnicalMachine).Null();
    }

    if (category == ItemCategory.Treasure)
    {
      When(x => x.Treasure is not null, () => RuleFor(x => x.Treasure!).SetValidator(new TreasureValidator()));
    }
    else
    {
      RuleFor(x => x.Treasure).Null();
    }
  }
}
