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

    SerializerOptions.Converters.Add(new FormIdConverter());
    SerializerOptions.Converters.Add(new FriendshipConverter());
    SerializerOptions.Converters.Add(new GameLocationConverter());
    SerializerOptions.Converters.Add(new ItemIdConverter());
    SerializerOptions.Converters.Add(new MoveIdConverter());
    SerializerOptions.Converters.Add(new NicknameConverter());
    SerializerOptions.Converters.Add(new NotesConverter());
    SerializerOptions.Converters.Add(new PokemonCharacteristicConverter());
    SerializerOptions.Converters.Add(new PokemonIdConverter());
    SerializerOptions.Converters.Add(new PokemonNatureConverter());
    SerializerOptions.Converters.Add(new PowerPointsConverter());
    SerializerOptions.Converters.Add(new SpeciesIdConverter());
    SerializerOptions.Converters.Add(new TrainerIdConverter());
    SerializerOptions.Converters.Add(new VarietyIdConverter());
  }
}
