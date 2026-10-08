# BillGameCore

Code-first game framework for **Unity 6**. Drop it in, and a bootstrap runs before your first scene:
services are registered, an event bus is ready, and `Bill.Tween`, `Bill.Pool`, `Bill.Audio`,
`Bill.Scene`, `Bill.Timer`, `Bill.Save` and a game state machine are one line away.

**Documentation:** https://billtruong003.github.io/BillGameCore/

## Packages in this repository

| Package | Version | What it is |
|---|---|---|
| [`com.bill.gamecore`](Packages/com.bill.gamecore) | 3.1.0 | Bootstrap, service locator, EventBus, BillTween, pooling, audio, scenes, state machine, timers, save, dev console, virtual joystick |
| [`com.bill.inspector`](Packages/com.bill.inspector) | 0.5.0 | Odin-style attribute inspector: 50+ attributes, expressions, serialization, shader GUI, validation |

Both target Unity **6000.0+**. `com.bill.gamecore` does not require `com.bill.inspector`; install it when
you want the attributes.

## Install

Package Manager → **+** → **Add package from git URL…**

```text
https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.gamecore#v3.1.0
https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.inspector#v3.1.0
```

Or in `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.bill.gamecore": "https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.gamecore#v3.1.0",
    "com.bill.inspector": "https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.inspector#v3.1.0"
  }
}
```

Then run **BillGameCore ▸ Setup Project** (Ctrl+Shift+G) (or create `Resources/BillBootstrapConfig` yourself)
and press Play. See [Quick start](https://billtruong003.github.io/BillGameCore/getting-started/quick-start/).

## 30-second tour

```csharp
using BillGameCore;
using UnityEngine;

public struct EnemyKilled : IEvent { public int Score; }

public class Enemy : MonoBehaviour
{
    void Die()
    {
        Bill.Events.Fire(new EnemyKilled { Score = 10 });
        Bill.Audio.Play("sfx.enemy.death", transform.position);
        Bill.Pool.Spawn("fx.explosion", transform.position, Quaternion.identity);
        BillTween.Scale(transform, 0f, 0.2f)?.SetEase(EaseType.InBack)
                 .OnComplete(() => gameObject.ReturnToPool());
    }
}
```

## Companion packages

Separate repositories, same author, all installable by git URL:

| Repository | Package | Purpose |
|---|---|---|
| [BillFav](https://github.com/billtruong003/BillFav) | `com.bill.fav` | Favourites panel for the Project window |
| [BillSceneSwitcher](https://github.com/billtruong003/BillSceneSwitcher) | `com.bill.sceneswitcher` | Main-toolbar scene switcher, Play from bootstrap |
| [Bill-SSOutline](https://github.com/billtruong003/Bill-SSOutline) | `com.bill.ss-outline` | Screen-space outline for URP 17 (Render Graph) |
| [BillVAT](https://github.com/billtruong003/BillVAT) | `com.bill.vat` | Vertex Animation Textures for large crowds |
| [stylized-toon-world-kit](https://github.com/billtruong003/stylized-toon-world-kit) | `com.billtruong.stylized-toon-world-kit` | Hand-written HLSL toon shaders for URP 17 |

## Repository layout

```text
Packages/com.bill.gamecore/    the framework (Runtime, Editor, Extras, Tests, Samples~)
Packages/com.bill.inspector/   the attribute inspector
docs/                          documentation site source (MkDocs Material)
```

The old sample project (BillDefense, BillFlappy, BillRunner) lives on the
[`legacy/sample-project`](https://github.com/billtruong003/BillGameCore/tree/legacy/sample-project) branch.

## License

MIT, see [LICENSE](LICENSE).
