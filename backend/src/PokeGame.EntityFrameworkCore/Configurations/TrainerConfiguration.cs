using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeGame.Core.Trainers;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Configurations;

internal class TrainerConfiguration : AggregateConfiguration<TrainerEntity>, IEntityTypeConfiguration<TrainerEntity>
{
  public override void Configure(EntityTypeBuilder<TrainerEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Trainers.Table.Table!, PokemonDb.Trainers.Table.Schema);
    builder.HasKey(x => x.TrainerId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.License);
    builder.HasIndex(x => x.LicenseNormalized).IsUnique();
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Gender);
    builder.HasIndex(x => x.Money);
    builder.HasIndex(x => x.UserId);
    builder.HasIndex(x => x.UserUid);

    builder.Property(x => x.License).HasMaxLength(License.MaximumLength);
    builder.Property(x => x.LicenseNormalized).HasMaxLength(License.MaximumLength);
    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Gender).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<TrainerGender>());
    builder.Property(x => x.UserId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.Sprite).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
  }
}
