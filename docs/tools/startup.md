# Startup screen

`BillStartup` is a splash screen for the bootstrap scene: a logo scales in, a progress bar fills while
your loading steps run, then the screen fades and loads the next scene.

## Set up

1. In the bootstrap scene, build a Canvas with an `Image` (logo), a `Slider`, a `TMP_Text` and a
   `CanvasGroup` on the root.
2. Add `BillStartup` and drag those into **Logo**, **Progress Slider**, **Status Text**,
   **Root Canvas Group**.
3. Set **Next Scene**.

| Field | Default | |
|---|---|---|
| Next Scene | empty | loaded when every step is done |
| Transition Duration | 0.5 | scene transition |
| Logo Start Scale | 0.3 | logo starts this small and scales to its authored size |
| Logo Scale Duration | 0.8 | |
| Logo Hold Duration | 1 | minimum time on screen |
| Fade Out Duration | 0.5 | |

## Add loading steps

```csharp
var startup = FindAnyObjectByType<BillStartup>();

startup.AddStep("Load settings", () => { Settings.Load(); return true; });

startup.AddStepAsync("Download config", LoadRemote);

IEnumerator LoadRemote(Action<string> log)
{
    log("Contacting server…");
    yield return Remote.Fetch();
    log("Done");
}
```

A synchronous step returns `true` on success. Async steps receive a `log` callback that updates the
status text.
