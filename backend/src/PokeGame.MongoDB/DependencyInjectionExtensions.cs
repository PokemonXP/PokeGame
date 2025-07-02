using Krakenar.Core;
using Krakenar.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("MONGOCONNSTR_Pokemon", configuration.GetConnectionString("MongoDB"));
    MongoDBSettings settings = MongoDBSettings.Initialize(configuration);
    return services.AddPokeGameMongoDB(connectionString, settings);
  }
  public static IServiceCollection AddPokeGameMongoDB(this IServiceCollection services, string? connectionString, MongoDBSettings settings)
  {
    return services.AddKrakenarMongoDB(connectionString, settings);
  }
}
