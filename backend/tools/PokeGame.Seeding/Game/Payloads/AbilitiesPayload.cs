namespace PokeGame.Seeding.Game.Payloads;

internal record AbilitiesPayload
{
  public string Primary { get; set; } = string.Empty;
  public string? Secondary { get; set; }
  public string? Hidden { get; set; }
}
