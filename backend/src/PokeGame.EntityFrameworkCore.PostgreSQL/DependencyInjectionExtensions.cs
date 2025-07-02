using Krakenar.Core;
using Krakenar.EntityFrameworkCore.PostgreSQL;
using Krakenar.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PokeGame.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeGameEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Pokemon", configuration.GetConnectionString("PostgreSQL"));
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The connection string for the database provider '{DatabaseProvider.EntityFrameworkCorePostgreSQL}' could not be found.", nameof(configuration));
    }
    return services.AddPokeGameEntityFrameworkCorePostgreSQL(connectionString.Trim());
  }
  public static IServiceCollection AddPokeGameEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services.AddKrakenarEntityFrameworkCorePostgreSQL(connectionString);
  }
}
