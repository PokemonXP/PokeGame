using CsvHelper.Configuration;
using PokeGame.Core;

namespace PokeGame.Seeding.Game.Payloads;

internal class MedicinePayload
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int Price { get; set; }

  public bool IsHerbal { get; set; }

  public HealingPayload? Healing { get; set; }
  public StatusHealingPayload? Status { get; set; }
  public PowerPointRestorePayload? PowerPoints { get; set; }

  public string? Sprite { get; set; }

  public string? Url { get; set; }
  public string? Notes { get; set; }

  public override bool Equals(object? obj) => obj is MedicinePayload Medicine && Medicine.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";

  public class Map : ClassMap<MedicinePayload>
  {
    public Map()
    {
      Map(x => x.Id).Index(0).Default(Guid.Empty);

      Map(x => x.UniqueName).Index(1).Default(string.Empty);
      Map(x => x.DisplayName).Index(2);
      Map(x => x.Description).Index(3);

      Map(x => x.Price).Index(4).Default(0);
      Map(x => x.IsHerbal).Index(5).Default(false);

      Map(x => x.Healing).Convert(args =>
      {
        int? value = args.Row.GetField<int?>(6);
        bool isPercentage = args.Row.GetField<bool?>(7) ?? false;
        bool revive = args.Row.GetField<bool?>(8) ?? false;
        return value.HasValue || isPercentage || revive ? new HealingPayload(value ?? 0, isPercentage, revive) : null;
      });
      Map(x => x.Status).Convert(args =>
      {
        StatusCondition? condition = args.Row.GetField<StatusCondition?>(9);
        bool all = args.Row.GetField<bool?>(10) ?? false;
        return condition.HasValue || all ? new StatusHealingPayload(condition, all) : null;
      });
      Map(x => x.PowerPoints).Convert(args =>
      {
        int? value = args.Row.GetField<int?>(11);
        bool isPercentage = args.Row.GetField<bool?>(12) ?? false;
        bool allMoves = args.Row.GetField<bool?>(13) ?? false;
        return value.HasValue || isPercentage || allMoves ? new PowerPointRestorePayload(value ?? 0, isPercentage, allMoves) : null;
      });

      Map(x => x.Sprite).Index(14);

      Map(x => x.Url).Index(15);
      Map(x => x.Notes).Index(16);
    }
  }
}
