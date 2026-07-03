# Scene 01 Visual Proof Pass 1

Date: 2026-07-03
Branch: `codex/scene01-visual-proof-pass1`
Base: `main` at `83e0df9`

## Result

Implemented the first Unity visual proof slice for `Scene 01: Summit Hatch +
Bore Room Proof`, then upgraded it into a real-material miniature pass aligned
to the final v7 canon stack.

The scene now reads as a playable-world setup rather than a blank test scene:

- tactile procedural material textures for stone, brick, clay, wood, cloth,
  glass, grass, snow, metal, ceramic, glow, Veil, and ghost-draft surfaces
- cinematic lighting, URP post-processing, HDR capture cameras, and screenshot
  export tooling
- refreshed WebGL preview served through the existing Cloudflare tunnel
- Summit Refuge miniature with roof tiles, plaster walls, timber beams, glass
  catchment, moss workyard, stone path, shrubs, loose material, and crew props
- visible Steward and workers with helmets, lamps, backpack/tool silhouettes,
  and material-coded gear
- damaged summit repair cluster with broad editable/buildable ground
- shallow excavation volume leading to the hidden hatch
- first repair anchors for shelter, terrace edge, and soil bed
- Bore Room reveal with protected shaft shell, old masonry, material cache,
  lanterns, inactive Works / Mist Engine hint, inactive Lines conduit, and
  sealed Black Vault hint
- explicit editability classes for protected shell, protected landmark,
  destroyable ruin, player-built draft, extraction volume, repair anchor, and
  story guide

## Files Changed

- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Editor/UnderstoryScene01VisualBlockoutBuilder.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Editor/UnderstoryProjectVerifier.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Editor/UnderstoryScene01ScreenshotExporter.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Editor/UnderstoryWebGLPreviewBuilder.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scripts/Scene01RuntimeController.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scripts/UnderstoryEditabilityMarker.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Materials/Scene01_Blockout/*`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Materials/Scene01_Blockout/ProceduralTextures/*`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Settings/Scene01_CinematicVolumeProfile.asset`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/README.md`
- `UnityProject/Understory_Unity_Proof/ProjectSettings/GraphicsSettings.asset`
- `UnityProject/Understory_Unity_Proof/ProjectSettings/QualitySettings.asset`
- `Reports/scene01-main-camera.png`
- `Reports/scene01-surface-repair-cluster.png`
- `Reports/scene01-bore-room-reveal.png`

Preview:

- Public tunnel: `https://rom-promises-machinery-renaissance.trycloudflare.com/`
- Local server: `http://127.0.0.1:5176/`
- WebGL output folder: `/tmp/understory-scene01-webgl`

## Validation

Passed:

```sh
/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath /Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil/UnityProject/Understory_Unity_Proof -executeMethod Understory.Editor.UnderstoryProjectVerifier.VerifyReady -logFile /Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil/Reports/unity-verify-ready.log
```

Evidence:

- `Reports/unity-scene01-build.log`
- `Reports/unity-verify-ready.log`
- `Reports/unity-scene01-screenshots.log`
- `Reports/unity-scene01-webgl-build.log`
- `Reports/scene01-main-camera.png`
- `Reports/scene01-surface-repair-cluster.png`
- `Reports/scene01-bore-room-reveal.png`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`

Verifier highlights:

- `Scene01 deterministic flow completed via \`trace_extract\`.`
- `Scene01 deterministic flow completed via \`blast_extract\`.`
- `Understory project verification passed.`
- WebGL page probe returned `HTTP/2 200`.
- WebGL wasm probe returned `HTTP/2 200` with `content-type: application/wasm`.

## Intentionally Untouched

- no store, monetization, multiplayer, combat, Android, or release work
- no character creator
- no full procedural mountain or city sim
- no full water or power simulation
- no old lore vocabulary reintroduced into scene object names or marker text

## Next Recommendation

Merge this branch after review, then run the next slice as an asset-quality
upgrade: replace the remaining primitive placeholders with a small bevelled
real-material kit while preserving this exact clickable loop and verifier gate.
