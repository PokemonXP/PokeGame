using Logitar.EventSourcing;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Storage;

public readonly struct PokemonStorageId
{
  private const string EntityType = "Storage";
  private const char Separator = '|';

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public TrainerId TrainerId => new(Value.Split(Separator).First());

  public PokemonStorageId(StreamId streamId)
  {
    StreamId = streamId;
  }
  public PokemonStorageId(TrainerId trainerId)
  {
    StreamId = new StreamId(string.Join(Separator, trainerId, EntityType));
  }
  public PokemonStorageId(string value)
  {
    StreamId = new StreamId(value);
  }

  public static bool operator ==(PokemonStorageId left, PokemonStorageId right) => left.Equals(right);
  public static bool operator !=(PokemonStorageId left, PokemonStorageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is PokemonStorageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
