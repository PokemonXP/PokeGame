﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core.Abilities;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class FormAbilityConfiguration : IEntityTypeConfiguration<FormAbilityEntity> // TODO(fpion): next migration
{
  public void Configure(EntityTypeBuilder<FormAbilityEntity> builder)
  {
    builder.ToTable(PokemonDb.FormAbilities.Table.Table!, PokemonDb.FormAbilities.Table.Schema);
    builder.HasKey(x => new { x.FormId, x.AbilityId });

    builder.HasIndex(x => new { x.FormId, x.Slot }).IsUnique();
    builder.HasIndex(x => x.FormUid);
    builder.HasIndex(x => x.AbilityUid);
    //builder.HasIndex(x => new { x.FormUid, x.AbilityUid }).IsUnique();
    //builder.HasIndex(x => new { x.FormUid, x.Slot }).IsUnique();

    builder.Property(x => x.Slot).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<AbilitySlot>());

    builder.HasOne(x => x.Form).WithMany(x => x.Abilities).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Ability).WithMany(x => x.Forms).OnDelete(DeleteBehavior.Cascade);
  }
}
