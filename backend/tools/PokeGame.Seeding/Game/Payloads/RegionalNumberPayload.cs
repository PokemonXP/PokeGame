namespace PokeGame.Seeding.Game.Payloads;

internal record RegionalNumberPayload
{
  public string Region { get; set; } = string.Empty;
  public int Number { get; set; }
}
