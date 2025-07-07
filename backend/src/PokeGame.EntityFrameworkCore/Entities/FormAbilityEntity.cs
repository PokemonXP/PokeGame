using PokeGame.Core;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class FormAbilityEntity
{
  public int FormAbilityId { get; private set; }

  public FormEntity? Form { get; private set; }
  public int FormId { get; private set; }
  public Guid FormUid { get; private set; }

  public AbilityEntity? Ability { get; private set; }
  public int AbilityId { get; private set; }
  public Guid AbilityUid { get; private set; }

  public AbilitySlot Slot { get; private set; }

  public FormAbilityEntity(FormEntity form, AbilityEntity ability, AbilitySlot slot)
  {
    Form = form;
    FormId = form.FormId;
    FormUid = form.Id;

    SetAbility(ability);

    Slot = slot;
  }

  private FormAbilityEntity()
  {
  }

  public void SetAbility(AbilityEntity ability)
  {
    Ability = ability;
    AbilityId = ability.AbilityId;
    AbilityUid = ability.Id;
  }

  public override bool Equals(object? obj) => obj is FormAbilityEntity entity && entity.FormAbilityId == FormAbilityId;
  public override int GetHashCode() => FormAbilityId.GetHashCode();
  public override string ToString() => $"{GetType()} (FormAbilityId={FormAbilityId})";
}
