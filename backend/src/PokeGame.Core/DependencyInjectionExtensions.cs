using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Items;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameCore(this IServiceCollection services)
  {
    return services
      .AddCommands()
      .AddCoreServices()
      .AddKrakenarCore()
      .AddManagers();
  }

  private static IServiceCollection AddCommands(this IServiceCollection services)
  {
    return services.AddTransient<ICommandHandler<CreatePokemon, PokemonModel>, CreatePokemonHandler>();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    return services.AddTransient<IPokemonService, PokemonService>();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IItemManager, ItemManager>()
      .AddTransient<IPokemonManager, PokemonManager>();
  }
}
