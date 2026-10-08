# Known issues

Things that work but have sharp edges in 3.1.0. Each one says what to do instead.

| Issue | Workaround |
|---|---|
| A `Tween` reference held after the tween finished can point at a recycled tween; `Kill()` on it stops someone else's tween. | Use `SetTarget` + `BillTween.KillTarget`, or clear your reference in `OnComplete`. |
| `SubscribeOnce` wraps the handler, so `Unsubscribe` with the original handler does not remove it. | Use `Subscribe` + `Unsubscribe` in the handler when you may need to cancel before it fires. |
| Event channels are static; a destroyed subscriber keeps receiving events. | Unsubscribe in `OnDisable`. |
| The cheat console toggle uses the legacy `Input` class. | Set *Active Input Handling* to *Both*, or call `Bill.Cheat.SetVisible(true)`. |
| `Bill.Debug` (the overlay) shares its name with `UnityEngine.Debug`. `Debug.Log` in your own code is unaffected; it only matters in code written inside the `Bill` class. | Write `UnityEngine.Debug.Log` there. |
| The bootstrap registers every service; there is no way to leave one out. | Unused services cost little; `Bill.Trace.Unused()` lists them. |
| `TimerHandle` and every `BillTween` shortcut allocate a little garbage per call. | Fine for gameplay events. Avoid creating tweens or timers every frame. |
| Fusion networking is not covered by tests. | Treat `PHOTON_FUSION` support as a starting point. |

## Roadmap

- Split the runtime into smaller packages (`core`, `tween`, `pool`, `audio`, `flow`) with opt-in
  service registration.
- Versioned tween handles so stale references are harmless.
- Input System support in the dev console.
