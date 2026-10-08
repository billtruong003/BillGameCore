# Changelog

## com.bill.gamecore


### [3.1.0] - 2026-10-08

First release as a standalone UPM package (previously copied into `Assets/`).

#### Fixed
- **BillTween sequences ticked their tweens twice.** A tween created by `BillTween.*` was also left in
  the service's active list after `Append`/`Join`, so it ran at double speed, appended steps started
  immediately, and the service recycled it while the sequence still held it. Sequenced tweens now
  leave the active list and are driven only by their sequence. `Move`, `LocalMove` and `ScaleTo` (and
  `TweenMove`/`TweenLocalMove`/`TweenScaleTo`) were affected every time.
- **`Join` did not run in parallel.** Joined steps only started after the step before them finished.
  A step and the `Join`/`Insert` steps right after it now form a group that starts together; the next
  `Append` waits for the whole group.
- `KillTarget` now also stops sequences that contain a tween with that target.
- An exception inside an event handler is logged with its stack trace (it used to log only the
  message).

#### Changed
- Package layout: `Runtime/`, `Editor/` (own asmdef `BillGameCore.Editor`), `Extras/`, `Tests/`,
  `Samples~/`. Samples no longer compile into `Assembly-CSharp` and no longer ship in player builds.
- `BillGameCore.Runtime` no longer references `BillInspector.Runtime`.
  `DynamicAnimationEventHub` moved to the optional `BillGameCore.Extras` assembly, compiled only
  when `com.bill.inspector` is installed, and now lives in the `BillGameCore` namespace.
- Cheat build define renamed from `ZW_CHEATS` to **`BILL_CHEATS`**. Add `BILL_CHEATS` to Scripting
  Define Symbols to keep `Bill.Cheat`/`Bill.Debug`/`Bill.Analytics` in release builds.
- `BillBootstrapConfig.enableTracing` now defaults to `false`. Tracing records a stack trace on every
  `Bill.X` access; turn it on only while debugging service wiring.
- Runtime asmdef `rootNamespace` set to `BillGameCore`.
- `package.json` declares its `com.unity.ugui` dependency, samples, documentation and license.
- BillFav and BillSceneSwitcher moved out to their own repositories.

#### Added
- EditMode tests for tween and sequence timing (`Tests/Editor`).

### [3.0.0]

Last version distributed as a folder inside a game project.

## com.bill.inspector

### [0.5.0] - 2026-10-08

- Released as a UPM package inside the BillGameCore repository.
- New define `BILL_INSPECTOR_NO_FALLBACK`: skips registering BillInspector as the fallback editor for
  every MonoBehaviour and ScriptableObject, so only types that use Bill attributes through their own
  editors (or `BillEditorWindow`) are affected.
- Removed the empty `BillInspector.Runtime.Tests` assembly (it had no tests).
- `package.json`: author, license, repository and documentation links.

### [0.4.0] — Phase 4 — Expression Engine, EditorWindow, Validation

#### Expression Engine (full rewrite)
- ExpressionCompiler: compiles @expressions into AST node tree, caches per type+expression
- Supports: field access, chained member access (a.b.c), method calls, comparisons
  (>=, <=, ==, !=, >, <), boolean operators (&&, ||), negation (!), ternary (a ? b : c)
- String interpolation: $"text {field:F2}" with format specifiers
- Static type access: Color.red, Vector3.zero, Application.isPlaying, etc.
- ValueResolver: unified resolution for labels, colors, dropdowns, titles

#### BillEditorWindow
- Base class for attribute-driven editor windows (no OnGUI needed)
- Auto-draws fields: string, int, float, bool, Color, Vector2/3, enum, Object
- All group attributes work: BoxGroup, FoldoutGroup, HorizontalGroup, VerticalGroup
- All meta attributes work: ShowIf, ReadOnly, Required, InfoBox, Title
- Buttons with groups and sizing
- State persistence across domain reload

#### Validation System
- BillValidator: validates single objects, batches, scene, project assets
- BillValidationWindow: editor window with search, severity filters, object ping
- New validation attributes: [BillValidate] (self-validation method),
  [BillFileExists], [BillRangeValidation], [BillNotEmpty]
- Menu items: Tools > BillInspector > Validate Scene / Validate Assets
- Cross-field validation via @expressions

#### Utilities
- BillReflectionCache: centralized field/property/method cache, auto-clears on domain reload
- BillEditorUtility: helper methods for dirty marking, type checking
- BillMenuItems: Tools menu with validation, cache clear, about

#### Samples
- 05-EditorWindow: QuestEditorWindow with full attribute-driven UI
- 06-Validation: ValidationDemo with all validation attribute types

### [0.3.0] — Phase 2 + 3

#### Phase 2 — Serialization + Advanced Drawers
- BillSerializer: binary serializer for Dictionary, HashSet, Tuple, nested generics
- BillSerializedMonoBehaviour / BillSerializedScriptableObject base classes
- [BillSerialize] attribute for manual field marking
- Dropdown, MinMaxSlider, ColorPalette, FilePath, AssetSelector drawers
- InlineEditor, TableList, DictionaryDrawer

#### Phase 3 — Shader Editor Framework
- BillShaderGUI: auto-generated URP-quality material inspector
- Naming conventions: _H_ headers, [Toggle] keywords, texture+color combos
- Material property drawers: BillHeader, BillToggle, BillEnum, BillGradient
- Sample toon shader (URP HLSL)

### [0.1.0] — Phase 1

#### Added
- Core attribute system (30+ attributes)
- Drawer pipeline with UI Toolkit and IMGUI
- Condition evaluator, group drawers, property tree
- Button support, basic validation
