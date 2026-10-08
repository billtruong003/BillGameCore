# Quick start

This page takes an empty Unity 6 project to a scene where a cube spawns from a pool, tweens in, plays a
sound, and reports through an event. About ten minutes.

## 1. Bootstrap scene

After [installation](installation.md), run **BillGameCore ▸ Setup Project**. You now have
`00_Bootstrap.unity` at build index 0 and a config in `Resources/`.

Add your game scene to **File ▸ Build Profiles ▸ Scene List** after the bootstrap scene, then set
**Default Game Scene** on the config to its name.

!!! note "Play from any scene"
    With **Enforce Bootstrap Scene** on, pressing Play in any scene first loads scene 0, boots the
    framework, and (in the Editor, with **Return To Edit Scene In Editor**) jumps back to the scene you
    were editing. Turn **Enforce Bootstrap Scene** off if your project boots differently.

## 2. Wait for the framework

Scripts in the first scene can run before the bootstrap finishes. Guard them with `Bill.IsReady`:

```csharp
using BillGameCore;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;

    void Start()
    {
        if (!Bill.IsReady) { Bill.Events.SubscribeOnce<GameReadyEvent>(_ => Init()); return; }
        Init();
    }

    void Init()
    {
        Bill.Pool.Register("cube", cubePrefab, warmCount: 10);
        Bill.Timer.Repeat(1f, SpawnOne);
    }

    void SpawnOne()
    {
        var pos = new Vector3(Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
        var cube = Bill.Pool.Spawn("cube", pos, Quaternion.identity);
        cube.transform.localScale = Vector3.zero;
        BillTween.Scale(cube.transform, 1f, 0.35f)?.SetEase(EaseType.OutBack);
        Bill.Audio.Play("sfx.spawn", pos);
        Bill.Events.Fire(new CubeSpawned { Position = pos });
        cube.ReturnToPool(3f);
    }
}

public struct CubeSpawned : IEvent { public Vector3 Position; }
```

## 3. Listen for the event

```csharp
public class SpawnCounter : MonoBehaviour
{
    int _count;

    void OnEnable()  => Bill.Events.Subscribe<CubeSpawned>(OnSpawned);
    void OnDisable() => Bill.Events.Unsubscribe<CubeSpawned>(OnSpawned);

    void OnSpawned(CubeSpawned e) => Debug.Log($"Spawned #{++_count} at {e.Position}");
}
```

!!! warning "Always unsubscribe"
    Event channels are static and outlive scenes. A destroyed component that is still subscribed
    keeps receiving events, and Unity throws when it touches `this`.

## 4. Sound

Create **Create ▸ BillGameCore ▸ Audio Library**, add an entry with key `sfx.spawn`, and assign the
library to **Default Audio Library** on the config. `Bill.Audio.Play` with an unknown key logs a warning
and plays nothing.

## 5. Check the wiring

Open the cheat console with the ++grave++ key (Editor and development builds) and type `trace`, or call:

```csharp
Bill.Trace.HealthCheck();   // every service and whether it initialised
Bill.Trace.Unused();        // services nobody has called yet
```

That is the whole loop. Read [Bootstrap & config](../core/bootstrap.md) next to see what runs and in
which order.
