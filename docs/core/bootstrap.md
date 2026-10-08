# Bootstrap & config

## What happens when you press Play

BillGameCore boots itself with two `[RuntimeInitializeOnLoadMethod]` hooks. You never add a component
or call an init method.

**Phase 1, before the first scene loads**

1. Load `Resources/BillBootstrapConfig`. If it is missing, log an error and stop.
2. Apply `targetFrameRate`, `vSyncCount` and `enableTracing`.
3. If **Enforce Bootstrap Scene** is on and the active scene is not build index 0, remember the
   current scene (Editor only) and load scene 0 instead.

**Phase 2, after the scene loads**

1. Create a `[BillGameCore]` root object marked `DontDestroyOnLoad`, with a coroutine runner that ticks
   every service.
2. Register services in this order. Each step is timed and logged as `[Bill] + Step (x ms)`.

    | Step | Services |
    |---|---|
    | Infrastructure | `IEventBus` |
    | Core Services | `IConfigService`, `ISaveService`, `ITimerService`, `ITweenService`, `IAudioService`, `IPoolService`, `IUIService`, `ISceneService` |
    | State Machine | `GameStateMachine` with `BootState`, `MenuState`, `LoadingState`, `GameplayState`, `PauseState`, `GameOverState` |
    | Network | `INetworkService` (offline adapter unless `PHOTON_FUSION`) |
    | Dev Tools | `DebugOverlay`, `CheatConsole`, `AnalyticsTracker` (Editor, development builds, or `BILL_CHEATS`) |

3. Mark the locator initialised, enter `BootState`, fire `GameReadyEvent`, log
   `[Bill] Ready. N services in X ms.`
4. In the Editor, return to the scene you pressed Play in. Otherwise load **Default Game Scene** if
   it is set.

## `Bill.IsReady`

`true` once phase 2 has finished. Code that may run earlier (an `Awake`/`Start` in scene 0, a
`[RuntimeInitializeOnLoadMethod]` of your own) should wait:

```csharp
void Start()
{
    if (!Bill.IsReady) { Bill.Events.SubscribeOnce<GameReadyEvent>(_ => Init()); return; }
    Init();
}
```

`Bill.Events` exists from the first step, so subscribing before `IsReady` is safe.

!!! warning "Tweens and timers need the bootstrap"
    `Bill.Tween` and `Bill.Timer` are ticked by the root object. If the bootstrap never ran, tweens
    return `null` (hence the `?.` in every example) and timers never fire.

## `BillBootstrapConfig`

**Create ▸ BillGameCore ▸ Bootstrap Config**, saved as `Assets/Resources/BillBootstrapConfig.asset`.

| Field | Default | Meaning |
|---|---|---|
| Enforce Bootstrap Scene | on | always boot through build index 0 |
| Default Game Scene | empty | scene to load after booting (player builds, or Editor when not returning) |
| Return To Edit Scene In Editor | on | after booting in the Editor, reload the scene you pressed Play in |
| Include Debug Overlay | on | register `Bill.Debug` |
| Include Cheat Console | on | register `Bill.Cheat` |
| Show Overlay On Startup | off | open the overlay immediately |
| Enable Tracing | off | record who accesses which service (costs a stack trace per access) |
| Default Network Mode | Offline | starting `NetworkMode` |
| Default Pools | empty | `PoolDefinition[]` registered at boot (key, prefab, warm count, max size, auto return) |
| Default Audio Library | none | `AudioLibrary` used by `Bill.Audio` |
| Master / Music / SFX Volume | 1 / 0.8 / 1 | initial channel volumes |
| Target Frame Rate | 60 | `Application.targetFrameRate`; 0 leaves it alone |
| VSync Count | 0 | `QualitySettings.vSyncCount` |

## Domain reload off

BillGameCore works with **Enter Play Mode Options ▸ Reload Domain** disabled. Services are torn down on
exit and statics are reset by the next boot.
