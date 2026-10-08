# Timer

`Bill.Timer` runs callbacks after a delay or on an interval, ticked by the framework (no coroutines,
no `MonoBehaviour` needed).

```csharp
TimerHandle h = Bill.Timer.Delay(0.4f, Fire);
Bill.Timer.Delay(2f, ShowHint, unscaled: true);   // keeps running while Time.timeScale = 0
Bill.Timer.Repeat(1f, SpawnWave);                  // forever
Bill.Timer.Repeat(0.1f, Blink, count: 6);          // six times

h.Cancel();                 // or Bill.Timer.Cancel(h)
bool running = h.IsActive;
Bill.Timer.CancelAll();
```

| Method | |
|---|---|
| `Delay(seconds, callback)` | once, scaled time |
| `Delay(seconds, callback, unscaled)` | once, choose time scale |
| `Repeat(interval, callback)` | every `interval` seconds until cancelled |
| `Repeat(interval, callback, count)` | `count` times |
| `Cancel(handle)`, `CancelAll()` | stop |
| `ActiveCount` | timers currently scheduled |

A timer does not know about the object that created it. Cancel it in `OnDisable`/`OnDestroy` if the
callback touches a component that can be destroyed first.

Each call allocates one small `TimerHandle`. Fine for gameplay events; for per-frame work use `Update`.

For a delay that should die with a target, `BillTween.DelayedCall(seconds, callback)?.SetTarget(this)`
plus `BillTween.KillTarget(this)` is an alternative.
