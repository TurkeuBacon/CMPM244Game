---
name: build-midi-player-or-jukebox
description: Build a MIDI player, jukebox, or background-music UI with Maestro MPTK. Use for song selection, transport, seeking, and repeat; external files, playlists, and overlapping transitions require Pro.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Build a MIDI Player or Jukebox

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Choose `MidiFilePlayer` for one selected MIDI from Maestro's MIDI DB. **Requires Pro:** use `MidiExternalPlayer` for user/local/remote files or `MidiListPlayer` for a playlist with transition support.

## Workflow

1. Build the catalog from `MidiPlayerGlobal.MPTK_ListMidi` and application metadata. **Requires Pro:** include validated external URIs.
2. Display names separately from the internal MIDI DB name or URI.
3. Wire play, pause, resume, stop, previous/next, seek, repeat, and volume to the selected player.
4. Update UI from `MPTK_IsPlaying`, `MPTK_IsPaused`, `MPTK_Position`, and `MPTK_DurationMS`.
5. Subscribe to start/end events to resolve loading, natural completion, manual stop, and looping.
6. Define focus/background behavior explicitly.

Do not seek immediately before or after `MPTK_Play`; set the initial position from `OnEventStartPlayMidi`. Use `MPTK_MidiAutoRestart` rather than deprecated `MPTK_Loop` for whole-song repeat.

**Requires Pro:** for external content, validate the URI scheme and surface `MPTK_StatusLastMidiLoaded`/`MPTK_WebRequestError` instead of leaving the UI in a permanent loading state.

## Verify

Test empty catalog, missing file, network failure, rapid next/previous, seek while paused/playing, end-of-list behavior, repeat, focus loss, scene changes, and SoundFont readiness.

## Technical depth

- [set-up-and-choose-maestro-player](../set-up-and-choose-maestro-player/SKILL.md)
- [control-midi-playback](../control-midi-playback/SKILL.md)
- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [handle-platform-timing-and-performance](../handle-platform-timing-and-performance/SKILL.md)
