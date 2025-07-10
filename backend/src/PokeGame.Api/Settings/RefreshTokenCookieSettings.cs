namespace PokeGame.Api.Settings;

public record RefreshTokenCookieSettings
{
  public bool HttpOnly { get; set; } = true;
  public TimeSpan? MaxAge { get; set; } = TimeSpan.FromDays(7);
  public SameSiteMode SameSite { get; set; } = SameSiteMode.None;
  public bool Secure { get; set; } = true;
}
