---
name: generate-music-at-runtime
description: Generate melodies, rhythms, and procedural music at runtime with Maestro MPTK. Use when musical content is created from code rather than loaded unchanged from a MIDI file; scheduled sequences, helper-generated harmony, and export require Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Generate Music at Runtime

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Choose the output model before generating notes:

- Immediate/live generation in Free or Pro: send finite or retained `MPTKEvent` objects to `MidiStreamPlayer`.
- **Requires Pro:** for a scheduled, editable, or exportable sequence, build an `MPTKWriter` timeline.

For immediate generation, let the application schedule each finite event and retain indefinite notes until release:

```csharp
foreach (int note in generatedNotes)
{
    stream.MPTK_PlayEvent(new MPTKEvent
    {
        Command = MPTKCommand.NoteOn,
        Channel = 0,
        Value = note,
        Velocity = 96,
        Duration = 400
    });
}
```

Keep the random seed and generator state when the result must be replayable or testable. Constrain generated pitches, velocities, channels, durations, and density to valid ranges.

**Requires Pro:** use `MPTKWriter` and musical ticks for patterns that must form a reusable tempo-aware timeline. Add tempo, time signature, bank, and preset events when the sequence must be self-contained, then stable-sort and calculate timing before playback or export.

**Requires Pro:** use Maestro scale, chord, and progression helpers for tonal structure. In either edition, keep compositional decisions such as form, voicing, rhythm, orchestration, repetition, and transition logic in the application.

## Verify

Generate from a fixed seed, inspect event order and last tick, audition the result, and test extreme generator inputs. Stop/cancel old live events when regenerating.

## Technical depth

- [play-midi-events-in-real-time](../play-midi-events-in-real-time/SKILL.md)
- [generate-chords-scales-and-progressions](../generate-chords-scales-and-progressions/SKILL.md) — requires Pro
- [create-edit-and-export-midi](../create-edit-and-export-midi/SKILL.md) — requires Pro
