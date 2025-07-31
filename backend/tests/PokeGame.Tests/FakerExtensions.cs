using Bogus;
using Krakenar.Core;
using Krakenar.Core.Settings;
using PokeGame.Core.Trainers;
using NameGender = Bogus.DataSets.Name.Gender;

namespace PokeGame;

public static class FakerExtensions
{
  public static NameGender ToNameGender(this TrainerGender gender) => gender switch
  {
    TrainerGender.Female => NameGender.Female,
    _ => NameGender.Male,
  };

  public static License License(this Faker faker, TrainerGender gender)
  {
    int number = 9;
    switch (gender)
    {
      case TrainerGender.Female:
        number = faker.Random.Int(min: 5, max: 8);
        break;
      case TrainerGender.Male:
        number = faker.Random.Int(min: 1, max: 4);
        break;
    }
    number *= 100000;
    number += faker.Random.Int(min: 0, max: 99999);

    int checksum = Checksum(number);

    string value = string.Join('-', ["Q", number.ToString(), checksum.ToString()]);
    return new License(value);
  }
  private static int Checksum(int value)
  {
    if (value < 10)
    {
      return value;
    }

    int sum = 0;
    while (value > 0)
    {
      sum += value % 10;
      value /= 10;
    }
    return Checksum(sum);
  }

  public static Trainer Trainer(this Faker faker, TrainerGender? gender = null)
  {
    if (!gender.HasValue)
    {
      gender = faker.PickRandom(Enum.GetValues<TrainerGender>());
    }

    License license = faker.License(gender.Value);
    UniqueName uniqueName = new(new UniqueNameSettings(), license.Value);

    DisplayName displayName = new(faker.Name.FullName(gender.Value.ToNameGender()));

    Trainer trainer = new(license, uniqueName, gender.Value)
    {
      DisplayName = displayName
    };
    trainer.Update();
    return trainer;
  }
}
