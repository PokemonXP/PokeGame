using FluentValidation;
using PokeGame.Seeding.Game.Payloads;

namespace PokeGame.Seeding.Game.Validators;

internal class TypesValidator : AbstractValidator<TypesPayload>
{
  public TypesValidator()
  {
    RuleFor(x => x.Primary).IsInEnum();
    RuleFor(x => x.Secondary).IsInEnum().NotEqual(x => x.Primary);
  }
}
