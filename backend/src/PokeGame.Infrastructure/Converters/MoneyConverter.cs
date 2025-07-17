using PokeGame.Core.Trainers;

namespace PokeGame.Infrastructure.Converters;

internal class MoneyConverter : JsonConverter<Money>
{
  public override Money? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.TryGetInt32(out int value) ? new Money(value) : null;
  }

  public override void Write(Utf8JsonWriter writer, Money money, JsonSerializerOptions options)
  {
    writer.WriteNumberValue(money.Value);
  }
}
