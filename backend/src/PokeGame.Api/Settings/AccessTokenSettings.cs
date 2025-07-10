namespace PokeGame.Api.Settings;

internal record AccessTokenSettings
{
  public string Type { get; set; } = "at+jwt";
  public int LifetimeSeconds { get; set; } = 300;
}
