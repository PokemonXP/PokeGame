using Krakenar.Contracts.Sessions;
using PokeGame.Api.Constants;
using PokeGame.Api.Extensions;

internal class RenewSession
{
  protected virtual RequestDelegate Next { get; }

  public RenewSession(RequestDelegate next)
  {
    Next = next;
  }

  public virtual async Task InvokeAsync(HttpContext context, ISessionService sessionService)
  {
    if (!context.IsSignedIn())
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken is not null)
      {
        try
        {
          RenewSessionPayload payload = new(refreshToken, context.GetSessionCustomAttributes());
          Session session = await sessionService.RenewAsync(payload);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await Next(context);
  }
}
