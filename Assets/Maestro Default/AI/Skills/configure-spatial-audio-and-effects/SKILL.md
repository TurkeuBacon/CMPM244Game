---
name: configure-spatial-audio-and-effects
description: Configure Maestro spatial MIDI routing, distance/orientation audio, reverb, chorus, and filters in Unity. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Configure Spatial Audio and Effects

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because `MidiSpatializer` routing and the documented SoundFont/Unity effect controllers are Pro capabilities.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present these APIs as compilable. A reduced Free design can position separate `MidiStreamPlayer` emitters and use ordinary Unity audio settings, but it does not provide the same Maestro routing and effect-control workflow.

Use `MidiSpatializer` when one MIDI file must be split across spatial synths by MIDI channel or track. Use an ordinary `MidiStreamPlayer` or other synth with distance attenuation when the application already owns separate instrument objects.

## Spatializer workflow

1. Add the `MidiSpatializer` prefab and keep Core Player enabled.
2. Choose `MPTK_ModeSpatializer` as channel or track.
3. Set `MPTK_MaxSpatialSynth` high enough: channel mode needs up to 16; track mode needs the relevant track count.
4. At runtime, treat `MPTK_SpatialSynthIndex < 0` as the reader and non-negative indexes as generated spatial synths.
5. Parent or position each generated synth at the desired emitter location.
6. Configure `MPTK_DistanceAttenuation`, minimum/maximum distance, and optional orientation behavior.

Track mode depends on meaningful track separation in the MIDI. A single-track file cannot produce useful per-track spatial separation.

## Effects

Use `MPTK_EffectSoundFont` for SoundFont filter/reverb/chorus parameters and `MPTK_EffectUnity` for Unity reverb/chorus filters. Start from their defaults and change only the effect family the application needs.

Do not confuse musical routing with acoustic spatialization: channel/track routing decides which synth receives an event; Unity/spatializer settings decide how that synth is heard from the listener.

## Verify

Use one Unity `AudioListener`, move the listener and emitters, solo each channel/track, and verify attenuation, direction, voice count, and effects in Play Mode. Validate the selected spatializer plugin in the final target build.
