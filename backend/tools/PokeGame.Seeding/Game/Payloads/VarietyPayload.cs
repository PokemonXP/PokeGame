namespace PokeGame.Seeding.Game.Payloads;

internal class VarietyPayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Species { get; set; } = string.Empty;
  public bool IsDefault { get; set; }
  public bool CanChangeForm { get; set; }
  public int? GenderRatio { get; set; }

  public string Genus { get; set; } = string.Empty;
  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is VarietyPayload variety && variety.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";
}
