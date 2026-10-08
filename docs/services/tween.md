# BillTween

A pooled float tween engine. Every tween drives one `float` from `from` to `to` through a setter, with
an ease, a delay, loops and callbacks. Multi-axis shortcuts and sequences are built from single-axis
tweens.

```csharp
BillTween.MoveY(transform, 3f, 1f)?.SetEase(EaseType.OutBack).SetTarget(this);
BillTween.Fade(canvasGroup, 0f, 0.4f)?.SetEase(EaseType.InQuad).OnComplete(Hide);
transform.TweenScale(1.2f, 0.15f)?.SetLoops(1, LoopType.Yoyo);
```

!!! note "Why `?.`"
    `BillTween` returns `null` until the bootstrap has registered `TweenService`. The null-conditional
    keeps code in scene 0 or in Edit Mode from throwing.

## Shortcuts

| Target | Single axis (returns `Tween`) | Multi axis (returns `TweenSequence`) |
|---|---|---|
| `Transform` position | `MoveX/Y/Z`, `LocalMoveX/Y/Z` | `Move`, `LocalMove` |
| `Transform` scale | `ScaleX/Y/Z`, `Scale` (uniform) | `ScaleTo` |
| `Transform` rotation | `RotateZ` | – |
| `CanvasGroup` | `Fade` | – |
| `SpriteRenderer` | `Fade`, `ColorR/G/B` | – |
| `Image` | `Fade`, `FillAmount` | – |
| `Text` (legacy) | `Fade` | – |
| anything | `Float(from, to, dur, setter)`, `To(getter, setter, to, dur)` | – |
| delay | `DelayedCall(seconds, callback)` | – |

Each has an extension-method twin: `transform.TweenMoveY(3f, 1f)`, `canvasGroup.TweenFade(0f, 0.4f)`,
`image.TweenFillAmount(0.3f, 0.2f)`, `gameObject.TweenScale(1f, 0.3f)`.

Shortcuts call `SetTarget(target)` for you, so `BillTween.KillTarget(transform)` stops them.

## Tween options

| Method | Effect |
|---|---|
| `SetEase(EaseType)` | one of 31 eases (below) |
| `SetDelay(seconds)` | wait before starting |
| `SetLoops(count, LoopType)` | `count` extra plays; `-1` = forever. `Restart`, `Yoyo`, `Incremental` |
| `SetUnscaled()` | ignore `Time.timeScale` (pause menus) |
| `SetTarget(object)` | owner for `KillTarget` |
| `OnStart(Action)` | once, when the delay ends |
| `OnUpdate(Action<float>)` | every tick, with normalised time 0..1 |
| `OnComplete(Action)` | when the last loop ends |
| `Kill()` | stop now, no complete callback |
| `Complete()` | jump to the end and fire `OnComplete` |

Eases: `Linear`, and `In`/`Out`/`InOut` versions of `Sine`, `Quad`, `Cubic`, `Quart`, `Quint`, `Expo`,
`Circ`, `Back`, `Elastic`, `Bounce`.

## Sequences

```csharp
BillTween.Sequence()
    ?.Append(BillTween.Fade(group, 1f, 0.2f))
    .Join(BillTween.MoveY(title, 0f, 0.2f))      // starts together with the Fade
    .AppendInterval(0.5f)
    .AppendCallback(() => Bill.Audio.Play("ui.whoosh"))
    .Append(BillTween.Scale(button, 1f, 0.15f)?.SetEase(EaseType.OutBack))
    .OnComplete(() => Debug.Log("intro done"));
```

How steps are timed:

- A step and every `Join`/`Insert` step right after it form a **group**. All tweens in a group start
  on the same frame.
- The next `Append`, `AppendInterval` or `AppendCallback` starts after the **whole group** has
  finished.
- Callbacks run on the frame the previous group ends. A tween step starts on the next frame.
- `Insert(atTime, tween)` is a joined tween with a delay.
- `SetLoops(n)` replays the whole sequence; `SetUnscaled()` applies to every step.
- `OnStepComplete(Action<int>)` reports the index of each finished group's leading step.

Only `Tween` steps can be appended. `Move`, `LocalMove` and `ScaleTo` already return a
`TweenSequence`; run them on their own, or build the three axis tweens yourself and `Join` them.

## Stopping tweens

```csharp
BillTween.KillTarget(transform);   // every tween and sequence with this target
BillTween.Kill(tween);
BillTween.KillAll();
BillTween.CompleteAll();
```

A tween whose target is a destroyed `UnityEngine.Object` stops by itself on its next tick.

!!! warning "Don't keep `Tween` references"
    Tweens are pooled. After a tween completes, the same object can be handed to new code. Holding a
    `Tween` and calling `Kill()` on it later can stop that unrelated tween. Use `SetTarget` +
    `KillTarget` instead, or drop your reference in `OnComplete`.

## Cost

- Tweens come from a pool (32 pre-made, grows as needed). Finished standalone tweens go back to it.
- Each shortcut allocates one setter closure; `Sequence()` allocates the sequence object.
- No per-frame allocation while a tween runs.
