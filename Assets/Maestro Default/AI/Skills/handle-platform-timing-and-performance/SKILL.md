---
name: handle-platform-timing-and-performance
description: Diagnose and tune Maestro timing, audio latency, threading, focus behavior, and platform constraints. Use when playback drifts, reacts late, glitches, consumes too much CPU, or behaves differently across Unity targets.
metadata:
  maestro-skill-version: "1.1.2"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Handle Platform Timing and Performance

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Before using a branch labeled **Requires Pro**, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence. When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

Measure before changing synth settings. Separate four different symptoms: MIDI scheduling drift, input latency, audio output latency, and Unity visual-frame latency.

## Reliable defaults

- Prefer Core Player where the target supports it. WebGL forces non-core behavior.
- Prefer Unity-provided audio settings (`MPTK_AudioSettingFromUnity`) unless a measured problem justifies manual sample-rate or buffer changes.
- Keep game/visual reactions in `OnEventNotesMidi` or the Unity main thread.
- Never call Unity APIs from callbacks explicitly documented as MIDI/worker-thread callbacks. **Requires Pro:** this includes `MPTKInnerLoop.OnEventInnerLoop` and runtime SoundFont progress callbacks.
- Use `MPTK_PauseOnFocusLoss` and `MidiPlayerGlobal.MPTK_RunInBackground` deliberately; do not let editor focus behavior define the product requirement accidentally.

Smaller audio buffers can reduce latency but increase CPU pressure and glitch risk. Changing synth rate or buffer configuration is global and can stop/restart audio sources; perform such changes while playback is stopped and test on the actual device.

## Diagnostic sequence

1. Reproduce with a known simple MIDI and default SoundFont.
2. Log `MPTK_TickCurrent`, event `Tick`, player position, frame time, and active voice/DSP statistics.
3. Test pause/resume, focus loss, and scene changes separately.
4. Compare Editor with a development build on the target platform.
5. Change one of buffer size, sample rate, MIDI thread mode/priority, or voice limits at a time.

Do not use `Update` accumulation as the musical clock. Frame rate affects when visuals are presented, but Maestro tick/position data should remain the synchronization authority.

## Platform checks

- WebGL: no Core Player; avoid workflows requiring threads or unrestricted local file writes.
- Android: verify the selected audio backend and device-specific latency. **Requires Pro:** Oboe-related behavior is conditional.
- **Requires Pro:** for external MIDI and spatializer plugins, verify native plugin availability per target.
- Mobile/background: test OS focus, suspension, and audio policy on device.
