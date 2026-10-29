---
name: load-and-inspect-midi-content
description: Load MIDI content and inspect its events, duration, tracks, tempo-related positions, and text with Maestro MPTK. Use before playback, visualization, editing, filtering, or gameplay preprocessing; external URI and runtime-byte routes require Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Load and Inspect MIDI Content

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Select the loading route from where the MIDI lives.

- MIDI DB: set `MidiFilePlayer.MPTK_MidiName`, then call `MPTK_Load()`.
- **Requires Pro:** for a local or remote URI, call `MidiFilePlayer.MPTK_Load(uri)` on a file-player component or use `MidiExternalPlayer` for playback.
- **Requires Pro:** `MidiExternalPlayer.MPTK_Play(byte[])` plays runtime bytes; use the supported loader/writer APIs when the events must be edited first.

Set load-time options such as `MPTK_KeepNoteOff`, `MPTK_KeepEndTrack`, and `MPTK_EnableChangeTempo` before loading.

```csharp
MidiLoad loaded = player.MPTK_Load();
if (loaded == null)
{
    Debug.LogError($"Unable to load '{player.MPTK_MidiName}'.");
    return;
}

foreach (MPTKEvent midiEvent in loaded.MPTK_MidiEvents)
{
    if (midiEvent.Command == MPTKCommand.NoteOn)
        Debug.Log($"tick={midiEvent.Tick} note={midiEvent.Value} durationMs={midiEvent.Duration}");
}
```

After loading, use `MPTK_MidiEvents` or `MPTK_ReadMidiEvents(fromTicks, toTicks)`. Useful event fields include `Command`, `Value`, `Velocity`, `Channel`, `Track`, `Tick`, `RealTime`, `Duration`, `Length`, `Meta`, and `Info`.

Do not infer musical meaning from NoteOn alone. Program changes, bank selection, tempo, time signatures, sustain controls, lyrics, and track names can affect the correct interpretation.

## Important distinctions

- `Tick` and `Length` are musical positions independent of playback speed.
- `RealTime` and `Duration` are calculated milliseconds; `RealTime` accounts for MIDI tempo changes but not `MPTK_Speed`.
- By default, Maestro commonly represents a file note as one NoteOn with a duration. Enable `MPTK_KeepNoteOff` before load only when explicit NoteOff events are needed.

## Verify

Log the event count, `MPTK_DurationMS`, `MPTK_TickLast`, `MPTK_TrackCount`, and several known events from a controlled test MIDI.
