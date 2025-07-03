using PokeGame.Core;

namespace PokeGame.Seeding.Game.Payloads;

internal record TypesPayload
{
  public PokemonType Primary { get; set; }
  public PokemonType? Secondary { get; set; }
}
