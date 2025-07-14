using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Pokemons.Validators;

internal class UpdatePokemonValidator : AbstractValidator<UpdatePokemonPayload>
{
  public UpdatePokemonValidator(IUniqueNameSettings uniqueNameSettings, FormModel? form = null)
  {
    if (form is null)
    {
      When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
      When(x => !string.IsNullOrWhiteSpace(x.Nickname?.Value), () => RuleFor(x => x.Nickname!.Value!).DisplayName());

      When(x => !string.IsNullOrWhiteSpace(x.Sprite?.Value), () => RuleFor(x => x.Sprite!.Value!).Url());
      When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());
    }
    else
    {
      VarietyModel variety = form.Variety;
      switch (variety.GenderRatio)
      {
        case null:
        case 0:
        case 8:
          RuleFor(x => x.Gender).Null();
          break;
        default:
          RuleFor(x => x.Gender).IsInEnum();
          break;
      }
    }
  }
}
