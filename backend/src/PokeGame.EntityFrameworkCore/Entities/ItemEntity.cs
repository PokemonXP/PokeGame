using Krakenar.Core.Contents;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using PokeGame.Core.Items;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.Infrastructure.Data;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeGame.EntityFrameworkCore.Entities;

internal class ItemEntity : AggregateEntity
{
  public int ItemId { get; private set; }
  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int Price { get; private set; }

  public ItemCategory Category { get; private set; }

  public string? Sprite { get; private set; }

  public string? Url { get; private set; }
  public string? Notes { get; private set; }

  public ItemEntity(ItemPublished published) : base(published.Event)
  {
    Id = new ContentId(published.Event.StreamId).EntityId;

    Update(published);
  }

  private ItemEntity() : base()
  {
  }

  public void Update(ItemPublished published)
  {
    ContentLocale invariant = published.Invariant;
    ContentLocale locale = published.Locale;

    Update(published.Event);

    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    Price = (int)invariant.GetNumberValue(Items.Price);

    Category = ParseCategory(invariant.TryGetSelectValue(Items.Category)?.Single());

    Sprite = invariant.TryGetStringValue(Items.Sprite);

    Url = locale.TryGetStringValue(Items.Url);
    Notes = locale.TryGetStringValue(Items.Notes);
  }
  private static ItemCategory ParseCategory(string? value) => value switch
  {
    "tm-material" => ItemCategory.TechnicalMachineMaterial,
    "treasure" => ItemCategory.Treasure,
    "picnic" => ItemCategory.PicnicItem,
    "key" => ItemCategory.KeyItem,
    _ => ItemCategory.OtherItem,
  };

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
