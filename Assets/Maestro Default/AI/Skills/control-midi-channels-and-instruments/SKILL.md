---
name: control-midi-channels-and-instruments
description: Control Maestro MIDI channels, banks, presets, volume, muting, transposition, and drums. Use when selecting instruments or adapting playback by channel.
metadata:
  maestro-skill-version: "1.1.1"
  maestro-version: "2.22.0"
  maestro-min-edition: "free"
---

# Control MIDI Channels and Instruments

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

Access channel state through the player's `MPTK_Channels` collection. MIDI channel indexes are zero-based; channel 9 is conventionally the drum channel.

```csharp
MPTKChannel channel = player.MPTK_Channels[0];
channel.Volume = 0.7f;
channel.Enable = true;
channel.BankNum = 0;
channel.PresetNum = 40;
```

Use `ForcedPreset` when incoming Program Change events must not replace the application's chosen preset. Set it back to `-1` to restore normal MIDI program handling. The same principle applies to a forced bank.

For a `MidiFilePlayer`, channel extensions reset when playback starts by default. Apply startup mute, volume, or forced-instrument choices from `OnEventStartPlayMidi`, or deliberately change `MPTK_Channels.EnableResetChannel` only when persistent state is required.

Use `MPTK_Transpose` for global playback transposition and `MPTK_TransExcludedChannel` when one channel, commonly drums, must not move. For custom event processing, `MPTKEvent.TransposeValue` preserves the original note value.

Do not assume a General MIDI preset exists in every SoundFont. Check the active SoundFont's available bank/preset lists and resolve names with its API.

## Verify

Use a MIDI with at least two melodic channels and drums. Confirm mute, volume, preset forcing, bank selection, and transpose behavior after stop/replay and after SoundFont changes.
