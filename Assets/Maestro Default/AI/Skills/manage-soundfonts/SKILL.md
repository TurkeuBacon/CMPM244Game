---
name: manage-soundfonts
description: Load, switch, and inspect internal or external SoundFonts at runtime with Maestro MPTK. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.3"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Manage SoundFonts

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This workflow requires Maestro MPTK Pro because runtime SoundFont loading and switching depend on the Pro SoundFont APIs.

Before generating implementation code, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not present runtime `MPTK_SoundFont.Load` workflows as compilable. Free can use the SoundFont already integrated into the package and select its available banks and presets, but it cannot perform the equivalent runtime library loading or switching.

First check the [SoundFont prerequisite](../set-up-and-choose-maestro-player/SKILL.md): if none is usable, explain the audio requirement and ask the designer to import/select their intended resource. Never search local folders or online sources for an external SoundFont, or automatically choose, import, or download one as a replacement. A selected resource still loading is different from missing configuration.

For an individual synth, use its `MPTK_SoundFont`. `Load` accepts an internal Maestro SoundFont name, an HTTP/HTTPS URL, a `file://` URI, or an empty value for the default internal SoundFont. Use the designer-selected name/URI; API support for external loading is not permission to discover or choose a library on their behalf.

```csharp
synth.MPTK_SoundFont.LoadedCallback = loadedSynth =>
{
    Debug.Log($"SoundFont status: {loadedSynth.MPTK_SoundFont.StatusSoundFont}");
};

bool started = synth.MPTK_SoundFont.Load("My Internal SoundFont");
if (!started)
    Debug.LogError("SoundFont loading did not start.");
```

`Load` returning true means loading has started, not that the SoundFont is already ready. Use `LoadedCallback`, `StatusSoundFont`, and `IsReady`. `ProgressCallback` is not on the Unity thread, so store progress data and update Unity UI later on the main thread.

Configure `LoadFromCache`, `SaveToCache`, and `DownloadOnly` before external loading when those behaviors are needed. Do not hard-code a bank or preset without checking `BanksNumber`, `PresetsNumber`, and the active SoundFont.

Use `MidiPlayerGlobal.MPTK_SelectSoundFont` only when intentionally switching the shared internal selection. Per-synth `MPTK_SoundFont.Load` supports a synth-specific runtime choice.

## Verify

Test success, invalid name/URI, download failure, cache reuse, callback thread handling, preset availability, and replay after switching. Display a ready/error state instead of assuming that download completion equals usable instruments.

## Related skill

For reducing package size in the Editor, read [optimize-soundfont-for-project](../optimize-soundfont-for-project/SKILL.md).
