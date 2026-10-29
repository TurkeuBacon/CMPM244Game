---
name: control-midi-playback
description: Control Maestro MIDI transport, seeking, looping, tempo, and speed in Unity. Use for players, jukeboxes, practice controls, or gameplay-driven transport; playlists and accurate subsection loops require Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Control MIDI Playback

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Use `MidiFilePlayer` for one MIDI DB item. **Requires Pro:** use `MidiExternalPlayer` for external content and `MidiListPlayer` when the product needs an actual playlist or overlapping transitions.

```csharp
player.MPTK_MidiName = "My imported MIDI";
player.MPTK_Play();

// Later:
player.MPTK_Pause();
player.MPTK_UnPause();
player.MPTK_Stop();
```

## Transport decisions

- Seek in milliseconds with `MPTK_Position` or in musical ticks with `MPTK_TickCurrent`.
- Set an initial seek from `OnEventStartPlayMidi`; `MPTK_Play()` resets the start position.
- Use `MPTK_MidiAutoRestart` for whole-file repeat. Do not use deprecated `MPTK_Loop`.
- **Requires Pro:** use `MPTK_InnerLoop` for an accurate subsection loop. Its callback runs on the MIDI thread and must not call Unity APIs.
- Use `MPTK_Speed` to scale playback speed. Use `MPTK_Tempo` when the intended control is the sequencer tempo itself; ensure tempo changes are enabled when the file's tempo map must be followed.
- Use `MPTK_IsPlaying` and `MPTK_IsPaused` for UI state rather than maintaining an unrelated transport flag.

Seeking normally rebuilds non-note context up to the destination. `MPTK_RawSeek` is faster but preserves the current context and can therefore produce a different instrument/controller state.

## Verify

Test play, pause, resume, stop, seek, whole-file repeat, and any inner loop against a MIDI containing program and tempo changes. Confirm both audible behavior and transport UI state.

## Go deeper

- For position semantics, read [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md).
- For playlists and application structure, read [build-midi-player-or-jukebox](../build-midi-player-or-jukebox/SKILL.md).
