# Scene 01 Working Loop

Date: 2026-07-03
Branch: `codex/scene01-visual-proof-pass1`
Base: `main` at `83e0df9`

## Result

Scene 01 is now a working Unity first-playable proof, not a static visual
blockout.

Implemented the first scene loop:

- workers gather scarce material and uncover the hidden hatch
- the Steward clears the hatch crust and opens the route into the Bore
- camera transitions to the Bore Room
- first shoring is placed before extraction
- player can use trace extraction or blast extraction
- trace requires a drag gesture; blast creates a burial detour before return
- haul returns physically through the Return Ritual table
- first kiln/refiner converts raw haul into usable repair/build material
- Core Sample receives the first stratum band
- Archive shelf receives the first seam/bulk fragment as a physical object
- player can place, remove, and replace one support block in the surface build zone
- committed build reveals a draft want-list for the next dig
- player repairs the shelter/windbreak, terrace edge, and tiny garden
- completion state confirms the v7 first-scene loop: descend, extract, return,
  refine, build, record, and want the next haul

## Runtime Files

- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scripts/Scene01RuntimeController.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scripts/Scene01Interactable.cs`
- `UnityProject/Understory_Unity_Proof/Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`

## Validation

Passed:

```sh
/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath /Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil/UnityProject/Understory_Unity_Proof -executeMethod Understory.Editor.UnderstoryProjectVerifier.VerifyReady -logFile /Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil/Reports/unity-verify-ready.log
```

Verifier evidence:

- `Scene01 deterministic flow completed via trace_extract.`
- `Scene01 deterministic flow completed via blast_extract.`
- `Understory project verification passed.`

Also passed:

- Unity scene regeneration through `UnderstoryScene01VisualBlockoutBuilder`
- runtime interactable presence checks
- required object and editability marker checks
- v7 canon checks for The Works, Clearers, Mist Engines, The Lines, Black Vault,
  Core Sample, Archive, and want-list proof objects
  proof objects

## Assets

No external asset packs were downloaded or imported for this slice. The scene
uses generated Unity primitives, local blockout materials, and local runtime
scripts.

## Intentionally Untouched

- no store, monetization, multiplayer, combat, Android, or release work
- no full character creator
- no full procedural mountain or city sim
- no full water or power simulation
- no production asset-pack dependency

## Next Recommendation

Review and merge this branch as the first working Scene 01 proof. Next slice
should improve feel and presentation: better Steward silhouette, cleaner camera
beats, more realistic material surfaces, and a stronger click/touch affordance
layer without expanding beyond Scene 01.
