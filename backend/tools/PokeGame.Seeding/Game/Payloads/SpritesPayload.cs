namespace PokeGame.Seeding.Game.Payloads;

internal record SpritesPayload
{
  public string Default { get; set; } = string.Empty;
  public string DefaultShiny { get; set; } = string.Empty;
  public string? Alternative { get; set; }
  public string? AlternativeShiny { get; set; }
}
