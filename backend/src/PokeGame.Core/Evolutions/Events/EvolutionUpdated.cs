using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Items;
using PokeGame.Core.Moves;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Regions;

namespace PokeGame.Core.Evolutions.Events;

public record EvolutionUpdated : DomainEvent
{
  public Change<Level>? Level { get; set; }
  public bool? Friendship { get; set; }
  public Change<PokemonGender?>? Gender { get; set; }
  public Change<ItemId?>? HeldItemId { get; set; }
  public Change<MoveId?>? KnownMoveId { get; set; }
  public Change<Location>? Location { get; set; }
  public Change<TimeOfDay?>? TimeOfDay { get; set; }
}
