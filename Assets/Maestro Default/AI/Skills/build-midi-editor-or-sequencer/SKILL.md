---
name: build-midi-editor-or-sequencer
description: Build a MIDI editor, step sequencer, arranger, or saveable composition tool with Maestro MPTK. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Build a MIDI Editor or Sequencer

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because its editable sequence, audition, and export path depends on `MPTKWriter`.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present `MPTKWriter` code as compilable. Free can load and inspect MIDI DB content and play generated events in real time, but it does not provide the equivalent saveable editor/export workflow.

Use an application-owned edit model for selection, undo, snapping, drag state, and display. Convert that model to/from `MPTKWriter` for MIDI semantics, playback, and export.

## Workflow

1. Load an existing sequence with `MPTKWriter.LoadFromFile`/`LoadFromMidiDB`, or create a new writer with the chosen tick resolution.
2. Present notes and other events by absolute `Tick`, track, and channel.
3. Quantize only when the user requests it; preserve original timing otherwise.
4. Apply edits to the writer event list or through its add/delete/import helpers.
5. Stable-sort and recalculate timing after structural changes.
6. Audition directly through `MidiFilePlayer.MPTK_Play(writer)`.
7. Export with `WriteToFile` and report failures to the user.

Do not model every rectangle as only a note. Preserve tempo, time signature, program, bank/controller, text/lyric, and track events when round-trip fidelity matters.

Keep undo/redo above Maestro's writer rather than assuming `MPTKWriter` provides editor history. Clone or snapshot only the data needed by the application's undo design.

## Verify

Round-trip a controlled MIDI through load, no-op save, reload, then compare events. Test overlapping notes, simultaneous events, multiple tempos/tracks, Unicode lyrics when enabled, deletion, undo, and unwritable destinations.

## Technical depth

- [create-edit-and-export-midi](../create-edit-and-export-midi/SKILL.md)
- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [control-midi-playback](../control-midi-playback/SKILL.md)
