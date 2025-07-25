using FluentValidation;
using PokeGame.Core.Inventory.Models;

namespace PokeGame.Core.Inventory.Validators;

internal class InventoryQuantityValidator : AbstractValidator<InventoryQuantityPayload>
{
  public InventoryQuantityValidator()
  {
    RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
  }
}
