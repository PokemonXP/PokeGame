using Microsoft.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore;

public class PokemonContext : DbContext
{
  internal const string Schema = "Pokemon";

  public PokemonContext(DbContextOptions<PokemonContext> options) : base(options)
  {
  }

  internal DbSet<AbilityEntity> Abilities => Set<AbilityEntity>();
  internal DbSet<MoveEntity> Moves => Set<MoveEntity>();
  internal DbSet<RegionalNumberEntity> RegionalNumbers => Set<RegionalNumberEntity>();
  internal DbSet<RegionEntity> Regions => Set<RegionEntity>();
  internal DbSet<SpeciesEntity> Species => Set<SpeciesEntity>();
  internal DbSet<VarietyEntity> Varieties => Set<VarietyEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
