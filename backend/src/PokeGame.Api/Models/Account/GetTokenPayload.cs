using FluentValidation;
using System.Text.Json.Serialization;

namespace PokeGame.Api.Models.Account;

public record GetTokenPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }

  [JsonPropertyName("credentials")]
  public SignInPayload? Credentials { get; set; }

  public GetTokenPayload()
  {
  }

  public GetTokenPayload(string refreshToken)
  {
    RefreshToken = refreshToken;
  }

  public GetTokenPayload(SignInPayload credentials)
  {
    Credentials = credentials;
  }
}

internal class GetTokenValidator : AbstractValidator<GetTokenPayload>
{
  public GetTokenValidator()
  {
    When(x => string.IsNullOrWhiteSpace(x.RefreshToken), () => RuleFor(x => x.Credentials).NotNull());
    When(x => !string.IsNullOrWhiteSpace(x.RefreshToken), () => RuleFor(x => x.Credentials).Null());

    When(x => x.Credentials is null, () => RuleFor(x => x.RefreshToken).NotEmpty());
    When(x => x.Credentials is not null, () => RuleFor(x => x.RefreshToken).Empty());
  }
}
