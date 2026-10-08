# Dev console & overlay

Compiled in the Editor, in development builds, and in any build with the scripting define
**`BILL_CHEATS`**. In other builds `Bill.Cheat`, `Bill.Debug` and `Bill.Analytics` do not exist, so
wrap calls in the same condition:

```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD || BILL_CHEATS
Bill.Cheat.Register("coins", (int n) => Wallet.Add(n), "Add coins");
#endif
```

## Cheat console (`Bill.Cheat`)

Toggle with the ++grave++ (backquote) key. Type a command and press Enter.

```csharp
Bill.Cheat.Register("god", () => Player.God = !Player.God, "Toggle invincibility");
Bill.Cheat.Register<int>("wave", w => Waves.Jump(w), "Jump to wave N");
Bill.Cheat.Register<string, int>("give", (id, n) => Inventory.Add(id, n), "give <item> <count>");
Bill.Cheat.Execute("wave 5");
```

Arguments are parsed from the typed text: `string`, `int`, `float` (invariant culture), `bool`
(`1`/`true`), and any other `IConvertible` type through `Convert.ChangeType`.

Built-in commands:

| Command | Does |
|---|---|
| `help` | list every command with its arguments |
| `clear` | clear the console output |
| `timescale <float>` | set `Time.timeScale` (0–10) |
| `fps <int>` | set the target frame rate |
| `gc` | force a garbage collection |
| `scene <name>`, `reload` | load or reload a scene |
| `pool` | pool statistics |
| `services` | service dependency report |
| `trace`, `health`, `log`, `unused` | [tracing](../core/services.md#tracing) |
| `states` | state machine history |
| `quit` | quit (stops Play Mode in the Editor) |

## Debug overlay (`Bill.Debug`)

A UI Toolkit panel with frame time, FPS, memory, GC, draw calls, triangles, batches, texture and mesh
memory (from Unity's `ProfilerRecorder`).

```csharp
Bill.Debug.Toggle();
Bill.Debug.SetOpen(true);
```

Turn on **Show Overlay On Startup** in the config to open it at boot.

## Analytics tracker (`Bill.Analytics`)

Samples FPS, memory, GC, draw calls and triangles at an interval for profiling sessions.

```csharp
Bill.Analytics.SetInterval(0.5f);
Debug.Log(Bill.Analytics.Summary());
string csv = Bill.Analytics.ExportCSV("run_01");
Bill.Analytics.Clear();
```

!!! note "Input"
    The console's toggle key uses the legacy `Input` class. With **Active Input Handling** set to
    *Input System Package (New)* only, open it from code instead (`Bill.Cheat.SetVisible(true)`)
    or set Active Input Handling to *Both*.
