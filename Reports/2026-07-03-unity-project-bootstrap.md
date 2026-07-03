# Unity Project Bootstrap Report

Timestamp: 7/3 9:27AM PT

Superseded setup note: Xcode finished installing after this bootstrap report.
See `Reports/2026-07-03-final-readiness-audit.md` for the final verified local
tooling state.

## Status

Unity project bootstrap is complete and verified for starting Scene 01 work.

## Repo

- Local path: `/Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil`
- Project path: `UnityProject/Understory_Unity_Proof`
- Base commit before this pass: `b1c884c`

## Installed Tooling Verified

- Unity Hub installed.
- Unity Editor installed: `6000.5.2f1`
- Unity iOS Build Support installed:
  `/Applications/Unity/Hub/Editor/6000.5.2f1/PlaybackEngines/iOSSupport`
- Apple Command Line Tools installed: `/Library/Developer/CommandLineTools`
- Xcode was still incomplete during this bootstrap pass. Final readiness later
  verified Xcode `26.6`, license acceptance, first-launch setup, iOS SDK
  `26.5`, iOS Simulator SDK `26.5`, and iOS Simulator runtime `26.5`.

## Unity Project Created

- Created Unity project at `UnityProject/Understory_Unity_Proof`.
- Added bundled Universal Render Pipeline package `17.5.0`.
- Created Understory folder skeleton under `Assets/Understory/`.
- Created URP assets:
  - `Assets/Understory/Settings/Understory_URP.asset`
  - `Assets/Understory/Settings/Understory_UniversalRenderer.asset`
- Created starter scene:
  - `Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`
- Registered Scene 01 in Unity build settings.
- Switched the active local Unity build target to iOS.

## Validation

Unity batch verifier passed using:

```sh
"/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$(pwd)/UnityProject/Understory_Unity_Proof" -executeMethod Understory.Editor.UnderstoryProjectVerifier.VerifyReady -logFile "$(pwd)/Reports/unity-verify-ready.log"
```

Verifier confirmed:

- Unity version is `6000.5.2f1`.
- Active build target is iOS on this machine.
- Default graphics pipeline is URP.
- Current quality render pipeline is URP.
- Understory URP asset exists.
- Understory Universal Renderer asset exists.
- Scene 01 starter scene exists.
- Scene 01 is enabled in build settings.

## Remaining Gate

Cleared after this bootstrap pass. Scene 01 implementation can begin now in the
Unity editor. iOS device signing/provisioning remains future Apple
developer-account work.
