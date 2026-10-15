# SmokeTest (Unity 6000.6.6f1, Android)

Minimal Android-buildable smoke test: ground plane, lit cube + capsule, directional light, camera, drag-to-rotate (touch or mouse), and an on-screen FPS label. The whole scene is built in code at runtime (`Assets/Scripts/SmokeTestBootstrap.cs`), so it runs from any scene, including an empty one.

- Unity version: **6000.6.6f1** (`ProjectSettings/ProjectVersion.txt`)
- Render pipeline: built-in (no URP/Cinemachine/Input System packages; see "Notes")
- Android: package `com.newgame.smoketest`, IL2CPP, ARM64, min API 24, APK output `Builds/SmokeTest.apk`

## Prerequisites

- Unity Hub with editor **6000.6.6f1** installed, including the **Android Build Support** module (Android SDK & NDK Tools, OpenJDK).
- For install: `adb` (Android platform-tools) and a device with USB debugging enabled.
- Git LFS (`git lfs install`) so future textures/models/audio are tracked via LFS (`.gitattributes`).

## Open and run in the Editor

1. Unity Hub > **Add** > **Add project from disk** and select this folder (`unity/SmokeTest`).
2. Open it with 6000.6.6f1. The first open generates `Library/` and `.meta` files (commit the `.meta` files).
3. Press **Play**. A cube and capsule spin on a ground plane; drag with the mouse to rotate; the FPS label is top-left.

## Build the APK (Editor)

1. Menu **Build > Android APK**.
2. The first run creates `Assets/Scenes/SmokeTest.unity` (an empty container scene) if missing, applies the Android settings, and builds to `Builds/SmokeTest.apk`.

## Install on a device

```bash
adb devices
adb install -r Builds/SmokeTest.apk
adb shell am start -n com.newgame.smoketest/com.unity3d.player.UnityPlayerGameActivity
```

If the launch command fails (activity name differs by Unity version), just open "SmokeTest" from the device launcher, or run `adb shell monkey -p com.newgame.smoketest 1`.

## Build from the command line (batchmode)

Run from the repo root, adjusting the Unity executable path:

```bash
# Linux
"$HOME/Unity/Hub/Editor/6000.6.6f1/Editor/Unity" \
  -batchmode -nographics -quit \
  -projectPath unity/SmokeTest \
  -buildTarget Android \
  -executeMethod NewGame.SmokeTest.EditorTools.SmokeTestBuilder.BuildAndroidCli \
  -logFile -
```

```powershell
# Windows
& "C:\Program Files\Unity\Hub\Editor\6000.6.6f1\Editor\Unity.exe" `
  -batchmode -nographics -quit `
  -projectPath unity\SmokeTest `
  -buildTarget Android `
  -executeMethod NewGame.SmokeTest.EditorTools.SmokeTestBuilder.BuildAndroidCli `
  -logFile -
```

```bash
# macOS
/Applications/Unity/Hub/Editor/6000.6.6f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit \
  -projectPath unity/SmokeTest \
  -buildTarget Android \
  -executeMethod NewGame.SmokeTest.EditorTools.SmokeTestBuilder.BuildAndroidCli \
  -logFile -
```

- Output: `unity/SmokeTest/Builds/SmokeTest.apk`; override with `-outputPath /path/to/file.apk`.
- Exit code is `0` on success and `1` on failure.
- Batchmode requires a valid Unity license (e.g. a logged-in Hub, or `-serial`/`-username`/`-password`/`-manualLicenseFile`).

## Notes

- Packages are intentionally minimal (built-in modules only) so the project resolves cleanly offline. URP/Cinemachine/Input System versions are tied to the editor version and were not added because they cannot be verified without running Unity; add them via Package Manager when needed.
- Touch/mouse input uses the legacy Input Manager, guarded by `#if ENABLE_LEGACY_INPUT_MANAGER`. If **Player Settings > Active Input Handling** is set to "Input System Package (New)" only, the objects auto-spin and drag is ignored. Set it to "Input Manager (Old)" or "Both" to enable dragging.
- `Builds/`, `Library/`, `*.apk` etc. are git-ignored (`.gitignore`).
