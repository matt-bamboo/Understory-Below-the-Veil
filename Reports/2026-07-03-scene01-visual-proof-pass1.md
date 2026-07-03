# Scene 01 Visual Proof Pass 1

Date: 2026-07-03
Branch: `codex/scene01-visual-proof-pass1`
Base: `main` at `83e0df9`

## Result

Implemented the first Unity visual proof slice for `Scene 01: Summit Hatch +
Bore Room Proof`, then replaced the blockout-forward presentation with a CC0
imported hero-art layer aligned to the current canon stack.

The scene now reads as a playable-world setup rather than a blank test scene:

- imported CC0 KayKit Medieval Builder house, mine/hatch mouth, farm plot,
  well/shafthead, bridge, walls, rocks, trees, and terrain pieces
- muted URP material remap for imported assets so the scene reads closer to a
  real-material miniature instead of neon source colors
- hidden scaffold/debug overlays that were useful for verification but made the
  camera shots read like diagrams
- tactile procedural support materials for stone, brick, clay, wood, cloth,
  glass, grass, metal, ceramic, glow, Veil, and ghost-draft surfaces
- cinematic lighting, URP post-processing, HDR capture cameras, and screenshot
  export tooling
- refreshed WebGL preview served through the existing Cloudflare tunnel
- Summit Refuge hero miniature with imported house, glass catchment, garden,
  return table, kiln, build pad, archive shelf, Core Sample/shafthead, and
  material scatter
- visible small-scale Steward/worker miniatures, with the runtime Steward using
  the visible hero figure instead of a hidden placeholder
- visible state changes for hatch crust/glow, shoring, trace seam, blast burial,
  haul return, refined materials, repaired shelter/terrace/garden, garden
  sprouts, support block, Core Sample band, and Archive fragment
- Bore Room reveal with protected shaft shell plus imported chamber floor,
  stone walls, mine/material face, rocks, shoring, seam, collapse pile, and
  lantern cues
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
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Materials/Scene01_Blockout/Imported/*`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Materials/Scene01_Blockout/ProceduralTextures/*`
- `UnityProject/Understory_Unity_Proof/Assets/ThirdParty/KayKit/MedievalBuilder/*`
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
- Local browser smoke test rendered the WebGL scene and Space advanced the
  runtime from `Damaged Summit` to `Worked Stone Found`.

Known caveat:

- Browser console reported repeated Unity `level0` data warnings during local
  WebGL load, but the scene rendered and input advanced correctly. Treat this
  as a follow-up WebGL packaging/cache hygiene item, not a blocker for the
  Unity scene proof.

## Intentionally Untouched

- no store, monetization, multiplayer, combat, Android, or release work
- no character creator
- no full procedural mountain or city sim
- no full water or power simulation
- no old lore vocabulary reintroduced into scene object names or marker text

## Next Recommendation

Merge this branch after review, then run the next slice as a focused polish
pass: improve the WebGL packaging warning, add a bespoke bevelled material kit
or higher-grade character/prop assets, and preserve this exact clickable loop
and verifier gate.
