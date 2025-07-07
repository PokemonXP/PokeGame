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
    SerializerOptions.Converters.Add(new PokemonIdConverter());
  }
}
