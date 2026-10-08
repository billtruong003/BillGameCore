# Scenes

`Bill.Scene` wraps `SceneManager` with transitions, additive scenes and events.

## Single scenes

```csharp
Bill.Scene.Load("Level_01");
Bill.Scene.Load("Level_01", TransitionType.Fade, 0.6f);
Bill.Scene.Load("Level_01", TransitionType.Fade, 0.6f, EaseType.InOutSine);
Bill.Scene.Load(2);                              // build index
Bill.Scene.Reload();
Bill.Scene.LoadNext();
Bill.Scene.LoadPrevious();
```

`TransitionType`: `None`, `Fade`, `CrossFade`. Half the duration fades out, the scene loads, the other
half fades in. The overlay is created by the service; you do not need a canvas.

## Async with progress

```csharp
Bill.Scene.LoadAsync("World", p => bar.value = p, () => Debug.Log("loaded"));

Bill.Scene.LoadWithTransition("World", TransitionType.Fade, 0.8f, EaseType.Linear,
    onProgress: p => bar.value = p,
    onComplete: StartLevel);
```

## Additive scenes

```csharp
Bill.Scene.LoadAdditive("UI_HUD", () => hudReady = true);
Bill.Scene.Unload("UI_HUD");
Bill.Scene.UnloadAllAdditive();
bool open = Bill.Scene.IsAdditiveLoaded("UI_HUD");
IReadOnlyList<string> all = Bill.Scene.LoadedAdditiveScenes;
```

## State

| Property | |
|---|---|
| `CurrentSceneName`, `CurrentBuildIndex` | the active scene |
| `IsLoading` | a load is in progress (a second `Load` while loading is ignored) |

## Events

`SceneLoadStartEvent`, `SceneLoadProgressEvent { SceneName, Progress }`,
`SceneLoadCompleteEvent` are fired on `Bill.Events`.
