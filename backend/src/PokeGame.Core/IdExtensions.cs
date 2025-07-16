using Krakenar.Core.Realms;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core;

internal static class IdExtensions
{
  public static MoveId GetMoveId(this MoveModel move, RealmId? realmId) => new(move.Id, realmId);
}
