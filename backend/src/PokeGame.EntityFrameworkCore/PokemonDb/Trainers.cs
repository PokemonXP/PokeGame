using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Trainers
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Trainers), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(TrainerEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(TrainerEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(TrainerEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(TrainerEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(TrainerEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(TrainerEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(TrainerEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(TrainerEntity.DisplayName), Table);
  public static readonly ColumnId Gender = new(nameof(TrainerEntity.Gender), Table);
  public static readonly ColumnId Id = new(nameof(TrainerEntity.Id), Table);
  public static readonly ColumnId License = new(nameof(TrainerEntity.License), Table);
  public static readonly ColumnId LicenseNormalized = new(nameof(TrainerEntity.LicenseNormalized), Table);
  public static readonly ColumnId Money = new(nameof(TrainerEntity.Money), Table);
  public static readonly ColumnId Notes = new(nameof(TrainerEntity.Notes), Table);
  public static readonly ColumnId Sprite = new(nameof(TrainerEntity.Sprite), Table);
  public static readonly ColumnId TrainerId = new(nameof(TrainerEntity.TrainerId), Table);
  public static readonly ColumnId UniqueName = new(nameof(TrainerEntity.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(TrainerEntity.UniqueNameNormalized), Table);
  public static readonly ColumnId Url = new(nameof(TrainerEntity.Url), Table);
  public static readonly ColumnId UserId = new(nameof(TrainerEntity.UserId), Table);
  public static readonly ColumnId UserUid = new(nameof(TrainerEntity.UserUid), Table);
}
