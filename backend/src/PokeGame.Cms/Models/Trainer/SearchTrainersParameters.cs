using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Cms.Models.Trainer;

public record SearchTrainersParameters : SearchParameters
{
  [FromQuery(Name = "gender")]
  public TrainerGender? Gender { get; set; }

  [FromQuery(Name = "user")]
  public Guid? UserId { get; set; }

  public virtual SearchTrainersPayload ToPayload()
  {
    SearchTrainersPayload payload = new()
    {
      Gender = Gender,
      UserId = UserId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out TrainerSort field))
      {
        payload.Sort.Add(new TrainerSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
