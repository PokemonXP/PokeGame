namespace PokeGame.Seeding.Game.Payloads;

internal class FormPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Variety { get; set; } = string.Empty;
  public bool IsDefault { get; set; }
  public bool IsBattleOnly { get; set; }
  public bool IsMega { get; set; }

  public int Height { get; set; }
  public int Weight { get; set; }

  public TypesPayload Types { get; set; } = new();
  public AbilitiesPayload Abilities { get; set; } = new();
  public BaseStatisticsPayload BaseStatistics { get; set; } = new();
  public YieldPayload Yield { get; set; } = new();
  public SpritesPayload Sprites { get; set; } = new();

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is FormPayload form && form.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";
}
