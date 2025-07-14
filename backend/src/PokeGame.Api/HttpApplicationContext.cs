using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using PokeGame.Api.Extensions;
using Actor = Krakenar.Contracts.Actors.Actor;
using ApiKey = Krakenar.Contracts.ApiKeys.ApiKey;
using Realm = Krakenar.Contracts.Realms.Realm;
using User = Krakenar.Contracts.Users.User;

namespace PokeGame.Api;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public ActorId? ActorId
  {
    get
    {
      User? user = Context.GetUser();
      if (user is not null)
      {
        return new Actor(user).GetActorId();
      }

      ApiKey? apiKey = Context.GetApiKey();
      if (apiKey is not null)
      {
        return new Actor(apiKey).GetActorId();
      }

      return null;
    }
  }

  public string BaseUrl => throw new NotImplementedException();

  public Realm? Realm => Context.GetUser()?.Realm ?? Context.GetApiKey()?.Realm;
  public RealmId? RealmId => Realm is null ? null : new(Realm.Id);

  public Secret Secret => throw new NotImplementedException();
  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? throw new InvalidOperationException("The Realm is required");
  public IPasswordSettings PasswordSettings => throw new NotImplementedException();
  public bool RequireUniqueEmail => throw new NotImplementedException();
  public bool RequireConfirmedAccount => throw new NotImplementedException();

  public ILoggingSettings LoggingSettings => throw new NotImplementedException();
}
