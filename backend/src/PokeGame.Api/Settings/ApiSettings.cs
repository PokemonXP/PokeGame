using Krakenar.Core;

namespace PokeGame.Api.Settings;

public class ApiSettings
{
  private const string SectionKey = "Api";

  public bool EnableSwagger { get; set; }
  public string Title { get; set; } = "PokéGame API";
  public Version Version { get; set; } = new Version(0, 1, 0);

  public static ApiSettings Initialize(IConfiguration configuration)
  {
    ApiSettings settings = configuration.GetSection(SectionKey).Get<ApiSettings>() ?? new();

    string? enableSwaggerValue = Environment.GetEnvironmentVariable("API_ENABLE_SWAGGER");
    if (!string.IsNullOrWhiteSpace(enableSwaggerValue) && bool.TryParse(enableSwaggerValue, out bool enableSwagger))
    {
      settings.EnableSwagger = enableSwagger;
    }

    settings.Title = EnvironmentHelper.GetString("API_TITLE", settings.Title);

    string? versionValue = Environment.GetEnvironmentVariable("API_VERSION");
    if (!string.IsNullOrWhiteSpace(versionValue) && Version.TryParse(versionValue, out Version? version))
    {
      settings.Version = version;
    }

    return settings;
  }
}
