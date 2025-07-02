using Krakenar.Core;
using Krakenar.Infrastructure;
using PokeGame.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.PostgreSQL;

namespace PokeGame.Seeding;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<SeedingWorker>();
    services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

    services.AddKrakenarCore();
    services.AddKrakenarInfrastructure();
    services.AddPokeGameEntityFrameworkCore();
    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCorePostgreSQL;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddPokeGameEntityFrameworkCorePostgreSQL(_configuration);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
    services.AddSingleton<IApplicationContext, SeedingApplicationContext>();
  }
}
