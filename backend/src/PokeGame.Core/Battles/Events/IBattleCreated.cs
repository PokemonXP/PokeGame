using Krakenar.Core;
using PokeGame.Core.Regions;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Battles.Events;

public interface IBattleCreated
{
  DisplayName Name { get; }
  Location Location { get; }
  Url? Url { get; }
  Notes? Notes { get; }

  IReadOnlyCollection<TrainerId> ChampionIds { get; }
}
