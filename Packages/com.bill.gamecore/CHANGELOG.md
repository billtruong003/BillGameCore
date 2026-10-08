# Changelog

All notable changes to `com.bill.gamecore`. Versions follow [Semantic Versioning](https://semver.org).

## [3.1.0] - 2026-10-08

First release as a standalone UPM package (previously copied into `Assets/`).

### Fixed
- **BillTween sequences ticked their tweens twice.** A tween created by `BillTween.*` was also left in
  the service's active list after `Append`/`Join`, so it ran at double speed, appended steps started
  immediately, and the service recycled it while the sequence still held it. Sequenced tweens now
  leave the active list and are driven only by their sequence. `Move`, `LocalMove` and `ScaleTo` (and
  `TweenMove`/`TweenLocalMove`/`TweenScaleTo`) were affected every time.
- **`Join` did not run in parallel.** Joined steps only started after the step before them finished.
  A step and the `Join`/`Insert` steps right after it now form a group that starts together; the next
  `Append` waits for the whole group.
- `KillTarget` now also stops sequences that contain a tween with that target.
- An exception inside an event handler is logged with its stack trace (it used to log only the
  message).

### Changed
- Package layout: `Runtime/`, `Editor/` (own asmdef `BillGameCore.Editor`), `Extras/`, `Tests/`,
  `Samples~/`. Samples no longer compile into `Assembly-CSharp` and no longer ship in player builds.
- `BillGameCore.Runtime` no longer references `BillInspector.Runtime`.
  `DynamicAnimationEventHub` moved to the optional `BillGameCore.Extras` assembly, compiled only
  when `com.bill.inspector` is installed, and now lives in the `BillGameCore` namespace.
- Cheat build define renamed from `ZW_CHEATS` to **`BILL_CHEATS`**. Add `BILL_CHEATS` to Scripting
  Define Symbols to keep `Bill.Cheat`/`Bill.Debug`/`Bill.Analytics` in release builds.
- `BillBootstrapConfig.enableTracing` now defaults to `false`. Tracing records a stack trace on every
  `Bill.X` access; turn it on only while debugging service wiring.
- Runtime asmdef `rootNamespace` set to `BillGameCore`.
- `package.json` declares its `com.unity.ugui` dependency, samples, documentation and license.
- BillFav and BillSceneSwitcher moved out to their own repositories.

### Added
- EditMode tests for tween and sequence timing (`Tests/Editor`).

## [3.0.0]

Last version distributed as a folder inside a game project.
