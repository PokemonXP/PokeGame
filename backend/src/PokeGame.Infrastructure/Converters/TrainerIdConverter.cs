using PokeGame.Core.Trainers;

namespace PokeGame.Infrastructure.Converters;

internal class TrainerIdConverter : JsonConverter<TrainerId>
{
  public override TrainerId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new TrainerId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, TrainerId trainerId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(trainerId.Value);
  }
}
