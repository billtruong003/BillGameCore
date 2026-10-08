# Pool

`Bill.Pool` reuses GameObjects by string key instead of `Instantiate`/`Destroy`.

```csharp
Bill.Pool.Register("fx.hit", hitPrefab, warmCount: 20);

var fx = Bill.Pool.Spawn("fx.hit", point, Quaternion.LookRotation(normal));
Bill.Pool.Return(fx, 1.5f);                     // or fx.ReturnToPool(1.5f)

var bullet = Bill.Pool.Spawn<Bullet>("bullet", muzzle.position, muzzle.rotation);
```

## Where pools come from

1. `Bill.Pool.Register(key, prefab, warmCount = 5)` at runtime.
2. **Default Pools** on `BillBootstrapConfig`: key, prefab, warm count, max size, auto return time.
3. Fallback: `Spawn` with an unknown key loads `Resources/Pools/<key>` and registers it. If that fails
   it logs `[Bill.Pool] '<key>' not found` and returns `null`.

## API

| Method | Notes |
|---|---|
| `Spawn(key)`, `Spawn(key, pos, rot)`, `Spawn(key, parent)`, `Spawn(key, pos, rot, parent)` | activates a pooled instance (creates one if the pool is empty) |
| `Spawn<T>(key, ...)` | same, returns `GetComponent<T>()` |
| `Return(go)` / `Return(go, delay)` | deactivates and stores it; an object that did not come from the pool is destroyed |
| `ReturnAll(key)` / `ReturnAll()` | return every active instance |
| `WarmUp(key, count)` | create inactive instances ahead of time |
| `GetPooledCount(key)`, `GetActiveCount(key)`, `GetStats()` | numbers for debugging |

Extension methods: `go.ReturnToPool()`, `go.ReturnToPool(delay)`, `component.ReturnToPool()`.

## Spawn and return hooks

Derive from `PooledObject` to reset state when an instance is reused:

```csharp
public class Bullet : PooledObject
{
    public override void OnSpawnedFromPool() { _life = 2f; _trail.Clear(); }
    public override void OnReturnedToPool()  { _rb.linearVelocity = Vector3.zero; }
}
```

## Limits

- **Max size** (pool definition): returning an instance when the pool already holds that many destroys
  it instead.
- **Auto return time**: every spawn is returned automatically after that many seconds.
