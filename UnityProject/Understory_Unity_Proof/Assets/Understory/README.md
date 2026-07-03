# Understory Unity Proof

This Unity project is the production proof for Scene 01: Summit Hatch + Bore
Room Proof.

Start with:

- `Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`
- `Assets/Understory/Settings/Understory_URP.asset`

Scope stays tight: visual proof first, then interaction/editability, then the
first extract / haul / refine / repair loop.

## Scene 01 Visual Proof Pass 1

The current scene contains `Scene01_VisualProofPass1`, a visual blockout for the
first playable proof:

- damaged summit repair cluster with a broad editable surface zone
- shallow summit cut revealing the hidden hatch
- Steward and worker placeholders to make the discovery beat readable
- Bore Room reveal with protected shell, inactive Works/Mist Engine hints, Lines,
  and sealed Black Vault hint
- editability markers for protected shell, protected landmark, destroyable ruin,
  player-built draft, extraction volume, and repair anchor zones

Regenerate the blockout with:

```sh
/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -quit \
  -projectPath /Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil/UnityProject/Understory_Unity_Proof \
  -executeMethod Understory.Editor.UnderstoryScene01VisualBlockoutBuilder.BuildScene01VisualProofPass1
```

Verify the project with:

```sh
/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -quit \
  -projectPath /Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil/UnityProject/Understory_Unity_Proof \
  -executeMethod Understory.Editor.UnderstoryProjectVerifier.VerifyReady
```
