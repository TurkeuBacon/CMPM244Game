---
name: build-piano-learning-app
description: Build a piano or instrument learning and play-along application with Maestro MPTK. Use for upcoming-note displays, matching, and guided practice; external MIDI hardware input requires Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Build a Piano Learning App

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Use a MIDI file as the lesson score, Maestro playback as optional accompaniment, and Unity input as the learner's performance. **Requires Pro:** receive the performance directly from external MIDI hardware.

## Workflow

1. Load the lesson and extract expected NoteOn events with pitch, tick/time, duration, channel, and track.
2. Select which tracks/channels belong to the learner and which remain accompaniment.
3. Display upcoming notes from the preloaded event list.
4. Receive learner notes from an on-screen keyboard or another Unity input adapter. **Requires Pro:** use `MidiInReader` or `MidiKeyboard` for external MIDI hardware.
5. Match input to active expected notes using pitch plus a documented timing window.
6. Track NoteOff/release only when the exercise evaluates duration or articulation.
7. Pause, wait, loop, or advance according to the chosen pedagogy.

Do not compare only against the next global MIDI event: chords and simultaneous notes require a set of currently acceptable expected notes. Decide whether octave equivalence, velocity, wrong extra notes, sustain pedal, and early releases matter.

For wait-for-player mode, keep lesson progress under application control. Do not repeatedly seek on every frame; advance transport or sequence state at explicit checkpoints.

## Verify

Use lessons containing single notes, chords, rests, repeated notes, left/right-hand tracks, and tempo changes. Test without hardware by feeding deterministic `MPTKEvent` input through the same matching layer.

## Technical depth

- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [connect-external-midi-devices](../connect-external-midi-devices/SKILL.md) — requires Pro
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)
- [play-midi-events-in-real-time](../play-midi-events-in-real-time/SKILL.md)
