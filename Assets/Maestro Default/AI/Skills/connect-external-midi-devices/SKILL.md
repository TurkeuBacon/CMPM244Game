---
name: connect-external-midi-devices
description: Receive from and send to external MIDI keyboards, controllers, pads, knobs, and devices with Maestro MPTK. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Connect External MIDI Devices

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because device enumeration, external MIDI input, and MIDI output use `MidiKeyboard` or `MidiInReader`.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present these APIs as compilable. Free can map Unity input to `MidiStreamPlayer`, but it has no equivalent Maestro API for external MIDI devices.

Choose one input style:

- `MidiInReader`: component-based input with Inspector settings, playback, channel controls, and Unity-event integration.
- `MidiKeyboard`: direct static API for device enumeration, input queues/callbacks, and output.

## Minimal queued input

```csharp
bool ready;

void Start()
{
    ready = MidiKeyboard.MPTK_Init();
    if (ready)
        MidiKeyboard.MPTK_OpenAllInp();
}

void Update()
{
    if (!ready) return;

    for (int count = 0; count < 100; count++)
    {
        MPTKEvent midiEvent = MidiKeyboard.MPTK_Read();
        if (midiEvent == null) break;
        HandleOnUnityThread(midiEvent);
    }
}

void OnApplicationQuit()
{
    if (ready) MidiKeyboard.MPTK_CloseAllInp();
}
```

Bound the number read per frame so an input flood cannot stall Unity. Always close input devices during shutdown to avoid crashes.

For callback input, subscribe to `MidiKeyboard.OnActionInputMidi`, call `MPTK_SetRealTimeRead`, and dispatch pending fallback messages from `Update` with `MPTK_DispatchPendingRealtimeInput`. Unsubscribe and call `MPTK_UnsetRealTimeRead` during teardown.

Enumerate with `MPTK_CountInp`/`MPTK_GetInpName` and `MPTK_CountOut`/`MPTK_GetOutName`. Open an output before calling `MPTK_PlayEvent(event, deviceIndex)`, then close it when finished.

## Platform boundary

Native MIDI device support is platform-dependent. Verify the actual Maestro plugin/import settings and test on each target device; Editor success is not platform proof.

## Verify

Test hot-plug/rebuild, unavailable devices, NoteOn/NoteOff pairs, controller values, shutdown, and the final player build on each supported platform.
