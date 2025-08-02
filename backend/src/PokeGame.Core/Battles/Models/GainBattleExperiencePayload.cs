namespace PokeGame.Core.Battles.Models;

public record GainBattleExperiencePayload
{
  public string Defeated { get; set; } = string.Empty;
  public List<VictoriousPokemonPayload> Victorious { get; set; } = [];
}
