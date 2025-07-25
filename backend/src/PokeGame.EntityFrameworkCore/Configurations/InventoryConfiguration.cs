using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class InventoryConfiguration : IEntityTypeConfiguration<InventoryEntity>
{
  public void Configure(EntityTypeBuilder<InventoryEntity> builder)
  {
    builder.ToTable(PokemonDb.Inventory.Table.Table!, PokemonDb.Inventory.Table.Schema);
    builder.HasKey(x => new { x.TrainerId, x.ItemId });

    builder.HasIndex(x => x.TrainerUid);
    builder.HasIndex(x => x.ItemUid);
    builder.HasIndex(x => new { x.TrainerUid, x.ItemUid }).IsUnique();
    builder.HasIndex(x => x.Quantity);

    builder.HasOne(x => x.Trainer).WithMany(x => x.Inventory).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Item).WithMany(x => x.Inventory).OnDelete(DeleteBehavior.Restrict);
  }
}
