---
name: create-music-driven-visuals
description: Create MIDI visualizers, piano rolls, falling notes, lighting, VFX, and animated objects with Maestro MPTK. Use when structured musical events should drive visual content.
metadata:
  maestro-skill-version: "1.1.1"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Create Music-Driven Visuals

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Choose one of two visual models:

- Event reaction: create flashes, particles, lights, or impulses from `OnEventNotesMidi` when events become due.
- Timeline visualization: load the MIDI first and construct a piano roll, falling-note field, or track overview from all relevant events.

Map `Value` to pitch position, `Velocity` to an intentional visual parameter, `Duration`/`Length` to extent, and `Channel` or `Track` to color/grouping. Treat these as design mappings, not fixed Maestro behavior.

For sustained visuals, record the NoteOn's duration or pair explicit NoteOff events by source/channel/note. Enable `MPTK_KeepNoteOff` before loading only when that pairing is required.

Keep rendering objects pooled and lightweight. A dense MIDI can produce many simultaneous events, and event callbacks may contain a group rather than one item.

## MIDI versus audio analysis

MIDI is appropriate for exact notes, instruments, tracks, beats, and future events. Audio analysis is appropriate for waveform energy, spectral content, or recorded material without MIDI. Hybrid visuals may use both without claiming that one replaces the other.

## Verify

Test chords, repeated same-pitch notes, tempo changes, seek, replay, and dense passages. Confirm object cleanup after stop and scene changes.

## Technical depth

- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [react-to-midi-events](../react-to-midi-events/SKILL.md)
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)
