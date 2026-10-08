# Installation

## Requirements

- Unity **6000.0** or newer
- `com.unity.ugui` 2.0 (Unity 6 ships TextMeshPro inside it; Package Manager adds it automatically)

## Add the package

=== "Package Manager"

    1. **Window ▸ Package Manager**
    2. **+** ▸ **Add package from git URL…**
    3. Paste:

    ```text
    https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.gamecore#v3.1.0
    ```

    Optional, for the attribute inspector:

    ```text
    https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.inspector#v3.1.0
    ```

=== "manifest.json"

    ```json
    {
      "dependencies": {
        "com.bill.gamecore": "https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.gamecore#v3.1.0",
        "com.bill.inspector": "https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.inspector#v3.1.0"
      }
    }
    ```

=== "Local clone"

    Clone the repository next to your project and point the manifest at the folders:

    ```json
    "com.bill.gamecore": "file:../../BillGameCore/Packages/com.bill.gamecore"
    ```

    Use this when you change the framework and want the game to pick the edits up immediately.

!!! tip "Pin a version"
    The `#v3.1.0` suffix pins a tag. Without it Unity follows `main`, and every
    **Update** in Package Manager pulls whatever is there.

## Create the config

BillGameCore loads `Resources/BillBootstrapConfig` at startup. Without it the bootstrap logs
`[Bill] BillBootstrapConfig missing from Resources/` and nothing starts.

**BillGameCore ▸ Setup Project** (++ctrl+shift+g++) creates:

- `Assets/Resources/BillBootstrapConfig.asset`
- `Assets/Resources/Pools/` and `Assets/Resources/Configs/`
- `Assets/_Game/Scenes/00_Bootstrap.unity`, placed at build index 0

You can also create the config by hand: **Create ▸ BillGameCore ▸ Bootstrap Config**, saved as
`Assets/Resources/BillBootstrapConfig.asset`.

## Import samples (optional)

Package Manager ▸ BillGameCore ▸ **Samples**:

| Sample | Shows |
|---|---|
| Quick Start | a minimal game wired through `Bill.*` |
| Tween Demo | every BillTween shortcut, all 31 eases, sequences; after import run **BillGameCore ▸ Tools ▸ Create Tween Demo Scene** |

## Run the tests (optional)

Add the package to `testables` in `Packages/manifest.json`, then open **Window ▸ General ▸ Test Runner**:

```json
"testables": ["com.bill.gamecore", "com.bill.inspector"]
```

Next: [Quick start](quick-start.md).
