using Krakenar.Contracts;

namespace PokeGame.Core.Moves.Models;

public record UpdateMovePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public Change<byte?>? Accuracy { get; set; }
  public Change<byte?>? Power { get; set; }
  public byte? PowerPoints { get; set; }

  public Change<string>? Url { get; set; }
  public Change<string>? Notes { get; set; }
}
