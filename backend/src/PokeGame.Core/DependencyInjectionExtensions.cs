using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameCore(this IServiceCollection services)
  {
    AbilityService.Register(services);
    MoveService.Register(services);
    PokemonService.Register(services);
    RegionService.Register(services);
    TrainerService.Register(services);

    return services.AddKrakenarCore();
  }
}
