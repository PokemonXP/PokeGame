using Logitar.Data;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.PokemonDb;

internal static class Evolutions
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Evolutions), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(EvolutionEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(EvolutionEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(EvolutionEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(EvolutionEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(EvolutionEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(EvolutionEntity.Version), Table);

  public static readonly ColumnId Id = new(nameof(EvolutionEntity.Id), Table);
  public static readonly ColumnId EvolutionId = new(nameof(EvolutionEntity.EvolutionId), Table);
  // TODO(fpion): complete
}
