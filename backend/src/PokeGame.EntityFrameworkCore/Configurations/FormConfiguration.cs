using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class FormConfiguration : AggregateConfiguration<FormEntity>, IEntityTypeConfiguration<FormEntity>
{
  public override void Configure(EntityTypeBuilder<FormEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Forms.Table.Table!, PokemonDb.Forms.Table.Schema);
    builder.HasKey(x => x.FormId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.VarietyUid);
    builder.HasIndex(x => new { x.VarietyId, x.IsDefault });
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Height);
    builder.HasIndex(x => x.Weight);
    builder.HasIndex(x => x.ExperienceYield);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.PrimaryType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonType>());
    builder.Property(x => x.SecondaryType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonType>());
    builder.Property(x => x.DefaultSprite).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.DefaultSpriteShiny).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.AlternativeSprite).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.AlternativeSpriteShiny).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Variety).WithMany(x => x.Forms).OnDelete(DeleteBehavior.Restrict);
  }
}
