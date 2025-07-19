namespace PokeGame.Core.Pokemon.Models;

public record PowerPointsModel
{
  public byte Current { get; set; }
  public byte Maximum { get; set; }
  public byte Reference { get; set; }

  public PowerPointsModel()
  {
  }

  public PowerPointsModel(byte current, byte maximum, byte reference)
  {
    Current = current;
    Maximum = maximum;
    Reference = reference;
  }
}
