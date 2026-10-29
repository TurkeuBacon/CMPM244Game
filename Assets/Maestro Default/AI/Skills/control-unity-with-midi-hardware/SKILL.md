---
name: control-unity-with-midi-hardware
description: Use MIDI keyboards, pads, knobs, sliders, and controllers as Unity input with Maestro MPTK. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Control Unity with MIDI Hardware

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because external MIDI input uses `MidiKeyboard` or `MidiInReader`.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present these APIs as compilable. Unity keyboard, touch, gamepad, or application-defined input can still drive `MidiStreamPlayer`, but Free has no equivalent Maestro external-MIDI device input.

Treat hardware input as an adapter that produces `MPTKEvent` messages, then map those messages into application actions.

## Workflow

1. Initialize and enumerate devices; show names instead of assuming a fixed index.
2. Open input through `MidiInReader` or `MidiKeyboard`.
3. Normalize events into an application mapping layer.
4. Map NoteOn/NoteOff to buttons or notes and ControlChange to continuous or stepped controls.
5. Apply smoothing, dead zones, pickup behavior, or value scaling in the application layer.
6. Close devices and unsubscribe callbacks during teardown.

```csharp
void HandleMidi(MPTKEvent midiEvent)
{
    if (midiEvent.Command == MPTKCommand.ControlChange &&
        midiEvent.Controller == MPTKController.Modulation)
        SetEffectAmount(midiEvent.Value / 127f);
}
```

Do not let raw device indexes or controller numbers leak throughout gameplay code. Store a user-editable mapping by device identity, channel, command, controller/note, and value rule.

If the same event should sound, send it to a `MidiStreamPlayer` after the application has decided whether to transform or consume it.

## Verify

Test missing devices, reconnect/rebuild, alternate device order, fast controller traffic, NoteOn velocity zero, teardown, and the actual target build. Hardware support is platform-dependent.

## Technical depth

- [connect-external-midi-devices](../connect-external-midi-devices/SKILL.md)
- [play-midi-events-in-real-time](../play-midi-events-in-real-time/SKILL.md)
- [handle-platform-timing-and-performance](../handle-platform-timing-and-performance/SKILL.md)
