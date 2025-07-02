using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameCore(this IServiceCollection services)
  {
    return services.AddKrakenarCore();
  }
}
