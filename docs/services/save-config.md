# Save & config

## `Bill.Save`

Key-value persistence on top of `PlayerPrefs`, with save slots.

```csharp
Bill.Save.Set("coins", 1200);
Bill.Save.Set("music_on", true);
Bill.Save.Set("profile", profile);            // any [Serializable] class, stored as JSON
Bill.Save.Flush();                            // PlayerPrefs.Save()

int coins = Bill.Save.GetInt("coins");
bool music = Bill.Save.GetBool("music_on", fallback: true);
var p = Bill.Save.Get<Profile>("profile");    // null if missing or unreadable

if (Bill.Save.Has("tutorial_done")) { }
Bill.Save.Delete("tutorial_done");
Bill.Save.SetSlot(1);                         // keys become s1_<key>
```

| Method | |
|---|---|
| `Set(key, string/int/float/bool)` | stored as a string |
| `Set<T>(key, T)` | `JsonUtility` |
| `GetString/GetInt/GetFloat/GetBool(key, fallback)` | fallback when missing or unparsable |
| `Get<T>(key)` | `JsonUtility`, null on failure |
| `Has`, `Delete` | |
| `SetSlot(n)` | prefixes every key with `s<n>_`; default slot 0 |
| `Flush()` | writes to disk; also called on shutdown |

!!! tip "Bigger save data"
    PlayerPrefs is fine for settings and small progress. For a full player profile, keep one
    versioned class and save it with `Set<T>`, or write your own `ISaveService` (file, cloud) and
    register it in place of the default.

## `Bill.Config`

Read-only tuning values with an optional remote override layer.

1. **Create ▸ BillGameCore ▸ Game Config**, put it in `Resources/Configs/`. Every config asset there is
   loaded at boot.
2. Read values:

```csharp
float speed = Bill.Config.GetFloat("player.speed", 5f);
bool  event = Bill.Config.GetBool("event.halloween");      // "1" or "true"
```

3. Override from a server:

```csharp
Bill.Config.ApplyRemote(new Dictionary<string, string> { ["player.speed"] = "6.5" });
// fires ConfigRefreshedEvent
```

Remote values win over local ones. `Set(key, value)` writes a local value at runtime.
