using System.Text;

namespace PokeGame.ETL.Settings;

internal record ExtractionSettings
{
  private const string SectionKey = "Extraction";

  public string Path { get; set; } = string.Empty;
  public Encoding Encoding { get; set; } = Encoding.Default;

  public string Language { get; set; } = string.Empty;
  public string Version { get; set; } = string.Empty;

  public static ExtractionSettings Initialize(IConfiguration configuration)
  {
    ExtractionSettings settings = configuration.GetSection(SectionKey).Get<ExtractionSettings>() ?? new();
    return settings;
  }
}
