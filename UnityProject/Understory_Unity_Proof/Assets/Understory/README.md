# Understory Unity Proof

This Unity project is the production proof for Scene 01: Summit Hatch + Bore
Room Proof.

Start with:

- `Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`
- `Assets/Understory/Settings/Understory_URP.asset`

Scope stays tight around the v7 first playable loop: Summit Refuge, hidden hatch,
first descent, trace/blast extraction, Return Ritual, kiln refine, Core Sample,
Archive shelf, ghost draft want-list, and first surface repair/build payoff.

## Scene 01 Working Proof

The current scene contains `Scene01_VisualProofPass1`, a working first-scene
playable proof:

- damaged summit repair cluster with a broad editable surface zone
- shallow summit cut revealing the hidden hatch into the Bore
- Steward and worker placeholders to make the first Bore opening beat readable
- Bore Room reveal with protected shell, inactive Works/Mist Engine hint, The
  Lines conduit, and sealed Black Vault hint
- Core Sample column, Phase 0 Archive shelf, Far-Glass seed, and ghost draft
  want-list objects
- editability markers for protected shell, protected landmark, destroyable ruin,
  player-built draft, extraction volume, and repair anchor zones
- tap/click runtime flow from summit material gathering to Bore opening,
  Bore Room reveal, shoring, trace/blast extraction, blast burial cleanup,
  physical Return Ritual, kiln refine, Core Sample band, Archive fragment,
  repair anchors, and build/remove proof
- glowing in-world action markers and a small HUD; the button-list proof path is
  no longer the primary interaction
- deterministic verifier coverage for both trace and blast extraction paths

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
