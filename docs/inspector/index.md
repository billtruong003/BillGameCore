# BillInspector

`com.bill.inspector` is an attribute-driven inspector in the style of Odin: 50+ attributes, a small
C# expression language, serialization for dictionaries and sets, a shader GUI and attribute-built
editor windows. It is independent of `com.bill.gamecore`.

## Install

```text
https://github.com/billtruong003/BillGameCore.git?path=/Packages/com.bill.inspector#v3.1.0
```

## Example

```csharp
using BillInspector;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [BillTitle("Turret")]
    [BillRequired] public GameObject bulletPrefab;

    [BillBoxGroup("Stats"), BillSlider(0, 100)] public float range = 20f;
    [BillBoxGroup("Stats"), BillProgressBar(0, 100, ColorType.Red)] public float health = 100f;

    [BillShowIf("@health < 30")]
    [BillInfoBox("Health is critically low!", InfoType.Error)]
    public bool showWarning;

    [BillButton("Heal")] void Heal() => health = 100f;
}
```

## Attributes

| Group | Attributes |
|---|---|
| Groups | `BillBoxGroup`, `BillFoldoutGroup`, `BillTabGroup`, `BillHorizontalGroup`, `BillVerticalGroup`, `BillToggleGroup` |
| Conditions | `BillShowIf`, `BillHideIf`, `BillEnableIf`, `BillDisableIf`, `BillHideInPlayMode`, `BillReadOnly` |
| Drawers | `BillSlider`, `BillMinMaxSlider`, `BillProgressBar`, `BillDropdown`, `BillEnumToggleButtons`, `BillTableList`, `BillTableColumnWidth`, `BillListDrawerSettings`, `BillInlineEditor`, `BillPreviewField`, `BillColorPalette`, `BillFilePath`, `BillAssetSelector`, `BillResizableTextArea`, `BillSearchable`, `BillDictionaryDrawer` |
| Display | `BillTitle`, `BillLabelText`, `BillHideLabel`, `BillSuffix`, `BillIndent`, `BillGUIColor`, `BillPropertyOrder`, `BillShowInInspector`, `BillInfoBox` |
| Object filters | `BillAssetsOnly`, `BillSceneObjectsOnly` |
| Buttons | `BillButton`, `BillButtonGroup`, `BillShowResultAs` |
| Validation | `BillRequired`, `BillValidateInput`, `BillOnValueChanged` |

## Expressions

Prefix `@` for a C# expression, `$"…"` for interpolation:

```csharp
[BillShowIf("@level >= 5 && !isDead")]
[BillLabelText("@$\"HP ({health:F0}/{maxHealth:F0})\"")]
[BillGUIColor("@health < 30 ? Color.red : Color.white")]
```

Supported: fields, properties, chained members (`a.b.c`), method calls, comparisons, `&&`, `||`, `!`,
ternary, and static members such as `Color.red` or `Application.isPlaying`. Expressions are compiled
once per type and cached.

## Serialization

Derive from `BillSerializedMonoBehaviour` or `BillSerializedScriptableObject` and mark fields with
`[BillSerialize]` to serialize `Dictionary<,>`, `HashSet<>` and tuples.

## Shader GUI

```hlsl
CustomEditor "BillShaderGUI"
```

Conventions: a property named `_H_Section` draws a header, `[Toggle] _UseX` draws a toggle bound to a
keyword, and a texture followed by its colour draws on one line.

## Validation

**Tools ▸ BillInspector ▸ Validation Window** scans the open scene for `BillRequired` and
`BillValidateInput` failures. From code: `BillValidator.ValidateScene()`.

## Editor windows

Derive from `BillEditorWindow`, declare fields with attributes, and the window draws itself. No
`OnGUI`.

## Fallback editor

By default BillInspector registers itself as the fallback editor for every `MonoBehaviour` and
`ScriptableObject` without a custom editor, so attributes work everywhere. To keep Unity's default
inspector, add the scripting define **`BILL_INSPECTOR_NO_FALLBACK`**.

Full changelog: [com.bill.inspector/CHANGELOG.md](https://github.com/billtruong003/BillGameCore/blob/main/Packages/com.bill.inspector/CHANGELOG.md).
