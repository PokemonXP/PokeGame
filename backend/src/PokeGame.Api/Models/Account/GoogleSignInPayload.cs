using FluentValidation;

namespace PokeGame.Api.Models.Account;

public record GoogleSignInPayload
{
  public string Token { get; set; }

  public GoogleSignInPayload() : this(string.Empty)
  {
  }

  public GoogleSignInPayload(string token)
  {
    Token = token;
  }
}

internal class GoogleSignInValidator : AbstractValidator<GoogleSignInPayload>
{
  public GoogleSignInValidator()
  {
    RuleFor(x => x.Token).NotEmpty();
  }
}
