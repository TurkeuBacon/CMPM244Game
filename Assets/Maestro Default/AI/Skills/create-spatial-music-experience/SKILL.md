---
name: create-spatial-music-experience
description: Create 3D, XR, stage, or installation-style music experiences with Maestro MPTK. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Create a Spatial Music Experience

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro when MIDI channels or tracks must be routed through `MidiSpatializer` and its generated spatial synths.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present `MidiSpatializer` code as compilable. A reduced Free design can position separate `MidiStreamPlayer` emitters and use ordinary Unity spatial-audio settings, but it does not provide the same per-track or per-channel spatial routing.

Start from the desired spatial ownership.

- One MIDI file split into emitters: use `MidiSpatializer` and route by channel or track.
- Independent interactive instruments: place separate `MidiStreamPlayer` synths on the instrument objects.
- One ordinary player with distance/orientation behavior: configure the synth's spatial settings without splitting the sequence.

## Workflow

1. Inspect the MIDI's tracks/channels before choosing the routing mode.
2. Create stable scene anchors for each musical source.
3. Configure routing and enough spatial synth instances.
4. Parent/position the generated synths at the anchors.
5. Configure distance, orientation, listener, and optional effects.
6. Keep interaction/gameplay logic separate from acoustic routing.
7. Profile active voices and spatial sources on the target device.

A track-routed experience needs a MIDI whose instruments are meaningfully separated into tracks. Channel routing is limited by MIDI's channel model. Neither mode automatically understands semantic concepts such as "violin section" unless the content/mapping supplies them.

XR input and locomotion are Unity/application concerns. Maestro supplies sound generation, MIDI events, and routing; do not imply that it supplies an XR framework.

## Verify

Solo and identify every source, move listener/emitters, test missing tracks/channels, inspect attenuation and orientation, and validate the selected audio spatializer in the target build.

## Technical depth

- [configure-spatial-audio-and-effects](../configure-spatial-audio-and-effects/SKILL.md)
- [load-and-inspect-midi-content](../load-and-inspect-midi-content/SKILL.md)
- [handle-platform-timing-and-performance](../handle-platform-timing-and-performance/SKILL.md)
