using Krakenar.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameInfrastructure(this IServiceCollection services)
  {
    return services.AddKrakenarInfrastructure();
  }
}
