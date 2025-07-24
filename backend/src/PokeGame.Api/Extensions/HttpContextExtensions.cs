﻿using Krakenar.Contracts;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Constants;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Microsoft.Extensions.Primitives;
using PokeGame.Api.Constants;
using PokeGame.Api.Settings;

namespace PokeGame.Api.Extensions;

internal static class HttpContextExtensions
{
  private const string ApiKeyKey = "ApiKey";
  private const string SessionIdKey = "SessionId";
  private const string SessionKey = "Session";
  private const string UserKey = "User";

  public static IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes(this HttpContext context)
  {
    List<CustomAttribute> customAttributes = new(capacity: 2)
    {
      new("AdditionalInformation", context.GetAdditionalInformation())
    };

    string? ipAddress = context.GetClientIpAddress();
    if (ipAddress is not null)
    {
      customAttributes.Add(new("IpAddress", ipAddress));
    }

    return customAttributes.AsReadOnly();
  }
  public static string GetAdditionalInformation(this HttpContext context)
  {
    HttpRequest request = context.Request;
    Dictionary<string, string> additionalInformation = new(capacity: 4);

    string userAgent = request.Headers.UserAgent.ToString();
    if (!string.IsNullOrWhiteSpace(userAgent))
    {
      additionalInformation[Headers.UserAgent] = Sanitize(userAgent);
    }

    string secureClientHintUserAgent = request.Headers[Headers.SecureClientHintUserAgent].ToString();
    if (!string.IsNullOrWhiteSpace(secureClientHintUserAgent))
    {
      additionalInformation[Headers.SecureClientHintUserAgent] = Sanitize(secureClientHintUserAgent);
    }

    string secureClientHintUserAgentMobile = request.Headers[Headers.SecureClientHintUserAgentMobile].ToString();
    if (!string.IsNullOrWhiteSpace(secureClientHintUserAgentMobile))
    {
      additionalInformation[Headers.SecureClientHintUserAgentMobile] = Sanitize(secureClientHintUserAgentMobile);
    }

    string secureClientHintUserAgentPlatform = request.Headers[Headers.SecureClientHintUserAgentPlatform].ToString();
    if (!string.IsNullOrWhiteSpace(secureClientHintUserAgentPlatform))
    {
      additionalInformation[Headers.SecureClientHintUserAgentPlatform] = Sanitize(secureClientHintUserAgentPlatform);
    }

    return JsonSerializer.Serialize(additionalInformation);
  }
  private static string Sanitize(string value)
  {
    value = value.Trim();
    return value[1..^1].Contains('"') ? value : value.Trim('"');
  }
  public static string? GetClientIpAddress(this HttpContext context)
  {
    string? ipAddress = null;

    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xForwardedFor))
    {
      IReadOnlyCollection<string> sanitized = xForwardedFor.Sanitize();
      if (sanitized.Count == 1)
      {
        ipAddress = sanitized.Single().Split(':').First();
      }
    }
    ipAddress ??= context.Connection.RemoteIpAddress?.ToString();

    return ipAddress;
  }

  public static ApiKey? GetApiKey(this HttpContext context) => context.GetItem<ApiKey>(ApiKeyKey);
  public static Session? GetSession(this HttpContext context) => context.GetItem<Session>(SessionKey);
  public static User? GetUser(this HttpContext context) => context.GetItem<User>(UserKey);
  public static Guid GetUserId(this HttpContext context) => context.GetUser()?.Id ?? throw new InvalidOperationException("An authenticated user is required.");
  public static T? GetItem<T>(this HttpContext context, object key) => context.Items.TryGetValue(key, out object? value) ? (T?)value : default;

  public static void SetApiKey(this HttpContext context, ApiKey? apiKey)
  {
    context.SetItem(ApiKeyKey, apiKey);
  }
  public static void SetSession(this HttpContext context, Session? session)
  {
    context.SetItem(SessionKey, session);
  }
  public static void SetUser(this HttpContext context, User? user)
  {
    context.SetItem(UserKey, user);
  }
  public static void SetItem<T>(this HttpContext context, object key, T? value)
  {
    if (value is null)
    {
      context.Items.Remove(key);
    }
    else
    {
      context.Items[key] = value;
    }
  }

  public static Guid? GetSessionId(this HttpContext context)
  {
    byte[]? bytes = context.Session.Get(SessionIdKey);

    return bytes is null ? null : new Guid(bytes);
  }
  public static bool IsSignedIn(this HttpContext context) => context.GetSessionId().HasValue;
  public static void SignIn(this HttpContext context, Session session)
  {
    context.Session.Set(SessionIdKey, session.Id.ToByteArray());

    if (session.RefreshToken is not null)
    {
      CookiesSettings cookiesSettings = context.RequestServices.GetRequiredService<CookiesSettings>();
      CookieOptions options = new()
      {
        HttpOnly = cookiesSettings.RefreshToken.HttpOnly,
        MaxAge = cookiesSettings.RefreshToken.MaxAge,
        SameSite = cookiesSettings.RefreshToken.SameSite,
        Secure = cookiesSettings.RefreshToken.Secure
      };
      context.Response.Cookies.Append(Cookies.RefreshToken, session.RefreshToken, options);
    }

    context.SetSession(session);
    context.SetUser(session.User);
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
