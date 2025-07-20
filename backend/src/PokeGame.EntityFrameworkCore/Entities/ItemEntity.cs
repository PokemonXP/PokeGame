using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Events;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }
  public Guid Id { get; private set; }

  public ItemCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int Price { get; private set; }

  public string? Sprite { get; private set; }
  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public List<PokemonEntity> ContainedPokemon { get; private set; } = [];
  public List<PokemonEntity> HoldingPokemon { get; private set; } = [];
  public List<PokemonMoveEntity> LearnedMoves { get; private set; } = [];

  private ItemEntity() : base()
  {
  }

  public void SetUniqueName(ItemUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(ItemUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.Price is not null)
    {
      Price = @event.Price.Value?.Value ?? 0;
    }

    if (@event.Sprite is not null)
    {
      Sprite = @event.Sprite.Value?.Value;
    }
    if (@event.Url is not null)
    {
      Url = @event.Url.Value?.Value;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
