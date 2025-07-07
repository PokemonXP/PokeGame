using Krakenar.Core;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure.Commands;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities;
using PokeGame.Core.Regions;
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
      .AddScoped<IRegionQuerier, RegionQuerier>();
  }
}
