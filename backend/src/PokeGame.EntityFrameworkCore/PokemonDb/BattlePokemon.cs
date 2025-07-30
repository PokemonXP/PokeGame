using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class BattlePokemon
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.BattlePokemon), alias: null);

  public static readonly ColumnId BattleId = new(nameof(BattlePokemonEntity.BattleId), Table);
  public static readonly ColumnId BattleUid = new(nameof(BattlePokemonEntity.BattleUid), Table);
  public static readonly ColumnId PokemonId = new(nameof(BattlePokemonEntity.PokemonId), Table);
  public static readonly ColumnId PokemonUid = new(nameof(BattlePokemonEntity.PokemonUid), Table);
}
