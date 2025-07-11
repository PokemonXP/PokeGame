namespace PokeGame.Api.Settings;

public record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.None;
}
