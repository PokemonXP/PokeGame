using Krakenar.Core;
using Krakenar.Infrastructure.Commands;
using PokeGame.Api.Settings;

namespace PokeGame.Api;

internal static class Program
{
  internal static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();

    DatabaseSettings database = application.Services.GetRequiredService<DatabaseSettings>();
    if (database.ApplyMigrations)
    {
      MigrateDatabase migrateDatabase = new();
      ICommandHandler<MigrateDatabase> migrationHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<MigrateDatabase>>();
      await migrationHandler.HandleAsync(migrateDatabase);
    }

    application.Run();
  }
}
