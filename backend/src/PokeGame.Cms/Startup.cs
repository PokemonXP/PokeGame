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

    services.AddControllers();
    services.AddOpenApi();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    Configure((WebApplication)builder);
  }
  public virtual void Configure(WebApplication application)
  {
    if (application.Environment.IsDevelopment())
    {
      application.MapOpenApi();
    }

    application.UseHttpsRedirection();
    application.UseAuthorization();

    application.MapControllers();
  }
}
