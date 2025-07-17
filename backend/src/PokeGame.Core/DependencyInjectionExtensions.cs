using Krakenar.Contracts.Search;
using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities;
using PokeGame.Core.Abilities.Commands;
using PokeGame.Core.Abilities.Models;
using PokeGame.Core.Abilities.Queries;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Commands;
using PokeGame.Core.Pokemons.Models;
using PokeGame.Core.Pokemons.Queries;
using PokeGame.Core.Regions;
using PokeGame.Core.Regions.Commands;
using PokeGame.Core.Regions.Models;
using PokeGame.Core.Regions.Queries;

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
      .AddTransient<ICommandHandler<CreateOrReplaceAbility, CreateOrReplaceAbilityResult>, CreateOrReplaceAbilityHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRegion, CreateOrReplaceRegionResult>, CreateOrReplaceRegionHandler>()
      .AddTransient<ICommandHandler<CreatePokemon, PokemonModel>, CreatePokemonHandler>()
      .AddTransient<ICommandHandler<DeleteAbility, AbilityModel?>, DeleteAbilityHandler>()
      .AddTransient<ICommandHandler<DeletePokemon, PokemonModel?>, DeletePokemonHandler>()
      .AddTransient<ICommandHandler<DeleteRegion, RegionModel?>, DeleteRegionHandler>()
      .AddTransient<ICommandHandler<RelearnPokemonMove, PokemonModel?>, RelearnPokemonMoveHandler>()
      .AddTransient<ICommandHandler<SwitchPokemonMoves, PokemonModel?>, SwitchPokemonMovesHandler>()
      .AddTransient<ICommandHandler<UpdatePokemon, PokemonModel?>, UpdatePokemonHandler>()
      .AddTransient<ICommandHandler<UpdateRegion, RegionModel?>, UpdateRegionHandler>();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IAbilityService, AbilityService>()
      .AddTransient<IPokemonService, PokemonService>()
      .AddTransient<IRegionService, RegionService>();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IAbilityManager, AbilityManager>()
      .AddTransient<IItemManager, ItemManager>()
      .AddTransient<IMoveManager, MoveManager>()
      .AddTransient<IPokemonManager, PokemonManager>()
      .AddTransient<IRegionManager, RegionManager>();
  }

  private static IServiceCollection AddQueries(this IServiceCollection services)
  {
    return services
      .AddTransient<IQueryHandler<ReadAbility, AbilityModel?>, ReadAbilityHandler>()
      .AddTransient<IQueryHandler<ReadPokemon, PokemonModel?>, ReadPokemonHandler>()
      .AddTransient<IQueryHandler<ReadRegion, RegionModel?>, ReadRegionHandler>()
      .AddTransient<IQueryHandler<SearchAbilities, SearchResults<AbilityModel>>, SearchAbilitiesHandler>()
      .AddTransient<IQueryHandler<SearchPokemon, SearchResults<PokemonModel>>, SearchPokemonHandler>()
      .AddTransient<IQueryHandler<SearchRegions, SearchResults<RegionModel>>, SearchRegionsHandler>();
  }
}
