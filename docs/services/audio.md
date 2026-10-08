# Audio

`Bill.Audio` plays sounds by key from an `AudioLibrary`. It runs a pooled set of `AudioSource` voices,
a music bus with fades, channel volumes and ducking.

## Set up a library

1. **Create ▸ BillGameCore ▸ Audio Library**.
2. Add entries: `key`, `clip`, `volume`, `pitch`, `pitchVariation` (random ± per play), `loop`.
3. Assign it to **Default Audio Library** on `BillBootstrapConfig`.

A dotted naming scheme keeps large libraries readable: `sfx.weapon.rifle.fire`,
`ui.button.click`, `music.menu`, `amb.forest.loop`.

An unknown key logs `[Bill.Audio] Key '...' not found.` and plays nothing.

`AudioLibrary.ReplaceEntries(entries)` swaps the content at runtime, for example after loading clips
from Addressables.

## Play

```csharp
Bill.Audio.Play("ui.button.click");
Bill.Audio.Play("sfx.explosion", transform.position);        // 3D at a point
Bill.Audio.Play("sfx.coin", 0.6f);                            // volume
Bill.Audio.PlayPitched("sfx.hit", pitchMultiplier: 1.2f);     // on top of the entry's pitch
float length = Bill.Audio.PlayCue("vo.intro", SfxPriority.High); // returns the clip length
```

## Music

```csharp
Bill.Audio.PlayMusic("music.battle", fadeDuration: 1f);
Bill.Audio.StopMusic(0.5f);
string now = Bill.Audio.CurrentMusicKey;     // "" when stopped
```

## Volume

Channels: `Master`, `Music`, `SFX`, `UI`, `Voice`.

```csharp
Bill.Audio.SetVolume(AudioChannel.Music, 0.5f);
Bill.Audio.Mute(AudioChannel.SFX);
Bill.Audio.Unmute(AudioChannel.SFX);
```

## Voices, priority and ducking

When all voices are busy, lower-priority sounds are dropped first.

| Type | Values |
|---|---|
| `SfxPriority` | `Low` (footsteps, ambience), `Medium` (impacts, enemy attacks), `High` (player weapons, explosions), `Critical` (victory, defeat; always gets a voice) |
| `CueGroup` | duck bus: `Critical` (stingers, never ducked by combat/world), `Combat`, `World` |
| `VoicePolicy` | `maxVoices`, `perKeyLimit` (simultaneous voices per key); set with `ConfigureVoices` |

```csharp
int duck = Bill.Audio.Duck(AudioChannel.Music, amount: 0.6f, attack: 0.1f);
// ... dialogue plays ...
Bill.Audio.Unduck(duck, release: 0.4f);

int g = Bill.Audio.DuckGroup(CueGroup.Combat, 0.5f, 0.05f);     // also affects voices already playing
```

Overlapping ducks are resolved by depth: releasing one never restores the volume while another is
still held.

## Stoppable and repeated cues

| Method | Use |
|---|---|
| `PlayManagedCue(key, priority, group)` | returns an `AudioCueHandle` you can `StopCue(handle, fadeOut)` |
| `RetriggerCue(key, handle, priority)` | restart on the same voice at your own cadence (automatic weapons) |
| `PlayGuaranteedTransient(key, priority, maxSameKeyVoices)` | never dropped; recycles that key's oldest voice when its budget is full |
| `IsCueAlive(handle)` | is the voice still playing this cue |

Handles carry a version, so a handle to a recycled voice simply reports not alive.
