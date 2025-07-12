using FluentValidation;

namespace PokeGame.Api.Models.Account;

public record SignInPayload
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public SignInPayload()
  {
  }

  public SignInPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}

internal class SignInValidator : AbstractValidator<SignInPayload>
{
  public SignInValidator()
  {
    RuleFor(x => x.Username).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
