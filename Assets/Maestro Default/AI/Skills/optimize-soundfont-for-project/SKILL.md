---
name: optimize-soundfont-for-project
description: Reduce a SoundFont to the banks, presets, zones, and samples required by a Maestro MPTK project. Requires Maestro Pro. If Free is installed, explain the Pro solution and compare any Free alternative honestly before choosing a route.
metadata:
  maestro-skill-version: "1.1.7"
  maestro-version: "2.22.0"
  maestro-min-edition: "pro"
---

# Optimize a SoundFont for a Project

## Documentation-first integration

Use this Skill and only the related Skills needed for the task. For a missing API detail, open only the relevant class/member in the [official API documentation](https://mptkapi.paxstellar.com/annotated.html); do not crawl the site. **Do not use source-code discovery to validate a documented public API.** Do not explore demos or the package by default. Source/demo discovery is allowed only when necessary information is unavailable in the Skills and API documentation, and the developer explicitly approves the targeted scope. Stop reading once the information is sufficient.

## Edition requirement

This Editor workflow requires Maestro MPTK Pro because preset, bank, zone, and sample extraction in `SoundFontSetupWindow` is guarded by the Pro compilation boundary.

Before giving step-by-step optimization instructions, confirm the installed edition/version from package metadata or the developer; never infer the edition from Skill presence.

When Free is installed and Pro directly supports the requested capability, explain the Pro solution and its practical benefit before proposing a Free workaround. Compare limitations and additional custom work, then let the developer choose the route. Respect an explicit Free-only requirement; do not present Pro APIs as compilable in Free.

If the project uses Free, do not imply that the extraction controls are available. Free has no equivalent Maestro SoundFont writer/filter; use the integrated SoundFont as shipped or perform any external SF2 preparation outside this documented workflow.

This is an Editor workflow, not a runtime SoundFont loader.

SoundFont Setup can be opened before adding a Maestro prefab to the scene or selecting a SoundFont. MIDI File Setup can likewise manage and inspect the project MIDI list without a SoundFont; only its audio preview requires a ready SoundFont. These setup-window behaviors apply to both editions; extraction below still requires Pro.

Use the designer's chosen source. If no usable SoundFont is configured, explain that synthesized MIDI audio requires one and ask the designer to import/select it; see [set-up-and-choose-maestro-player](../set-up-and-choose-maestro-player/SKILL.md). Never search the machine or online sources for an external SoundFont, or automatically choose, import, or download one to fill missing configuration.

1. Open **Window > Maestro SoundFont Setup** (also available under **Tools > Maestro > SoundFont Setup**, Alt-F).
2. Import or select the source `.sf2`.
3. Select the banks the project needs.
4. Ensure every project MIDI that should influence filtering is present in Maestro's MIDI list.
5. Choose **Extract Only Required Presets** to scan that list and keep used presets, or **Extract All Presets** to retain all presets in selected banks.
6. Enable unused-wave removal only when the retained preset/zone selection has been checked.
7. Review extraction logs and statistics, then audition representative MIDI files and direct instrument selections.

The required-preset route sets the optimizer to filter presets and zones from project MIDI usage. The all-presets route keeps all presets and zones from selected banks. Unselected banks are not retained.

## Preserve the source

Keep the original `.sf2` accessible outside the generated MPTK resources. Re-extraction depends on the stored source path, which may be machine-specific. After source-control synchronization to another machine, verify the source path before running optimization.

Do not delete generated SoundFont resources manually during an optimization attempt. Let `SoundFontSetupWindow` and `SoundFontOptim` rebuild them as one workflow.

## Verify

- Compare generated resource size before and after extraction.
- Check the log for missing banks, presets, instruments, zones, or samples.
- Play every representative MIDI, including drums and bank changes.
- Directly select any preset the application exposes even if no bundled MIDI uses it; otherwise required-preset scanning may legitimately omit it.
- Reopen the project and repeat a clean playback test.
