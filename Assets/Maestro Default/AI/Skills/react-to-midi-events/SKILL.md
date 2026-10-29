---
name: react-to-midi-events
description: React to notes, controllers, program changes, lyrics, and other MIDI events during Maestro playback. Use when MIDI data drives gameplay, UI, visuals, learning feedback, or synchronized application logic.
metadata:
  maestro-skill-version: "1.1.1"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# React to MIDI Events

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Subscribe to `MidiFilePlayer.OnEventNotesMidi`. Maestro passes a list because all events in one callback share the same MIDI tick. The callback is safe for ordinary Unity object interaction.

```csharp
using System.Collections.Generic;
using MidiPlayerTK;
using UnityEngine;

public class NoteReaction : MonoBehaviour
{
    public MidiFilePlayer Player;

    void OnEnable() => Player.OnEventNotesMidi.AddListener(OnMidiEvents);
    void OnDisable() => Player.OnEventNotesMidi.RemoveListener(OnMidiEvents);

    void OnMidiEvents(List<MPTKEvent> eventsAtTick)
    {
        foreach (MPTKEvent midiEvent in eventsAtTick)
            if (midiEvent.Command == MPTKCommand.NoteOn && midiEvent.Velocity > 0)
                Debug.Log($"note={midiEvent.Value} channel={midiEvent.Channel} track={midiEvent.Track}");
    }
}
```

Filter by `Command` first, then inspect fields appropriate to that command:

- NoteOn: `Value`, `Velocity`, `Duration`, `Channel`, `Track`.
- ControlChange: `Controller`, `Value`, `Channel`.
- PatchChange: `Value`, `Channel`.
- MetaEvent: `Meta`, `Info`, and timing fields.

The callback is observational: changing note pitch or velocity inside it does not modify the note already being played. To transform music, preprocess/edit events or generate a new sequence.

## Event lifecycle

- `OnEventStartPlayMidi` supplies the MIDI name and is the correct place for start-position setup.
- `OnEventNotesMidi` supplies event groups during playback.
- `OnEventEndPlayMidi` supplies the MIDI name and end reason.
- Explicit file NoteOff events require `MPTK_KeepNoteOff = true` before loading; otherwise use NoteOn duration when planning release behavior.

## Verify

Use a controlled MIDI containing notes, program changes, controller changes, lyrics, and tempo events. Confirm filters do not treat every event as a note.
