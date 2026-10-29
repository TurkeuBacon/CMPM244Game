---
name: synchronize-with-midi-time
description: Synchronize Unity logic with Maestro MIDI ticks, event times, tempo changes, and look-ahead data. Use for rhythm mechanics, animation cues, anticipation, scoring windows, or any logic that must stay aligned with musical playback.
metadata:
  maestro-skill-version: "1.1.1"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Synchronize with MIDI Time

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Choose the time domain that matches the feature.

- Use `MPTKEvent.Tick` for beat-relative structure and comparisons that must remain stable when tempo or speed changes.
- Use `MPTKEvent.RealTime` for the event's calculated source-sequence time in milliseconds.
- Use `MPTK_TickCurrent` for the current musical position and `MPTK_Position` for the current playback position in milliseconds.
- Use `MPTK_DeltaTicksPerQuarterNote` to convert beat distances into ticks.

React to due events through `OnEventNotesMidi`. For anticipation, load the MIDI before play and schedule visual/game objects from future events rather than waiting for their audible callback.

```csharp
MidiLoad loaded = Player.MPTK_Load();
foreach (MPTKEvent midiEvent in loaded.MPTK_MidiEvents)
{
    if (midiEvent.Command != MPTKCommand.NoteOn || midiEvent.Velocity == 0)
        continue;

    // Build lightweight gameplay data here from Tick/RealTime.
}
Player.MPTK_Play(alreadyLoaded: true);
```

When `MPTK_Speed` can change, do not compare unadjusted wall-clock time directly with `RealTime`. Prefer the player's current tick/position or reschedule from the changed transport state. Treat Unity frame time as presentation time, not as the authoritative musical clock.

Tempo changes are already reflected in loaded `RealTime` values when tempo processing is enabled. Ticks remain the better representation for measures, beats, quantization, and score windows expressed musically.

## Look-ahead pattern

Maintain a sorted index into preloaded events. Each frame, compare the next event against `MPTK_TickCurrent` plus a look-ahead window, instantiate only once, and calculate its remaining visual travel from the authoritative player position.

## Verify

Test at multiple frame rates, with a MIDI containing tempo changes, after pause/resume, after seek, and after changing `MPTK_Speed`. Log player tick beside the gameplay event tick when diagnosing drift.
