using Krakenar.Infrastructure;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PokeGame.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameInfrastructure(this IServiceCollection services)
  {
    return services
      .AddKrakenarInfrastructure()
      .RemoveAll<IEventSerializer>()
      .AddSingleton<IEventSerializer, EventSerializer>();
  }
}
