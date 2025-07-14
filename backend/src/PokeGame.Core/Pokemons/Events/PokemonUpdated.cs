using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Pokemons.Events;

public record PokemonUpdated : DomainEvent
{
  public Change<PokemonGender?>? Gender { get; set; }

  public int? Vitality { get; set; }
  public int? Stamina { get; set; }
  public Change<StatusCondition?>? StatusCondition { get; set; }

  public byte? Friendship { get; set; }

  public Change<Url>? Sprite { get; set; }
  public Change<Url>? Url { get; set; }
  public Change<Description>? Notes { get; set; }
}
