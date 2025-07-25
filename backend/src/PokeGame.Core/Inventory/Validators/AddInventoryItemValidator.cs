using FluentValidation;
using PokeGame.Core.Inventory.Models;

namespace PokeGame.Core.Inventory.Validators;

internal class AddInventoryItemValidator : AbstractValidator<AddInventoryItemPayload>
{
  public AddInventoryItemValidator()
  {
    RuleFor(x => x.Quantity).GreaterThan(0);
  }
}
