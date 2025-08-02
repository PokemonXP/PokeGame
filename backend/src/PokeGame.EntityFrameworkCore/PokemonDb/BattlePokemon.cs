using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class BattlePokemon
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.BattlePokemon), alias: null);

  public static readonly ColumnId Accuracy = new(nameof(BattlePokemonEntity.Accuracy), Table);
  public static readonly ColumnId Attack = new(nameof(BattlePokemonEntity.Attack), Table);
  public static readonly ColumnId BattleId = new(nameof(BattlePokemonEntity.BattleId), Table);
  public static readonly ColumnId BattleUid = new(nameof(BattlePokemonEntity.BattleUid), Table);
  public static readonly ColumnId Critical = new(nameof(BattlePokemonEntity.Critical), Table);
  public static readonly ColumnId Defense = new(nameof(BattlePokemonEntity.Defense), Table);
  public static readonly ColumnId Evasion = new(nameof(BattlePokemonEntity.Evasion), Table);
  public static readonly ColumnId IsActive = new(nameof(BattlePokemonEntity.IsActive), Table);
  public static readonly ColumnId PokemonId = new(nameof(BattlePokemonEntity.PokemonId), Table);
  public static readonly ColumnId PokemonUid = new(nameof(BattlePokemonEntity.PokemonUid), Table);
  public static readonly ColumnId SpecialAttack = new(nameof(BattlePokemonEntity.SpecialAttack), Table);
  public static readonly ColumnId SpecialDefense = new(nameof(BattlePokemonEntity.SpecialDefense), Table);
  public static readonly ColumnId Speed = new(nameof(BattlePokemonEntity.Speed), Table);
}
