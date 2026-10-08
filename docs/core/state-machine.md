# Game state machine

`Bill.State` is a `GameStateMachine` holding the high-level mode of the game. Six states are
registered at boot:

| State | Built-in behaviour |
|---|---|
| `BootState` | entered when the bootstrap finishes |
| `MenuState` | – |
| `LoadingState` | – |
| `GameplayState` | – |
| `PauseState` | `Time.timeScale = 0` on enter, `1` on exit |
| `GameOverState` | – |

```csharp
Bill.State.GoTo<GameplayState>();
Bill.State.GoBack();                         // to the previous state
bool playing = Bill.State.IsInState<GameplayState>();
```

## React to transitions

```csharp
Bill.State.OnEnter<GameOverState>(ShowResults);
Bill.State.OnExit<GameplayState>(StopSpawning);
Bill.State.OnTransition((from, to) => Debug.Log($"{from?.Name} -> {to.Name}"));
```

Every transition also fires `StateChangedEvent { From, To }` on `Bill.Events`.

## Your own states

```csharp
public class ShopState : GameState
{
    public override void Enter() => Bill.UI.Open<ShopPanel>();
    public override void Tick(float dt) { }
    public override void Exit() => Bill.UI.Close<ShopPanel>();
}

Bill.State.AddState<ShopState>();            // or AddState(new ShopState(...))
Bill.State.GoTo<ShopState>();
```

`GetState<T>()` returns the registered instance, or `null` if that state was never added. Use it to
check before going to a state that only some scenes register.

## History

`Bill.State.History` holds recent transitions (`From`, `To`, `Timestamp`, `Frame`);
`Bill.State.GetHistoryLog()` formats them. The cheat console command `states` prints the same.
