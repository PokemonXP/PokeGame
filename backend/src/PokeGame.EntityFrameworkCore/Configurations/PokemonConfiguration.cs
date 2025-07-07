using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core;
using PokeGame.Core.Pokemons;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class PokemonConfiguration : AggregateConfiguration<PokemonEntity>, IEntityTypeConfiguration<PokemonEntity>
{
  public override void Configure(EntityTypeBuilder<PokemonEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Pokemons.Table.Table!, PokemonDb.Pokemons.Table.Schema);
    builder.HasKey(x => x.PokemonId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.SpeciesUid);
    builder.HasIndex(x => x.VarietyUid);
    builder.HasIndex(x => x.FormUid);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.Nickname);
    builder.HasIndex(x => x.Gender);
    builder.HasIndex(x => x.TeraType);
    builder.HasIndex(x => x.Nature);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.Nickname).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Gender).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonGender>());
    builder.Property(x => x.TeraType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonType>());
    builder.Property(x => x.AbilitySlot).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<AbilitySlot>());
    builder.Property(x => x.Nature).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<PokemonNature>());
    builder.Property(x => x.Sprite).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.Species).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Variety).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Form).WithMany(x => x.Pokemon).OnDelete(DeleteBehavior.Restrict);
  }
}
