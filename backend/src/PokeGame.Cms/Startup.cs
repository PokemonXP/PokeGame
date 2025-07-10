using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure;
using Krakenar.Web;
using Krakenar.Web.Middlewares;
using Krakenar.Web.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using PokeGame.Cms.Extensions;
using PokeGame.Cms.Settings;
using PokeGame.Core;
using PokeGame.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.PostgreSQL;
using PokeGame.Infrastructure;
using PokeGame.MongoDB;

namespace PokeGame.Cms;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddApplicationInsightsTelemetry();

    services.AddPokeGameCore();
    services.AddPokeGameInfrastructure();
    services.AddKrakenarWeb(_configuration);

    AdminSettings? adminSettings = services.Where(x => x.ServiceType.Equals(typeof(AdminSettings)) && x.ImplementationInstance is AdminSettings)
      .FirstOrDefault()?.ImplementationInstance as AdminSettings
      ?? throw new InvalidOperationException($"The {nameof(AdminSettings)} service has not been regiseterd.");
    if (adminSettings.EnableSwagger)
    {
      services.AddKrakenarSwagger(adminSettings);
    }

    IHealthChecksBuilder healthChecks = services.AddHealthChecks();
    DatabaseSettings databaseSettings = DatabaseSettings.Initialize(_configuration);
    services.AddSingleton(databaseSettings);
    services.AddPokeGameEntityFrameworkCore();
    switch (databaseSettings.Provider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddPokeGameEntityFrameworkCorePostgreSQL(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<KrakenarContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseSettings.Provider);
    }
    services.AddPokeGameMongoDB(_configuration);
  }

  public override void Configure(IApplicationBuilder builder)
  {
    Configure((WebApplication)builder);
  }
  public virtual void Configure(WebApplication application)
  {
    AdminSettings adminSettings = application.Services.GetRequiredService<AdminSettings>();
    if (adminSettings.EnableSwagger)
    {
      application.UseKrakenarSwagger(adminSettings);
    }

    application.UseHttpsRedirection();
    application.UseCors();
    application.UseStaticFiles();
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<Logging>();
    application.UseMiddleware<RenewSession>();
    application.UseMiddleware<RedirectNotFound>();
    application.UseAuthentication();
    application.UseAuthorization();
    application.UseMiddleware<ResolveRealm>();
    application.UseMiddleware<ResolveUser>();

    application.MapControllers();
    application.MapControllerRoute(name: "Admin", pattern: $"{adminSettings.BasePath}/{{**anything}}", defaults: new { Controller = "Admin", Action = "Index" });
    application.MapHealthChecks("/health");
  }
}
