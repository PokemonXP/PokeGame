using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using MediatR;
using PokeGame.Seeding.Game.Tasks;
using PokeGame.Seeding.Krakenar.Tasks;
using PokeGame.Seeding.Settings;
using UserDto = Krakenar.Contracts.Users.User;

namespace PokeGame.Seeding;

internal class SeedingWorker : BackgroundService
{
  private const string GenericErrorMessage = "An unhanded exception occurred.";

  private readonly SeedingApplicationContext _applicationContext;
  private readonly IConfiguration _configuration;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly ILogger<SeedingWorker> _logger;
  private readonly IServiceProvider _serviceProvider;

  private IMediator? _mediator = null;
  private IMediator Mediator => _mediator ?? throw new InvalidOperationException($"The {nameof(Mediator)} has not been initialized yet.");

  private LogLevel _result = LogLevel.Information; // NOTE(fpion): "Information" means success.

  public SeedingWorker(
    IApplicationContext applicationContext,
    IConfiguration configuration,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<SeedingWorker> logger,
    IServiceProvider serviceProvider)
  {
    _applicationContext = (SeedingApplicationContext)applicationContext;
    _configuration = configuration;
    _hostApplicationLifetime = hostApplicationLifetime;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();
    _logger.LogInformation("Worker executing at {Timestamp}.", DateTimeOffset.Now);

    using IServiceScope scope = _serviceProvider.CreateScope();
    _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    try
    {
      DefaultSettings defaults = DefaultSettings.Initialize(_configuration);

      // NOTE(fpion): the order of these tasks matter.
      await ExecuteAsync(new MigrateDatabaseTask(), cancellationToken);
      await ExecuteAsync(new InitializeConfigurationTask(defaults), cancellationToken);

      IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
      UserDto user = await userService.ReadAsync(id: null, defaults.UniqueName, customIdentifier: null, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'UniqueName={defaults.UniqueName}' was not found.");
      _applicationContext.ActorId = new ActorId(new UserId(user.Id).Value);

      await ExecuteAsync(new SeedRealmsTask(), cancellationToken);
      await ExecuteAsync(new SeedContentTypesTask(), cancellationToken);
      await ExecuteAsync(new SeedFieldTypesTask(), cancellationToken);
      await ExecuteAsync(new SeedContentTypesTask(fieldDefinitions: true), cancellationToken);

      await ExecuteAsync(new SeedRegionsTask(defaults.Locale), cancellationToken);
      await ExecuteAsync(new SeedAbilitiesTask(defaults.Locale), cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, GenericErrorMessage);
      _result = LogLevel.Error;

      Environment.ExitCode = exception.HResult;
    }
    finally
    {
      chrono.Stop();

      long seconds = chrono.ElapsedMilliseconds / 1000;
      string secondText = seconds <= 1 ? "second" : "seconds";
      switch (_result)
      {
        case LogLevel.Error:
          _logger.LogError("Seeding failed after {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
        case LogLevel.Warning:
          _logger.LogWarning("Seeding completed with warnings in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
        default:
          _logger.LogInformation("Seeding succeeded in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
      }

      _hostApplicationLifetime.StopApplication();
    }
  }

  private async Task ExecuteAsync(SeedingTask task, CancellationToken cancellationToken)
  {
    await ExecuteAsync(task, continueOnError: false, cancellationToken);
  }
  private async Task ExecuteAsync(SeedingTask task, bool continueOnError, CancellationToken cancellationToken)
  {
    bool hasFailed = false;
    try
    {
      await Mediator.Publish(task, cancellationToken);
    }
    catch (Exception exception)
    {
      if (continueOnError)
      {
        _logger.LogWarning(exception, GenericErrorMessage);
        hasFailed = true;
      }
      else
      {
        throw;
      }
    }
    finally
    {
      task.Complete();

      LogLevel result = LogLevel.Information;
      if (hasFailed)
      {
        _result = LogLevel.Warning;
        result = LogLevel.Warning;
      }

      int milliseconds = task.Duration?.Milliseconds ?? 0;
      int seconds = milliseconds / 1000;
      string secondText = seconds <= 1 ? "second" : "seconds";
      _logger.Log(result, "Task '{Name}' succeeded in {Elapsed}ms ({Seconds} {SecondText}).", task.Name, milliseconds, seconds, secondText);
    }
  }
}
