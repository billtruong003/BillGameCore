# Networking (Photon Fusion)

`Bill.Net` is always registered. Without Photon Fusion it is an **offline adapter**: `IsOffline` and
`IsHost` are `true`, `PlayerCount` is 1, and `CreateRoom`/`JoinRoom`/`LeaveRoom` call their success
callback immediately. Single-player games can ignore it.

## Enable Fusion

1. Import Photon Fusion into the project.
2. Add the scripting define **`PHOTON_FUSION`**. The code under `Runtime/Network/Fusion` compiles only
   with it.
3. Pick **Default Network Mode** on the config: `FusionHost`, `FusionShared`, `FusionClient` or
   `FusionAutoHostOrClient`.

```csharp
Bill.Net.CreateRoom("room-42", maxPlayers: 4, onSuccess: StartMatch, onFail: Debug.LogError);
Bill.Net.JoinRoom("room-42", StartMatch, Debug.LogError);
Bill.Net.LeaveRoom(() => Bill.Scene.Load("Menu"));
```

`Bill.Net.Cycle.Phase` follows `Disconnected → Connecting → InLobby → InRoom → Playing →
Disconnecting`; each change fires `NetworkPhaseChangedEvent`.

## Fusion extras (with `PHOTON_FUSION`)

| API | |
|---|---|
| `Bill.Players.Local`, `.All`, `.Get(PlayerRef)` | registry of players implementing `IBillPlayer` |
| `Bill.Players.PlayerRegistered / PlayerUnregistered` | events |
| `NetworkPoolable`, `PooledNetworkObjectProvider` | network objects through `Bill.Pool` |
| `FusionStartedEvent`, `FusionPlayerJoinedEvent`, `FusionPlayerLeftEvent`, `FusionShutdownEvent`, … | on `Bill.Events` |

!!! note
    The Fusion adapter is maintained against Fusion 2 but is not covered by this package's tests.
