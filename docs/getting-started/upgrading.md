# Upgrading from 3.0

Version 3.0 was copied into `Assets/` (often `Assets/ThirdParty/BillGameCore`). 3.1 is a UPM package.
Script GUIDs are unchanged, so scenes, prefabs and ScriptableObjects keep their references.

## Steps

1. Close Play Mode. Commit or back up the project.
2. Delete the old folder from `Assets/` (including `BillInspector`, `BillFav`, `BillSceneSwitcher` if
   they were inside it). Keep your own `Resources/BillBootstrapConfig.asset`.
3. Add the packages by git URL ([Installation](installation.md)). Add
   [BillFav](https://github.com/billtruong003/BillFav) and
   [BillSceneSwitcher](https://github.com/billtruong003/BillSceneSwitcher) if you used them.
4. Apply the breaking changes below, then let Unity recompile.

## Breaking changes

| 3.0 | 3.1 | What to do |
|---|---|---|
| define `ZW_CHEATS` kept dev tools in release builds | define `BILL_CHEATS` | rename the define in Player Settings or your build script |
| `BillGameCore.Runtime` referenced `BillInspector.Runtime` | no reference | if your asmdef relied on getting BillInspector through BillGameCore, reference `BillInspector.Runtime` yourself |
| `DynamicAnimationEventHub` in the global namespace, part of the runtime assembly | `BillGameCore.DynamicAnimationEventHub` in `BillGameCore.Extras`, only with `com.bill.inspector` installed | add `using BillGameCore;`; install BillInspector if you use it |
| `enableTracing` defaulted to `true` | defaults to `false` | existing config assets keep their saved value; check it |
| BillFav / BillSceneSwitcher namespaces `BillGameCore.BillFav`, `BillGameCore.BillSceneSwitcher` | `BillFav`, `BillSceneSwitcher` | only matters if your code called them |
| `Samples/` compiled into `Assembly-CSharp` | `Samples~/`, imported on demand | re-import from Package Manager if you used them |

## Behaviour changes

**Sequences now time correctly.** In 3.0 a tween added to a sequence also ran on its own, so it played
at double speed, and `Join` steps waited for the step before them. If you tuned durations by eye to
compensate, they will now run at the speed you wrote. `BillTween.Move`, `LocalMove` and `ScaleTo`
move all three axes together instead of X first.

**`KillTarget` reaches sequences.** Killing a target also stops any sequence holding a tween with that
target.
