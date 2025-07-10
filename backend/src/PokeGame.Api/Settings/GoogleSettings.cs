using Krakenar.Core;

namespace PokeGame.Api.Settings;

public record GoogleSettings
{
  private const string SectionKey = "Google";

  public string IdentifierKey { get; set; } = "Google";
  public string ClientId { get; set; } = string.Empty;

  public static GoogleSettings Initialize(IConfiguration configuration)
  {
    GoogleSettings settings = configuration.GetSection(SectionKey).Get<GoogleSettings>() ?? new();

    settings.IdentifierKey = EnvironmentHelper.GetString("GOOGLE_IDENTIFIER_KEY", settings.IdentifierKey);
    settings.ClientId = EnvironmentHelper.GetString("GOOGLE_CLIENT_ID", settings.ClientId);

    return settings;
  }
}
