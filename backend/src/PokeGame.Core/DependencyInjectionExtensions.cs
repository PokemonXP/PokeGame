using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeGame.Core.Abilities;
using PokeGame.Core.Forms;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;
using PokeGame.Core.Species;
using PokeGame.Core.Trainers;
using PokeGame.Core.Varieties;

namespace PokeGame.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameCore(this IServiceCollection services)
  {
    AbilityService.Register(services);
    FormService.Register(services);
    ItemService.Register(services);
    MoveService.Register(services);
    PokemonService.Register(services);
    RegionService.Register(services);
    SpeciesService.Register(services);
    TrainerService.Register(services);
    VarietyService.Register(services);

    return services.AddKrakenarCore();
  }
}
