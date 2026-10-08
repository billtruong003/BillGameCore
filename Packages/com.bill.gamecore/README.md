# BillGameCore (`com.bill.gamecore`)

Code-first Unity 6 game framework: auto-bootstrap, service locator with tracing, EventBus, BillTween,
object pooling, audio, scene flow, game state machine, timers, save, a dev console and a virtual
joystick.

Full documentation: https://billtruong003.github.io/BillGameCore/

## Install

```text
https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.gamecore#v3.1.0
```

Requires Unity 6000.0+ and `com.unity.ugui` 2.0 (TextMeshPro is part of it).

## Minimum setup

1. Create `Assets/Resources/BillBootstrapConfig.asset` (**BillGameCore ▸ Setup Project** does it).
2. Put a bootstrap scene at build index 0.
3. Press Play. The console prints `[Bill] Ready. N services in X ms.`

```csharp
void Start()
{
    if (!Bill.IsReady) { Bill.Events.SubscribeOnce<GameReadyEvent>(_ => Init()); return; }
    Init();
}
```

## Contents

| Folder | Assembly | Notes |
|---|---|---|
| `Runtime/` | `BillGameCore.Runtime` | everything you call at runtime |
| `Editor/` | `BillGameCore.Editor` | setup wizard |
| `Extras/` | `BillGameCore.Extras` | `DynamicAnimationEventHub`; compiled only with `com.bill.inspector` |
| `Tests/Editor/` | `BillGameCore.Tests.Editor` | add the package to `testables` to run them |
| `Samples~/` | – | Quick Start, Tween Demo (import from Package Manager) |

## Scripting defines

| Define | Effect |
|---|---|
| `BILL_CHEATS` | keeps `Bill.Cheat`, `Bill.Debug`, `Bill.Analytics` in non-development builds |
| `PHOTON_FUSION` | compiles the Photon Fusion adapter (`Runtime/Network/Fusion`) |

License: MIT.
