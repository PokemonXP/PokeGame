using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core.Items;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class ItemConfiguration : AggregateConfiguration<ItemEntity>, IEntityTypeConfiguration<ItemEntity>
{
  public override void Configure(EntityTypeBuilder<ItemEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Items.Table.Table!, PokemonDb.Items.Table.Schema);
    builder.HasKey(x => x.ItemId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Price);
    builder.HasIndex(x => x.Category);
    builder.HasIndex(x => x.MoveUid);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Category).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<ItemCategory>());
    builder.Property(x => x.BattleItem).HasMaxLength(Constants.BattleItemMaximumLength);
    builder.Property(x => x.Sprite).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Move).WithMany(x => x.TechnicalMachines).OnDelete(DeleteBehavior.Restrict);
  }
}
