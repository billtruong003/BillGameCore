# EventBus

`Bill.Events` delivers game-wide messages. An event is a `struct` implementing `IEvent`, so firing it
does not allocate.

```csharp
public struct WaveStarted : IEvent { public int Wave; public int EnemyCount; }

Bill.Events.Fire(new WaveStarted { Wave = 3, EnemyCount = 40 });
Bill.Events.Fire<GameReadyEvent>();          // payload-less, uses default(T)
```

## Subscribe and unsubscribe

```csharp
void OnEnable()  => Bill.Events.Subscribe<WaveStarted>(OnWave);
void OnDisable() => Bill.Events.Unsubscribe<WaveStarted>(OnWave);
void OnWave(WaveStarted e) { /* ... */ }
```

| Method | Notes |
|---|---|
| `Subscribe<T>(handler)` | adding the same handler twice is ignored |
| `SubscribeOnce<T>(handler)` | removed after the first call; it wraps your handler, so `Unsubscribe` with the original handler does **not** remove it |
| `Unsubscribe<T>(handler)` | pass the same method group or delegate you subscribed |
| `Fire<T>(data)` | calls handlers newest-first; an exception in one handler is logged with its stack and does not stop the others |

!!! warning "Channels are static"
    Each event type has one static listener list that lives until the framework shuts down. A
    component that subscribes in `OnEnable` must unsubscribe in `OnDisable`, or destroyed objects keep
    getting called.

Subscribing and unsubscribing from inside a handler is allowed.

## When to use it

Use `Bill.Events` for signals several unrelated systems care about: wave started, player died,
currency changed. Inside one component or between a component and its children, a plain C#
`event Action` is simpler and cheaper.

## Built-in events

| Event | Fired when |
|---|---|
| `GameReadyEvent` | the bootstrap finished |
| `AppPauseEvent { IsPaused }` | the app is paused or resumed (mobile background) |
| `SceneLoadStartEvent { SceneName }` | `Bill.Scene` starts a load |
| `SceneLoadProgressEvent` | load progress while loading asynchronously |
| `SceneLoadCompleteEvent { SceneName }` | the load finished |
| `StateChangedEvent { From, To }` | `Bill.State` changed state |
| `NetworkPhaseChangedEvent { Phase }` | the network phase changed |
| `ConfigRefreshedEvent` | `Bill.Config.ApplyRemote` applied new values |
