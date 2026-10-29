---
name: build-rhythm-game
description: Build rhythm-game mechanics in Unity with Maestro MPTK. Use for note highways, falling notes, Piano Tiles or Guitar Hero style spawning, input timing, scoring, and music synchronization; generating editable practice sequences with MPTKWriter requires Pro.
metadata:
  maestro-skill-version: "1.1.6"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Build a Rhythm Game

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Use MIDI as structured chart data and Maestro playback as the transport. Keep chart preparation, visual anticipation, audible playback, and player scoring as separate responsibilities.

The imported-MIDI workflow supports Free and Pro. Before using the branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

For either playback route, apply the SoundFont prerequisite in [set-up-and-choose-maestro-player](../set-up-and-choose-maestro-player/SKILL.md). If no usable SoundFont is configured, explain the audio requirement and ask the designer to import/select one; never search for or automatically choose an external SoundFont.

## Choose the playback architecture

`SourceCatchMusic` may look similar to a rhythm game because MIDI notes are represented by moving GameObjects. However, it was designed as an interactive-music experiment where gameplay can alter the music itself. Its use of `MidiStreamPlayer` is therefore intentional and should not be used as the default architecture for a conventional rhythm game.

For a conventional rhythm game, preload the MIDI events to prepare upcoming notes, let `MidiFilePlayer` play the music, and derive visual positions and input judgments from the same MIDI transport. A falling note reaches the goal at its scheduled musical time; reaching the goal does not trigger its sound. Player input affects scoring independently of playback, unless the requested game design explicitly requires missed or hit notes to alter the music.

## Workflow

1. Create the game's scenes, scripts, and custom prefabs outside `Assets/MidiPlayer/`, for example in `Assets/MyRhythmGame/`. Add a Maestro player instance and disable `MPTK_PlayOnStart` in the Inspector or in `Awake()`; the game owns startup. See [set-up-and-choose-maestro-player](../set-up-and-choose-maestro-player/SKILL.md) for setup and readiness handling.
2. Load the selected MIDI before playback.
3. Convert relevant NoteOn events into a lightweight chart containing at least note/lane, tick, calculated time, duration, channel, and track.
4. Map pitches, channels, or tracks to gameplay lanes using explicit game rules.
5. Enter a ready state only after the player's `MPTK_SoundFont` is available and `IsReady`, the chart is built, and gameplay objects are prepared. Keep scoring and Start controls inactive during loading; show loading/configuration failures.
6. From the ready state, accept a Start action or an automatic launch gated by readiness. Prepare any visual pre-roll and spawn upcoming objects from the chart using a look-ahead window. Start playback with `MPTK_Play(alreadyLoaded: true)` and `MPTK_DirectSendToPlayer` enabled so the MIDI sequencer drives audible playback. Define the pre-roll's transition to MIDI time explicitly; do not launch gameplay and music independently from unrelated `Start()` methods or fixed delays.
7. Judge player input against the authoritative current MIDI tick or position, not against accumulated frame time.
8. Handle pause, seek, speed change, restart, and end by resetting chart cursors and active objects together with transport state.

```csharp
long errorTicks = System.Math.Abs(Player.MPTK_TickCurrent - target.Tick);
if (errorTicks <= PerfectWindowTicks) AwardPerfect();
else if (errorTicks <= GoodWindowTicks) AwardGood();
else AwardMiss();
```

Express scoring windows in ticks when they should scale musically with tempo. Express them in milliseconds when the design requires fixed human-response windows. Document which model the game uses.

`OnEventNotesMidi` is useful for effects that occur when a note becomes due, but it is too late for a falling-note object that needs travel time. Build those objects from inspected future events.

## Requires Pro - Generate a practice sequence

Use this optional branch when generation is requested or accepted. If no MIDI is supplied, offer a temporary practice sequence instead of silently creating content. Generated MIDI still needs the designer-selected SoundFont for audible playback.

1. Define BPM, duration in bars, scale or allowed pitches, note density, maximum simultaneous notes, lane mapping, and a reproducible seed. These composition and difficulty rules belong to the application; `MPTKWriter` builds the sequence.
2. Build the writer in musical ticks, include tempo and instrument context, then call `StableSortEvents()` and `CalculateTiming()`.
3. Prepare the chart from the writer's `MPTK_MidiEvents` using the same note filtering and lane rules as imported content. Retain note lengths in ticks for sustained targets. Freeze the sequence for the round so music and chart describe the same events.
4. Follow the ready-state and launch rules above, but use `MidiFilePlayer.MPTK_Play(writer)` for this branch, replacing MIDI DB loading and `MPTK_Play(alreadyLoaded: true)`. Keep `MPTK_DirectSendToPlayer` enabled and derive visuals/scoring from the player's MIDI transport. Export is optional.

For generator design, read [generate-music-at-runtime](../generate-music-at-runtime/SKILL.md), using its Pro writer route. For editing and optional `WriteToFile` export, read [create-edit-and-export-midi](../create-edit-and-export-midi/SKILL.md) - requires Pro. In Editor prototypes, keep temporary exports in the application's dedicated folder; in player builds, choose a platform-appropriate writable destination.

## Boundaries

- Filter non-note events and usually ignore NoteOn velocity zero as a playable onset.
- Decide how chords, overlapping notes, sustain, drums, and unmapped pitches behave.
- MIDI supplies expected musical events; input still needs a keyboard, controller, touchscreen, or other game-input route.

## Verify

Test tempo changes, low/high frame rates, pause/resume, seek, speed changes, simultaneous notes, and a full replay. Log target tick, player tick, and awarded result for a deterministic test chart.

Check startup with both an initially unavailable and an already-ready SoundFont in each supported Unity version: no playback or scoring during loading, and exactly one coordinated launch after readiness. Verify the first notes audibly as well as visually.

For the Pro generation branch, check reproducibility from the same seed and parameters, agreed lane/density constraints, chart onsets against the generated sequence, and replay without regeneration. Audition generated material before treating it as final content.

## Technical depth

- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [synchronize-with-midi-time](../synchronize-with-midi-time/SKILL.md)
- [react-to-midi-events](../react-to-midi-events/SKILL.md)
- [handle-platform-timing-and-performance](../handle-platform-timing-and-performance/SKILL.md)
