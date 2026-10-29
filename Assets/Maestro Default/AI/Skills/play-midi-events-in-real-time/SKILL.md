---
name: play-midi-events-in-real-time
description: Play and stop notes or MIDI commands generated at runtime with Maestro MPTK. Use for virtual instruments, procedural music, UI keyboards, percussion pads, or live interaction.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Play MIDI Events in Real Time

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Use a `MidiStreamPlayer`. Create an `MPTKEvent` for each command and retain the same NoteOn instance when a sustained note must later be stopped.

```csharp
MPTKEvent heldNote;

void NoteDown()
{
    heldNote = new MPTKEvent
    {
        Command = MPTKCommand.NoteOn,
        Channel = 0,
        Value = 60,
        Velocity = 100,
        Duration = -1
    };
    stream.MPTK_PlayEvent(heldNote);
}

void NoteUp()
{
    if (heldNote != null)
        stream.MPTK_StopEvent(heldNote);
}
```

For a finite note, set `Duration` in milliseconds. For an indefinite note, use `Duration = -1` and always provide a release path for input-up, cancellation, scene disable, and application shutdown.

`MPTK_PlayEvent(List<MPTKEvent>)` can start a group together. `MPTK_PlayDirectEvent` and `MPTK_StopDirectEvent` are lower-level synth methods; prefer the stream-player pair for ordinary application code.

Other commands use the same event model. For example, send `MPTKCommand.PatchChange` with the preset number in `Value`, or use the clearer channel API when controlling a player's persistent channel state.

## Verify

Test repeated press/release, overlapping notes of the same pitch, focus loss, object disable, and high input rates. Confirm that the active-voice count returns to baseline after all notes are released.

## Go deeper

- For instruments and banks, read [control-midi-channels-and-instruments](../control-midi-channels-and-instruments/SKILL.md).
- **Requires Pro:** for Maestro's chord and scale helpers, read [generate-chords-scales-and-progressions](../generate-chords-scales-and-progressions/SKILL.md). Free can play an application-constructed list of notes.
