---
name: display-lyrics-and-playback-information
description: Display lyrics, current or upcoming notes, track names, instruments, and synchronized MIDI information with Maestro MPTK. Use for karaoke, learning displays, inspectors, or playback HUDs.
metadata:
  maestro-skill-version: "1.1.1"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Display Lyrics and Playback Information

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Use live callbacks for current information and preloaded events for upcoming information.

## Live display

In `OnEventNotesMidi`, filter `MPTKCommand.MetaEvent`. Read `MPTKEvent.Meta` and `Info` for `MPTKMeta.Lyric`, `TextEvent`, `SequenceTrackName`, and `TrackInstrumentName`. Read NoteOn events for current notes and PatchChange events for program changes.

```csharp
if (midiEvent.Command == MPTKCommand.MetaEvent &&
    midiEvent.Meta == MPTKMeta.Lyric)
    LyricsLabel.text = midiEvent.Info.Replace("\\", "\n");
```

## Upcoming display

Load before playback, extract the desired event types, sort by tick, and keep a cursor keyed to `MPTK_TickCurrent`. Do not scan the full list every frame.

Use `MPTK_SequenceTrackName`, `MPTK_ProgramName`, `MPTK_TrackInstrumentName`, and `MPTK_TextEvent` only as current loaded-player summaries; event-level display still needs the event list/callback.

MIDI text quality is file-dependent. Handle missing metadata, repeated fragments, slash-separated line breaks, encoding differences, and multiple lyric tracks without assuming karaoke-ready content.

## Verify

Test a controlled file containing lyrics, text, track names, instrument names, tempo changes, and missing fields. Check seek/replay so previous lyric state is cleared and rebuilt correctly.

## Technical depth

- [react-to-midi-events](../react-to-midi-events/SKILL.md)
- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)
