---
name: create-adaptive-game-music
description: Create game music that changes with gameplay using Maestro MPTK. Use for runtime tempo, speed, transposition, channel, instrument, sequence, or section changes driven by game state; advanced sequence and section workflows require Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Create Adaptive Game Music

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Choose adaptation points that preserve musical continuity: beats, bars, section markers, loop boundaries, or explicit game-state transitions.

## Supported strategies

- Change `MPTK_Speed`, `MPTK_Tempo`, or `MPTK_Transpose` for broad playback variation.
- Mute, unmute, change volume, or force presets per `MPTK_Channels` entry.
- **Requires Pro:** use `MPTK_InnerLoop` for repeatable sections, remembering that its callback is not a Unity-thread callback.
- Switch MIDI DB files for larger structural changes. **Requires Pro:** use `MidiListPlayer` for playlist transitions.
- Use `MidiStreamPlayer` when events must be generated immediately. **Requires Pro:** build an editable or exportable `MPTKWriter` sequence.
- **Requires Pro:** load or switch SoundFonts asynchronously when a true sound-library change is required.

Changing fields of events received by `OnEventNotesMidi` does not rewrite the music already being played. Preprocess the event list or generate a new sequence for note-level transformation.

Centralize adaptation in one music-state controller. Queue requested changes and commit them at the chosen musical boundary; otherwise several gameplay systems can fight over tempo, instruments, or transport.

## Verify

Exercise every state transition in both directions, including rapid repeated requests. Log the requested game state, commit tick, actual transport state, and resulting channel settings. Test tempo changes, loops, seek, and stop/restart together.

## Technical depth

- [control-midi-playback](../control-midi-playback/SKILL.md)
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)
- [control-midi-channels-and-instruments](../control-midi-channels-and-instruments/SKILL.md)
- [create-edit-and-export-midi](../create-edit-and-export-midi/SKILL.md) — requires Pro
- [manage-soundfonts](../manage-soundfonts/SKILL.md) — requires Pro
