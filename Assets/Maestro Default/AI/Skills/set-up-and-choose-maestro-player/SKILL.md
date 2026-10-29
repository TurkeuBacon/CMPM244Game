---
name: set-up-and-choose-maestro-player
description: Choose and configure the right Maestro MPTK player in a Unity scene. Use when starting an integration or deciding between MIDI file, real-time, external-file, hardware-input, playlist, and spatial playback; several specialized players require Pro.
metadata:
  maestro-skill-version: "1.1.5"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Set Up and Choose a Maestro Player

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Choose the component from the source of the musical events, then add the corresponding Maestro prefab through the Unity Editor when practical. Prefabs expose important Inspector settings; runtime `AddComponent` setup is useful for minimal examples but is not the preferred production default.

Create application scenes, scripts, and custom prefabs in a dedicated folder such as `Assets/MyRhythmGame/`, outside `Assets/MidiPlayer/`. Use bundled demos as references. Configure Maestro prefab instances in the application scene without changing the package prefab assets.

## Choose the player

- `MidiFilePlayer`: play or inspect MIDI files imported into Maestro's MIDI DB.
- **Requires Pro:** `MidiExternalPlayer` plays MIDI from a local/remote URI, bytes, or an `MPTKWriter` sequence.
- `MidiStreamPlayer`: play notes and other MIDI events created at runtime.
- **Requires Pro:** `MidiInReader` or `MidiKeyboard` receives external MIDI devices. Use `MidiInReader` for component/Inspector integration and `MidiKeyboard` for direct device control.
- **Requires Pro:** `MidiListPlayer` provides playlists and overlapping transitions.
- **Requires Pro:** `MidiSpatializer` routes a MIDI file to spatial synths by channel or track.

Keep exactly one active `MidiPlayerGlobal`; it owns shared Maestro setup such as the default SoundFont. Keep exactly one Unity `AudioListener` in the listening scene.

## Confirm the SoundFont prerequisite

Before implementing synthesized MIDI playback, inspect the configured SoundFont in **Window > Maestro SoundFont Setup** and its generated resource state. In a running scene, check the selected synth's `MPTK_SoundFont` and `IsReady`, then the banks/presets required by the application. Distinguish missing or unusable configuration from a designer-selected SoundFont still loading; `IsReady == false` alone does not establish that no SoundFont is configured.

If no usable SoundFont is configured, explain that Maestro needs one to synthesize MIDI audio, then ask the designer to import or select the SoundFont they want. Never search machine folders or online sources for an external SoundFont, or automatically choose, import, or download one to fill this gap. Use only the designer's supplied or explicitly selected resource. Sound-independent work such as MIDI inspection can continue while that choice is pending.

Offer the MPTK steps: open **Window > Maestro SoundFont Setup**, select an available SoundFont, and check that the required presets/resources are usable. **Requires Pro:** the designer can use **Import SoundFont** outside Play Mode to import their chosen `.sf2`; see [optimize-soundfont-for-project](../optimize-soundfont-for-project/SKILL.md) for extraction steps. In Free, use the integrated SoundFont; do not present the Pro import controls as available.

## Minimal file-player setup

Project setup is independent of scene playback. In both editions, **Window > Maestro SoundFont Setup** opens without a scene prefab or a selected SoundFont. **Window > Maestro MIDI File Setup** allows MIDI import, management, raw-event inspection and statistics without a SoundFont; audio preview requires a ready SoundFont and otherwise shows guidance in that window. Missing setup alone does not require a Console warning from these tools. The Pro MIDI Editor retains its SoundFont requirement and reports missing configuration with a dialog.

When application code owns playback, disable **Automatic MIDI Start** (`MPTK_PlayOnStart`) on the scene instance in the Inspector, or in `Awake()` before the player's `Start()`. Wait for `MPTK_SoundFont.IsReady` before sound-dependent actions; a single unsuccessful check in `Start()` must not abandon startup. After checking the SoundFont configuration above, this minimal example starts automatically once the SoundFont is available:

```csharp
using System.Collections;
using MidiPlayerTK;
using UnityEngine;

public class StartMaestro : MonoBehaviour
{
    public MidiFilePlayer Player;

    void Awake()
    {
        if (Player != null)
            Player.MPTK_PlayOnStart = false;
    }

    IEnumerator Start()
    {
        if (Player == null)
        {
            Debug.LogError("Assign the scene's MidiFilePlayer.", this);
            yield break;
        }

        // Let scene components finish their Start methods.
        yield return null;
        while (Player.MPTK_SoundFont == null || !Player.MPTK_SoundFont.IsReady)
            yield return null;

        Player.MPTK_MidiName = "My imported MIDI";
        Player.MPTK_Play();
    }
}
```

Use the exact MIDI DB name without a file extension. **Requires Pro:** for external files, use `MidiExternalPlayer` and a supported URI instead of pretending the file is in the MIDI DB.

For a game, keep one startup owner: prepare game data, expose a ready state, then start transport from the player's action or an automatic launch gated by readiness. Keep Start controls unavailable during loading and display configuration/loading failures instead of leaving the application waiting silently. A fixed delay is not a readiness check.

## Verify

- The scene contains `MidiPlayerGlobal`, the selected player, and one `AudioListener`.
- The selected player's `MPTK_SoundFont.IsReady` becomes true before sound-dependent actions.
- With missing or unusable configuration, the assistant explains the audio prerequisite and asks for the designer's SoundFont choice without searching for or selecting an external replacement.
- Application files are outside `Assets/MidiPlayer/`; application-controlled playback has one startup owner and no competing player autostart. Check both an initially unavailable and an already-ready SoundFont.
- A known MIDI and the active SoundFont produce sound in Play Mode.

## Go deeper

- For content sources and inspection, read [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md).
- For coordinated chart preparation and game startup, read [build-rhythm-game](../build-rhythm-game/SKILL.md).
- For low-latency and platform choices, read [handle-platform-timing-and-performance](../handle-platform-timing-and-performance/SKILL.md).
