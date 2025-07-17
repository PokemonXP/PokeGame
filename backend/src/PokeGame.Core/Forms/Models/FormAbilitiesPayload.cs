namespace PokeGame.Core.Forms.Models;

public record FormAbilitiesPayload
{
  public string Primary { get; set; }
  public string? Secondary { get; set; }
  public string? Hidden { get; set; }

  public FormAbilitiesPayload() : this(string.Empty)
  {
  }

  public FormAbilitiesPayload(string primary, string? secondary = null, string? hidden = null)
  {
    Primary = primary;
    Secondary = secondary;
    Hidden = hidden;
  }
}
