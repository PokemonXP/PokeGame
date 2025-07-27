using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Storage;

public class PokemonStorageFullException : BadRequestException
{
  private const string ErrorMessage = "The trainer Pokémon storage is full.";

  public Guid TrainerId
  {
    get => (Guid)Data[nameof(TrainerId)]!;
    private set => Data[nameof(TrainerId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(TrainerId)] = TrainerId;
      return error;
    }
  }

  public PokemonStorageFullException(PokemonStorage storage) : this(storage.TrainerId)
  {
  }
  public PokemonStorageFullException(TrainerId trainerId) : base(BuildMessage(trainerId))
  {
    TrainerId = trainerId.ToGuid();
  }

  private static string BuildMessage(TrainerId trainerId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TrainerId), trainerId.ToGuid())
    .Build();
}
