# Repository Guidelines

## Project Structure & Module Organization

QuatRoi is a Windows desktop toy written in C# with WinForms and Win32 interop. `src/Program.cs` contains input, animation, and audio playback; `src/LayeredWindow.cs` presents the overlay with per-pixel alpha. `assets/whip-atlas.png` is the embedded 4 × 4 sprite sheet. `assets/AUDIO-LICENSE.md` documents the embedded WAV. `art/README.md` records art prompts. `scripts/build.ps1` builds into `dist/`. See `README.md` for controls and limitations.

## Build, Test, and Development Commands

Run from the repository root on Windows with .NET Framework 4.8:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/build.ps1
Start-Process .\dist\QuatRoi.exe -ArgumentList '--self-test' -Wait
Start-Process .\dist\QuatRoi.exe -ArgumentList '--smoke' -Wait
```

Build uses the Windows framework C# compiler without NuGet. Self-test checks atlas shape and transparency and writes `art/preview.png`. Smoke test opens the overlay, installs the input hook, and closes automatically. Inspect process exit codes when validating changes.

## Coding Style & Naming Conventions

Use four-space indentation, braces on separate lines, PascalCase for types and methods, and camelCase for fields and locals. Keep code compatible with the framework compiler; avoid newer language syntax or runtime APIs. No formatter or linter is configured. Keep interop and resource lifecycle changes easy to review.

## Testing Guidelines

No unit test framework or coverage threshold is configured. Run both executable checks after input or rendering changes. Inspect the preview, then check clicks on each monitor, screen edges, repeated swings, Ctrl+Shift+Home show, Esc hide, and tray exit. Verify hook and timer cleanup. After lifecycle changes, launch several copies and confirm only the first stays alive, then verify restart after exit. Check mixed DPI and negative monitor coordinates.

## Commit & Pull Request Guidelines

Use concise imperative commit subjects, such as `Fix impact frame timing`. Pull requests should describe behavior changes, validation results, and limitations. Include screenshots or a recording for animation changes and link related issues when available. Publish portable executables as GitHub Release assets, keeping `dist/` out of source history.

## Assets & Configuration

Keep runtime assets inside the repository and embedded in the executable. Preserve atlas frame ordering. Do not commit secrets, generated executables, or test previews. Record revised art prompts in `art/README.md`.
