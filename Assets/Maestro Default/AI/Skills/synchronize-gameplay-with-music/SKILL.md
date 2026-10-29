---
name: synchronize-gameplay-with-music
description: Synchronize Unity gameplay, animation, scenes, or logic with music played by Maestro MPTK. Use when musical notes, tempo, tracks, or positions must trigger non-rhythm-game behavior.
metadata:
  maestro-skill-version: "1.1.1"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Synchronize Gameplay with Music

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

First decide whether each cue must be known in advance or only handled when due.

- For immediate reactions, subscribe to `OnEventNotesMidi` and filter the desired note, channel, track, controller, program, or meta event.
- For anticipation, load and inspect the event list before playback, then schedule from `Tick` or `RealTime`.
- For continuous behavior, derive normalized progress from the player's current position and known duration.

```csharp
void OnMidiEvents(System.Collections.Generic.List<MPTKEvent> eventsAtTick)
{
    foreach (MPTKEvent midiEvent in eventsAtTick)
        if (midiEvent.Command == MPTKCommand.NoteOn &&
            midiEvent.Channel == 0 && midiEvent.Value == 60)
            TriggerGameplayAction();
}
```

Keep the mapping from music to game actions in application data rather than scattering note numbers through scene scripts. Include transport resets so seeking or replaying does not leave one-shot actions, animation state, or object pools inconsistent.

Use MIDI because it provides discrete, named events and advance timing. Use rendered-audio analysis instead when the required cue is an acoustic property that the MIDI does not contain.

## Verify

Run the same sequence after restart, seek, pause/resume, a tempo change, and an intentional frame-rate drop. Confirm that each one-shot cue fires once and continuous state reconstructs correctly.

## Technical depth

- [react-to-midi-events](../react-to-midi-events/SKILL.md)
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)
- [control-midi-playback](../control-midi-playback/SKILL.md)
