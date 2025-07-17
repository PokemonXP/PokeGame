using Krakenar.Core;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure.Commands;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Events;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;
using PokeGame.EntityFrameworkCore.Handlers;
using PokeGame.EntityFrameworkCore.Queriers;
using PokeGame.EntityFrameworkCore.Repositories;

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
      .AddRepositories()
      .AddScoped<ICommandHandler<MigrateDatabase>, MigratePokemonDatabaseCommandHandler>();
  }

  private static IServiceCollection AddEventHandlers(this IServiceCollection services)
  {
    AbilityEvents.Register(services);
    RegionEvents.Register(services);
    TrainerEvents.Register(services);

    return services
      .AddScoped<IEventHandler<ContentLocalePublished>, PokemonContentEvents>()
      .AddScoped<IEventHandler<ContentLocaleUnpublished>, PokemonContentEvents>()
      .AddScoped<IEventHandler<PokemonCaught>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonCreated>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonDeleted>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonItemHeld>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonItemRemoved>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonMoveLearned>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonMoveRelearned>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonMoveMastered>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonMovesSwitched>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonNicknamed>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonReceived>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonReleased>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonTechnicalMachineUsed>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonUniqueNameChanged>, PokemonEvents>()
      .AddScoped<IEventHandler<PokemonUpdated>, PokemonEvents>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityQuerier, AbilityQuerier>()
      .AddScoped<IFormQuerier, FormQuerier>()
      .AddScoped<IItemQuerier, ItemQuerier>()
      .AddScoped<IMoveQuerier, MoveQuerier>()
      .AddScoped<IPokemonQuerier, PokemonQuerier>()
      .AddScoped<IRegionQuerier, RegionQuerier>()
      .AddScoped<ISpeciesQuerier, SpeciesQuerier>()
      .AddScoped<ITrainerQuerier, TrainerQuerier>()
      .AddScoped<IVarietyQuerier, VarietyQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityRepository, AbilityRepository>()
      .AddScoped<IPokemonRepository, PokemonRepository>()
      .AddScoped<IRegionRepository, RegionRepository>()
      .AddScoped<ITrainerRepository, TrainerRepository>();
  }
}
