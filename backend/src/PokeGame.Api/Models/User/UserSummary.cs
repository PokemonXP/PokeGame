namespace PokeGame.Api.Models.User;

public class UserSummary
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? FullName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public UserSummary()
  {
  }

  public UserSummary(Krakenar.Contracts.Users.User user)
  {
    Id = user.Id;

    UniqueName = user.UniqueName;
    FullName = user.FullName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }

  public override bool Equals(object? obj) => obj is UserSummary user && user.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder user = new();
    user.Append(FullName ?? UniqueName);
    if (EmailAddress is not null)
    {
      user.Append(" <").Append(EmailAddress).Append('>');
    }
    user.Append(" (Id=").Append(Id).Append(')');
    return user.ToString();
  }
}
