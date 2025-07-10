using FluentValidation;

namespace PokeGame.Api.Models.Account;

public record SignInPayload
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

internal class SignInValidator : AbstractValidator<SignInPayload>
{
  public SignInValidator()
  {
    RuleFor(x => x.Username).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
