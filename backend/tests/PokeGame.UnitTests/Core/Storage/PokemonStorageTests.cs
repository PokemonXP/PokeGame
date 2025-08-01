using Bogus;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Storage;

[Trait(Traits.Category, Categories.Unit)]
public class PokemonStorageTests // TASK: [POKEGAME-263](https://logitar.atlassian.net/browse/POKEGAME-263)
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly Faker _faker = new();
  private readonly IPokemonRandomizer _randomizer = PokemonRandomizer.Instance;

  private readonly Trainer _trainer;
  private readonly PokemonStorage _storage;

  public PokemonStorageTests()
  {
    _trainer = _faker.Trainer();
    _storage = new PokemonStorage(_trainer);
  }
}
