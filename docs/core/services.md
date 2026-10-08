# Service locator & tracing

Every `Bill.X` property is a typed lookup in `ServiceLocator`:

```csharp
public static IPoolService Pool => ServiceLocator.Get<IPoolService>();
```

## Built-in services

| Property | Interface | Implementation |
|---|---|---|
| `Bill.Events` | `IEventBus` | `EventBus` |
| `Bill.Tween` | `ITweenService` | `TweenService` (static shortcuts: `BillTween`) |
| `Bill.Pool` | `IPoolService` | `PoolService` |
| `Bill.Audio` | `IAudioService` | `AudioService` (a MonoBehaviour under the root) |
| `Bill.Scene` | `ISceneService` | `SceneService` |
| `Bill.Timer` | `ITimerService` | `TimerService` |
| `Bill.Save` | `ISaveService` | `SaveService` (PlayerPrefs) |
| `Bill.Config` | `IConfigService` | `ConfigService` |
| `Bill.UI` | `IUIService` | `UIService` (UI Toolkit panels) |
| `Bill.Net` | `INetworkService` | `NetworkService` |
| `Bill.State` | `GameStateMachine` | – |
| `Bill.Cheat`, `Bill.Debug`, `Bill.Analytics` | – | dev tools, see [Dev console](../tools/devtools.md) |

## Register your own service

A service is a class implementing `IService`. Optional lifecycle interfaces:

| Interface | Called |
|---|---|
| `IInitializable.Initialize()` | right after registration |
| `ITickable.Tick(float dt)` | every `Update` |
| `ILateTickable.LateTick(float dt)` | every `LateUpdate` |
| `IDisposableService.Cleanup()` | on shutdown / exit Play Mode |

```csharp
public interface IScoreService : IService { int Score { get; } void Add(int points); }

public class ScoreService : IScoreService, IInitializable
{
    public int Score { get; private set; }
    public void Initialize() => Bill.Events.Subscribe<EnemyKilled>(e => Add(e.Score));
    public void Add(int points) => Score += points;
}

// once, after GameReadyEvent:
ServiceLocator.Register<IScoreService, ScoreService>(new ScoreService());

// anywhere:
ServiceLocator.Get<IScoreService>().Add(5);
```

Registering an interface twice replaces the old instance and logs a warning.

| Method | Use |
|---|---|
| `Register<TInterface, TImpl>(impl)` | register behind an interface |
| `Register<T>(instance)` | register by concrete type |
| `Get<T>()` | get or log an error and return null |
| `TryGet<T>(out T)` | get without logging |
| `Has<T>()` | is it registered |
| `Unregister<T>()` | calls `Cleanup()` and removes it |

## Tracing

Turn on **Enable Tracing** in the config, or `Bill.Trace.Enabled = true` at runtime.

| Call | Prints |
|---|---|
| `Bill.Trace.Print()` | every service, who registered it, who accessed it and how often |
| `Bill.Trace.Log(40)` | the last N accesses |
| `Bill.Trace.HealthCheck()` | each service and whether it initialised |
| `Bill.Trace.Unused()` | services nobody accessed |

The same commands exist in the cheat console: `trace`, `log`, `health`, `unused`.

!!! warning "Tracing cost"
    With tracing on, every `Bill.X` access captures a `System.Diagnostics.StackTrace`. That is fine
    while you debug wiring and far too slow for gameplay. Keep it off in builds.
