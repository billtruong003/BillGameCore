# Virtual joystick

`BillVirtualJoystick` is an on-screen stick for mobile, built on uGUI pointer events. It has no
dependency on the rest of the framework.

## Set up

1. Under a Canvas, create a `RectTransform` that covers the touch area and add `BillVirtualJoystick`.
2. Add a child **Background** image and a grandchild **Handle** image; assign both. Optionally assign
   a **Direction Indicator** that rotates toward the input.

| Field | Default | |
|---|---|---|
| Mode | Floating | `Fixed`: stays where you placed it. `Floating`: re-centres on the first touch inside the area |
| Active Axes | Both | `Both`, `Horizontal`, `Vertical` |
| Dead Zone | 0.08 | input below this length reads as zero; above it, the range is remapped to start at 0 |
| Handle Range | 1 | how far the handle travels, relative to the background radius |
| Keep Handle Inside Background | on | |

## Read input

```csharp
[SerializeField] BillVirtualJoystick stick;

void Update()
{
    Vector2 dir = stick.Direction;          // length 0..1
    if (stick.IsHeld) Move(new Vector3(dir.x, 0f, dir.y));
}
```

Events: `OnValueChanged`, `OnPointerDownEvent`, `OnPointerUpEvent` (all `UnityEvent<Vector2>`).

The stick locks onto the first pointer that touches it, so a second finger elsewhere on screen does
not steal it.
