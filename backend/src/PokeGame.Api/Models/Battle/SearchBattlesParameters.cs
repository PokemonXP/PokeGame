using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Battles;
using PokeGame.Core.Battles.Models;

namespace PokeGame.Api.Models.Battle;

public record SearchBattlesParameters : SearchParameters
{
  [FromQuery(Name = "kind")]
  public BattleKind? Kind { get; set; }

  [FromQuery(Name = "status")]
  public BattleStatus? Status { get; set; }

  [FromQuery(Name = "resolution")]
  public BattleResolution? Resolution { get; set; }

  [FromQuery(Name = "trainer")]
  public Guid? TrainerId { get; set; }

  public virtual SearchBattlesPayload ToPayload()
  {
    SearchBattlesPayload payload = new()
    {
      Kind = Kind,
      Status = Status,
      Resolution = Resolution,
      TrainerId = TrainerId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out BattleSort field))
      {
        payload.Sort.Add(new BattleSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
