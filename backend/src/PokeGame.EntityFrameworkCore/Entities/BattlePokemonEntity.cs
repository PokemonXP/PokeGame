namespace PokeGame.EntityFrameworkCore.Entities;

internal class BattlePokemonEntity
{
  public BattleEntity? Battle { get; private set; }
  public int BattleId { get; private set; }
  public Guid BattleUid { get; private set; }

  public PokemonEntity? Pokemon { get; set; }
  public int PokemonId { get; private set; }
  public Guid PokemonUid { get; private set; }

  public BattlePokemonEntity(BattleEntity battle, PokemonEntity pokemon)
  {
    Battle = battle;
    BattleId = battle.BattleId;
    BattleUid = battle.Id;

    Pokemon = pokemon;
    PokemonId = pokemon.PokemonId;
    PokemonUid = pokemon.Id;
  }

  private BattlePokemonEntity()
  {
  }

  public override bool Equals(object? obj) => obj is BattlePokemonEntity entity && entity.BattleId == BattleId && entity.PokemonId == PokemonId;
  public override int GetHashCode() => HashCode.Combine(BattleId, PokemonId);
  public override string ToString() => $"{GetType()} (BattleId={BattleId}, PokemonId={PokemonId})";
}
