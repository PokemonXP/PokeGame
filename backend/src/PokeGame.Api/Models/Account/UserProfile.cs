using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Sessions;
using PokeGame.Api.Constants;

namespace PokeGame.Api.Models.Account;

public record UserProfile
{
  public string Username { get; set; }

  public string? EmailAddress { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public Locale? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }

  public DateTime CreatedOn { get; set; }
  public DateTime UpdatedOn { get; set; }
  public DateTime? AuthenticatedOn { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public bool IsAdmin { get; set; }

  public UserProfile() : this(string.Empty)
  {
  }

  public UserProfile(string username)
  {
    Username = username;
  }

  public UserProfile(Krakenar.Contracts.Users.User user) : this(user.UniqueName)
  {
    EmailAddress = user.Email?.Address;

    FirstName = user.FirstName;
    LastName = user.LastName;
    FullName = user.FullName;

    Birthdate = user.Birthdate;
    Gender = user.Gender;
    Locale = user.Locale;
    TimeZone = user.TimeZone;

    Picture = user.Picture;

    CreatedOn = user.CreatedOn;
    UpdatedOn = user.UpdatedOn;
    AuthenticatedOn = user.AuthenticatedOn;
    PasswordChangedOn = user.PasswordChangedOn;

    IsAdmin = user.Roles.Any(role => role.UniqueName.Equals(Roles.Admin, StringComparison.InvariantCultureIgnoreCase));
  }

  public UserProfile(Session session) : this(session.User)
  {
  }
}
