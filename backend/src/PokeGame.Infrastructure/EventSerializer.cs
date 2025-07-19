using Krakenar.Infrastructure.Converters;
using PokeGame.Infrastructure.Converters;

namespace PokeGame.Infrastructure;

internal class EventSerializer : Krakenar.Infrastructure.EventSerializer
{
  public EventSerializer(PasswordConverter passwordConverter) : base(passwordConverter)
  {
  }

  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new AbilityIdConverter());
    SerializerOptions.Converters.Add(new AccuracyConverter());
    SerializerOptions.Converters.Add(new CatchRateConverter());
    SerializerOptions.Converters.Add(new EggCyclesConverter());
    SerializerOptions.Converters.Add(new FormIdConverter());
    SerializerOptions.Converters.Add(new FriendshipConverter());
    SerializerOptions.Converters.Add(new GenderRatioConverter());
    SerializerOptions.Converters.Add(new HeightConverter());
    SerializerOptions.Converters.Add(new ItemIdConverter());
    SerializerOptions.Converters.Add(new LevelConverter());
    SerializerOptions.Converters.Add(new LicenseConverter());
    SerializerOptions.Converters.Add(new LocationConverter());
    SerializerOptions.Converters.Add(new MoneyConverter());
    SerializerOptions.Converters.Add(new MoveIdConverter());
    SerializerOptions.Converters.Add(new NicknameConverter());
    SerializerOptions.Converters.Add(new NotesConverter());
    SerializerOptions.Converters.Add(new NumberConverter());
    SerializerOptions.Converters.Add(new PokemonCharacteristicConverter());
    SerializerOptions.Converters.Add(new PokemonIdConverter());
    SerializerOptions.Converters.Add(new PokemonNatureConverter());
    SerializerOptions.Converters.Add(new PowerConverter());
    SerializerOptions.Converters.Add(new PowerPointsConverter());
    SerializerOptions.Converters.Add(new PriceConverter());
    SerializerOptions.Converters.Add(new RegionIdConverter());
    SerializerOptions.Converters.Add(new SpeciesIdConverter());
    SerializerOptions.Converters.Add(new TrainerIdConverter());
    SerializerOptions.Converters.Add(new VarietyIdConverter());
    SerializerOptions.Converters.Add(new WeightConverter());
  }
}
