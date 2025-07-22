using Krakenar.Contracts.Sessions;
using PokeGame.Api.Constants;

namespace PokeGame.Api.Models.Account;

public record CurrentUser
{
  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public bool IsAdmin { get; set; }

  public CurrentUser() : this(string.Empty)
  {
  }

  public CurrentUser(string displayName)
  {
    DisplayName = displayName;
  }

  public CurrentUser(Krakenar.Contracts.Users.User user) : this(user.FullName ?? user.UniqueName)
  {
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;

    IsAdmin = user.Roles.Any(role => role.UniqueName.Equals(Roles.Admin, StringComparison.InvariantCultureIgnoreCase));
  }

  public CurrentUser(Session session) : this(session.User)
  {
  }
}
