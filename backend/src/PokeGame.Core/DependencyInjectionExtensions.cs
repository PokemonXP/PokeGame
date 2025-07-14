using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Queries;

namespace PokeGame.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameCore(this IServiceCollection services)
  {
    return services
      .AddCommands()
      .AddCoreServices()
      .AddKrakenarCore()
      .AddManagers()
      .AddQueries();
  }

  private static IServiceCollection AddCommands(this IServiceCollection services)
  {
    return services
      .AddTransient<ICommandHandler<CreatePokemon, PokemonModel>, CreatePokemonHandler>()
      .AddTransient<ICommandHandler<DeletePokemon, PokemonModel?>, DeletePokemonHandler>()
      .AddTransient<ICommandHandler<RelearnPokemonMove, PokemonModel?>, RelearnPokemonMoveHandler>()
      .AddTransient<ICommandHandler<SwitchPokemonMoves, PokemonModel?>, SwitchPokemonMovesHandler>()
      .AddTransient<ICommandHandler<UpdatePokemon, PokemonModel?>, UpdatePokemonHandler>();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    return services.AddTransient<IPokemonService, PokemonService>();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IItemManager, ItemManager>()
      .AddTransient<IMoveManager, MoveManager>()
      .AddTransient<IPokemonManager, PokemonManager>();
  }

  private static IServiceCollection AddQueries(this IServiceCollection services)
  {
    return services
      .AddTransient<IQueryHandler<ReadPokemon, PokemonModel?>, ReadPokemonHandler>()
      .AddTransient<IQueryHandler<SearchPokemon, SearchResults<PokemonModel>>, SearchPokemonHandler>();
  }
}
