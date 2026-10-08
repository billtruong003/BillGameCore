# BillGameCore

A code-first game framework for **Unity 6**. You install a package, add one config asset, and press
Play. Before your first scene loads, BillGameCore has registered its services, opened an event bus and
entered a game state. Gameplay code talks to everything through one static entry point, `Bill`.

```csharp
Bill.Events.Fire(new EnemyKilled { Score = 10 });
Bill.Audio.Play("sfx.enemy.death", transform.position);
Bill.Pool.Spawn("fx.explosion", transform.position, Quaternion.identity);
BillTween.Scale(transform, 0f, 0.2f)?.SetEase(EaseType.InBack);
Bill.State.GoTo<GameOverState>();
```

## What you get

| Area | API | Page |
|---|---|---|
| Startup | auto-bootstrap, `BillBootstrapConfig`, `Bill.IsReady` | [Bootstrap & config](core/bootstrap.md) |
| Wiring | `ServiceLocator`, `Bill.Trace` | [Service locator](core/services.md) |
| Messages | `Bill.Events` (struct events) | [EventBus](core/events.md) |
| Flow | `Bill.State` | [Game state machine](core/state-machine.md) |
| Animation | `BillTween`, `TweenSequence` | [BillTween](services/tween.md) |
| Spawning | `Bill.Pool` | [Pool](services/pool.md) |
| Sound | `Bill.Audio`, `AudioLibrary` | [Audio](services/audio.md) |
| Scenes | `Bill.Scene` | [Scenes](services/scene.md) |
| Delays | `Bill.Timer` | [Timer](services/timer.md) |
| Persistence | `Bill.Save`, `Bill.Config` | [Save & config](services/save-config.md) |
| Debugging | `Bill.Cheat`, `Bill.Debug` | [Dev console](tools/devtools.md) |

## Design rules

1. **One entry point.** Everything is `Bill.Something`. You never `new` a service or search the scene
   for it.
2. **No scene setup.** The bootstrap runs from `[RuntimeInitializeOnLoadMethod]`. A bootstrap scene at
   build index 0 is the only convention.
3. **Struct events.** Game-wide signals are `struct : IEvent`, so firing one does not allocate.
4. **Pooled tweens, not DOTween.** BillTween reuses tween objects. The only allocation per call is the
   setter closure.
5. **Traceable.** `Bill.Trace.Print()` shows which code touched which service, and
   `Bill.Trace.Unused()` lists services nobody uses.

## Packages

| Package | Install path |
|---|---|
| `com.bill.gamecore` 3.1.0 | `?path=/Packages/com.bill.gamecore#v3.1.0` |
| `com.bill.inspector` 0.5.0 | `?path=/Packages/com.bill.inspector#v3.1.0` |

Next: [Installation](getting-started/installation.md).
