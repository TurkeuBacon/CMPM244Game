---
name: create-edit-and-export-midi
description: Create, modify, merge, calculate, and export MIDI sequences with Maestro MPTK. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Create, Edit, and Export MIDI

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because sequence creation, editing, merging, and file export depend on `MPTKWriter`.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present `MPTKWriter` code as compilable. Free can inspect MIDI DB content and play application-created events immediately with `MidiStreamPlayer`, but it does not provide the equivalent editable timeline or MIDI-file export.

Use [`MPTKWriter`](https://mptkapi.paxstellar.com/db/df6/class_midi_player_t_k_1_1_m_p_t_k_writer.html) as the editable MIDI sequence. Work in absolute ticks and decide the ticks-per-quarter-note resolution when constructing it.

```csharp
var writer = new MPTKWriter(deltaTicksPerQuarterNote: 480, midiFileType: 1, bpm: 120);
long tick = 0;

writer.AddChangePreset(track: 1, tick: tick, channel: 0, preset: 0);
writer.AddNote(track: 1, tick: tick, channel: 0, note: 60, velocity: 100, length: 480);
writer.AddNote(track: 1, tick + 480, channel: 0, note: 64, velocity: 100, length: 480);

writer.StableSortEvents();
writer.CalculateTiming();
bool saved = writer.WriteToFile(destinationPath);
```

Load an existing file with `LoadFromFile` or an imported MIDI with `LoadFromMidiDB`. Then modify `MPTK_MidiEvents` using the writer's add/delete/import operations. Prefer `AddNote`, `AddChangePreset`, `AddControlChange`, `AddBPMChange`, `AddTimeSignature`, and `AddText` over manually encoding raw MIDI data.

After edits:

1. Keep every event's `Tick` absolute and non-negative.
2. Call `StableSortEvents()` when order may have changed.
3. Call `CalculateTiming()` before relying on calculated time, measure, beat, tempo, or duration information.
4. Write with `WriteToFile`, or use `WriteToMidiDB` only for the intended Maestro Editor/project workflow.

Use `MidiFilePlayer.MPTK_Play(writer)` to audition without first writing a file.

## Boundaries

- MIDI file writing is Pro functionality.
- MusicXML conversion is not a supported shipped Maestro workflow.
- File-system destinations and write permissions differ by platform; do not assume an arbitrary desktop path is writable in a player build.

## Verify

Reload the exported file, compare event count and last tick, inspect tempo/signature maps, and audition it. Verify the resulting file outside Maestro when interoperability matters.
