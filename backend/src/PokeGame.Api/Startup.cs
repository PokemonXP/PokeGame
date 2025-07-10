using Krakenar.Client;
using Krakenar.Contracts.Constants;
using Krakenar.Core;
using Krakenar.Infrastructure;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using PokeGame.Api.Authentication;
using PokeGame.Api.Authorization;
using PokeGame.Api.Constants;
using PokeGame.Api.Extensions;
using PokeGame.Api.Settings;
using PokeGame.Core;
using PokeGame.EntityFrameworkCore;
using PokeGame.EntityFrameworkCore.PostgreSQL;
using PokeGame.Infrastructure;
using PokeGame.MongoDB;

namespace PokeGame.Api;

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

    services.AddControllers().AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

    services.AddSingleton(CorsSettings.Initialize(_configuration));
    services.AddCors();

    AuthenticationSettings authenticationSettings = AuthenticationSettings.Initialize(_configuration);
    services.AddSingleton(authenticationSettings);
    services.AddSingleton(GoogleSettings.Initialize(_configuration));
    string[] authenticationSchemes = GetAuthenticationSchemes(authenticationSettings);
    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSettings.EnableBasic)
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }
    services.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build())
      .AddPolicy(Policies.IsAdmin, new AuthorizationPolicyBuilder(authenticationSchemes)
        .RequireAuthenticatedUser()
        .AddRequirements(new PokeGameAdminRequirement())
        .Build());
    services.AddSingleton<IAuthorizationHandler, PokeGameAdminHandler>();

    CookiesSettings cookiesSettings = CookiesSettings.Initialize(_configuration);
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    services.AddSingleton(ErrorSettings.Initialize(_configuration));
    services.AddExceptionHandler<ExceptionHandler>();
    services.AddProblemDetails();

    ApiSettings apiSettings = ApiSettings.Initialize(_configuration);
    services.AddSingleton(apiSettings);
    if (apiSettings.EnableSwagger)
    {
      services.AddPokeGameApiSwagger(apiSettings);
    }

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddPokeGameCore();
    services.AddPokeGameInfrastructure();

    DatabaseSettings databaseSettings = DatabaseSettings.Initialize(_configuration);
    services.AddSingleton(databaseSettings);
    services.AddPokeGameEntityFrameworkCore();
    switch (databaseSettings.Provider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddPokeGameEntityFrameworkCorePostgreSQL(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<PokemonContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseSettings.Provider);
    }
    services.AddPokeGameMongoDB(_configuration);
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();

    services.AddKrakenarClient(_configuration);
  }
  private static string[] GetAuthenticationSchemes(AuthenticationSettings settings)
  {
    List<string> schemes = new(capacity: 4)
    {
      Schemes.ApiKey,
      Schemes.Bearer,
      Schemes.Session
    };

    if (settings.EnableBasic)
    {
      schemes.Add(Schemes.Basic);
    }

    return [.. schemes];
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      Configure(application);
    }
  }
  public virtual void Configure(WebApplication application)
  {
    ApiSettings apiSettings = application.Services.GetRequiredService<ApiSettings>();
    if (apiSettings.EnableSwagger)
    {
      application.UsePokeGameApiSwagger(apiSettings);
    }

    application.UseHttpsRedirection();
    application.UseCors();
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<RenewSession>();
    application.UseAuthentication();
    application.UseAuthorization();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
