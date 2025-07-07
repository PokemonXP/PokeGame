using Krakenar.Core;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure.Commands;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Moves;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.EntityFrameworkCore.Queriers;

namespace PokeGame.EntityFrameworkCore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameEntityFrameworkCore(this IServiceCollection services)
  {
    return services
      .AddEventHandlers()
      .AddKrakenarEntityFrameworkCoreRelational()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddScoped<ICommandHandler<MigrateDatabase>, MigratePokemonDatabaseCommandHandler>();
  }

  private static IServiceCollection AddEventHandlers(this IServiceCollection services)
  {
    return services
      .AddScoped<IEventHandler<ContentLocalePublished>, PokemonContentEvents>()
      .AddScoped<IEventHandler<ContentLocaleUnpublished>, PokemonContentEvents>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityQuerier, AbilityQuerier>()
      .AddScoped<IFormQuerier, FormQuerier>()
      .AddScoped<IMoveQuerier, MoveQuerier>()
      .AddScoped<IRegionQuerier, RegionQuerier>()
      .AddScoped<ISpeciesQuerier, SpeciesQuerier>()
      .AddScoped<ITrainerQuerier, TrainerQuerier>()
      .AddScoped<IVarietyQuerier, VarietyQuerier>();
  }
}
