---
name: generate-chords-scales-and-progressions
description: Generate and play scales, chords, and chord progressions with Maestro MPTK helpers. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Generate Chords, Scales, and Progressions

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because it depends on Maestro's chord, scale, and progression helper APIs.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present those helpers as compilable. Free can play manually constructed note lists with `MidiStreamPlayer`, but the application must provide its own music-theory generation logic.

Choose the abstraction that matches the musical rule:

- `MPTKScaleLib.CreateScale` returns scale intervals.
- `MPTKChordBuilder` builds a chord from a scale range or the chord library.
- `MPTKChordProgressionLib` supplies progression presets and chord builders.
- `MidiStreamPlayer` plays generated chords live.
- `MPTKWriter` places them on a MIDI timeline for later playback or export.

```csharp
MPTKScaleLib scale = MPTKScaleLib.CreateScale(MPTKScaleName.MajorMelodic);
int tonic = 60;

var root = new MPTKEvent { Command = MPTKCommand.NoteOn, Value = tonic, Duration = 500 };
var third = new MPTKEvent { Command = MPTKCommand.NoteOn, Value = tonic + scale[2], Duration = 500 };
stream.MPTK_PlayEvent(new System.Collections.Generic.List<MPTKEvent> { root, third });
```

Keep pitch range, voicing, inversion, octave, velocity, rhythm, and voice leading as explicit application decisions. A progression helper supplies musical structure; it does not define the complete arrangement.

For held live chords, retain the returned/built chord or its events and call `MPTK_StopChord` or stop the individual events. For a file, add chord events at absolute ticks and call `CalculateTiming` before inspection or export.

## Verify

Log generated note values, keep them in MIDI range 0-127, test the selected scale/chord names, and listen for stuck notes after repeated chord changes.
