using Krakenar.EntityFrameworkCore.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.EntityFrameworkCore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameEntityFrameworkCore(this IServiceCollection services)
  {
    return services.AddKrakenarEntityFrameworkCoreRelational();
  }
}
