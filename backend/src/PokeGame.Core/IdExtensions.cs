using Krakenar.Core.Realms;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.Core.Items;
using PokeGame.Core.Items.Models;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Core;

internal static class IdExtensions
{
  public static FormId GetFormId(this FormModel form, RealmId? realmId) => new(form.Id, realmId);
  public static ItemId GetItemId(this ItemModel item, RealmId? realmId) => new(item.Id, realmId);
  public static MoveId GetMoveId(this MoveModel move, RealmId? realmId) => new(move.Id, realmId);
  public static TrainerId GetTrainerId(this TrainerModel trainer, RealmId? realmId) => new(trainer.Id, realmId);
}
