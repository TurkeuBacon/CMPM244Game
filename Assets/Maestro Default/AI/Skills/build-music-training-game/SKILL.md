---
name: build-music-training-game
description: Build rhythm, sight-reading, ear-training, note, chord, or theory games with Maestro MPTK. Use for exercises and scoring; external MIDI input and Maestro harmony helpers require Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Build a Music Training Game

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Define the exercise as data before choosing Maestro APIs: prompt, allowed response, timing rule, scoring, feedback, and progression.

## Supported patterns

- Rhythm training: generate or load timed events, then score taps against tick or millisecond windows.
- Sight reading: present preloaded notes and receive answers from Unity controls. **Requires Pro:** receive answers directly from external MIDI hardware.
- Ear training: play a known note or an application-defined group of notes with `MidiStreamPlayer`, then compare a symbolic answer.
- Note/chord recognition: compare application input with the expected pitch or pitch-class set. **Requires Pro:** receive NoteOn sets from external MIDI hardware.
- Theory drills: construct prompts from application-owned interval rules. **Requires Pro:** use Maestro scale/chord/progression helpers to generate them.

```csharp
var prompt = new MPTKEvent
{
    Command = MPTKCommand.NoteOn,
    Value = expectedMidiNote,
    Velocity = 100,
    Duration = 750
};
stream.MPTK_PlayEvent(prompt);
```

Randomization must remain reproducible for tests: store the seed or generated exercise. Separate MIDI note numbers from displayed note spelling when enharmonic notation matters.

## Boundary

Maestro can compare structured MIDI input. Do not imply general microphone pitch, chord, or audio-transcription recognition; that requires a separate audio-analysis system.

## Verify

Test generated answers independently from UI, include chord-order permutations and octave policy, and ensure every prompt has a reachable correct response. **Requires Pro:** validate latency windows on target hardware when external MIDI is used.

## Technical depth

- [play-midi-events-in-real-time](../play-midi-events-in-real-time/SKILL.md)
- [generate-chords-scales-and-progressions](../generate-chords-scales-and-progressions/SKILL.md) — requires Pro
- [connect-external-midi-devices](../connect-external-midi-devices/SKILL.md) — requires Pro
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)

Evidence boundary: this workflow is supported by Maestro capabilities and real user applications, but no dedicated packaged demo is claimed for version 2.22.0.
