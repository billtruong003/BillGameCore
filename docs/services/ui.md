# UI panels

`Bill.UI` manages **UI Toolkit** panels in one `UIDocument` created at boot (1920×1080 reference,
match 0.5). It is optional; uGUI projects can ignore it.

```csharp
public class PausePanel : BasePanel
{
    protected override void Build(VisualElement root)
    {
        root.Add(new Button(() => Bill.UI.Close<PausePanel>()) { text = "Resume" });
    }

    public override void OnOpened() => Bill.State.GoTo<PauseState>();
    public override void OnClosed() => Bill.State.GoBack();
}

Bill.UI.Open<PausePanel>();
Bill.UI.Open<ShopPanel>(p => p.SetTab("weapons"));
Bill.UI.Toggle<PausePanel>();
Bill.UI.Close<PausePanel>();
Bill.UI.CloseAll();
bool blocking = Bill.UI.AnyOpen();
```

Panels are created on first `Open` and kept. `Build(root)` runs once; `root` fills the screen.
`OnOpened`/`OnClosed` run on every show/hide. `IsOpen<T>()` checks one panel.
