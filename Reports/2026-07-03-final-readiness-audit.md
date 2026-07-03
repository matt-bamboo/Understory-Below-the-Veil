# Final Readiness Audit

Timestamp: 7/3 10:47AM PT

## Status

Understory is ready to begin Scene 01 implementation in Unity.

The local setup gate that was previously blocked by Xcode is cleared:

- Xcode `26.6` is installed at `/Applications/Xcode.app`.
- Xcode license check exits `0`.
- Xcode first-launch setup exits `0`.
- `xcode-select` points to `/Applications/Xcode.app/Contents/Developer`.
- Installed SDK list includes iOS `26.5` and iOS Simulator `26.5`.
- iOS Simulator runtime `26.5` is installed.
- Available simulator devices include iPhone 17, iPhone 17 Pro, iPhone 17 Pro Max, iPhone 17e, iPhone Air, iPad Pro 13-inch (M5), iPad Pro 11-inch (M5), iPad mini (A17 Pro), iPad Air 13-inch (M4), iPad Air 11-inch (M4), and iPad (A16).
- Unity `6000.5.2f1` is installed.
- Unity iOS Build Support is installed.
- Unity batch verification passes for `UnityProject/Understory_Unity_Proof`.

## Repo

- Local path: `/Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil`
- Branch: `main`
- Current commit at audit start: `826e81d66aab82e8044a7e87b1dbd9a67e0f8a0a`
- Unity project path: `UnityProject/Understory_Unity_Proof`

## Source Materials Rechecked

The downloaded start docs match the repo copies byte-for-byte:

- `/Users/matthewgrossman/Downloads/UNDERMINE_UNITY_PROOF_SOURCE_OF_TRUTH_v1.md`
- `SOURCE_OF_TRUTH.md`
- `/Users/matthewgrossman/Downloads/understory-new-repo-handoff-manual-v7.md`
- `Docs/START_HERE_UNDERSTORY_HANDOFF_v7.md`

The handoff zip contains the expected docs, visual references, character references, and archived prior scope docs.

## Source-of-Truth Carry Forward

Locked for Scene 01:

- Working title remains `Understory`; `Undermine` is only a title candidate.
- Unity 6 + URP is the active production proof path.
- First deliverable is `Scene 01: Summit Hatch + Bore Room Proof`.
- Player role is the visible Summit Steward.
- Movement target is soft tap-to-move between meaningful nodes.
- Core loop is `descend -> extract -> ascend -> refine -> build -> wait -> return`.
- First repair cluster is shelter/windbreak, terrace edge, and tiny soil/garden bed.
- First discovery is workers gathering scarce material uncovering a hidden hatch.
- The Bore Room must feel ancient, built, deep, practical, and non-magical.
- Broad editable/buildable zones plus protected structural shells are the architecture target.
- Phase 0 should not include monetization, combat, multiplayer, Android, full water sim, full character creator, full procedural mountain, or a full city sim.

Deprecated language remains blocked:

- `Old Lungs`
- `Breathstone`
- `Breathlines`
- `the mountain breathes`
- `breathing mountain`

Use instead:

- The Works
- Clearers
- Mist Engines
- The Lines
- Black Vaults

## Hemstitch Context Rechecked

Hemstitch remains preserved reference, not production source.

Reviewed current and branch context from:

- `/Users/matthewgrossman/Documents/GitHub/Hemstitch/HEMSTITCH_MASTER_BUILD.md`
- `/Users/matthewgrossman/Documents/GitHub/Hemstitch/R1_TUNE_AND_LOCK.md`
- `/Users/matthewgrossman/Documents/GitHub/Hemstitch/REFERENCE_NOTES.md`
- `/Users/matthewgrossman/Documents/GitHub/Hemstitch/MATERIAL_TEST_RESULTS.md`
- `/Users/matthewgrossman/Documents/GitHub/Hemstitch/artifacts/rounded/SANDBOX_DOCUMENTATION.md`
- `/Users/matthewgrossman/Documents/GitHub/Hemstitch/artifacts/rounded/WORLD_FIXTURE_HANDOFF.md`
- `origin/codex/understory-concept-world-mockup`
- `origin/codex/understory-crew-lab`

Useful lessons to carry forward:

- Keep the world data simple and the presentation rich.
- Preserve touch-first ghost/preview/commit behavior so camera gestures do not accidentally edit.
- Treat every world change as an explicit validated edit operation.
- Separate protected shell, editable ruin, player-built, extraction volume, and repair anchor classes early.
- Use Hem material proof as evidence that material identity and rounded form matter.
- Use Hem's scale study as a caution: full literal mountain editability is not the goal, but Understory also should not collapse into tiny fake pockets only.

Do not carry forward:

- Hem as canon.
- Hem's later Premium Toy PBR target as the Understory runtime target.
- Old Understory branch language such as `Breathline` or `Old Lung`.
- The custom web engine as the production engine.
- Live social/co-op, economy, or city-sim scope into Scene 01.

## Validation

Unity batch verifier command:

```sh
"/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/MacOS/Unity" -batchmode -quit -projectPath "$(pwd)/UnityProject/Understory_Unity_Proof" -executeMethod Understory.Editor.UnderstoryProjectVerifier.VerifyReady -logFile "$(pwd)/Reports/unity-verify-ready.log"
```

Verifier result:

- `Understory project verification passed.`

Confirmed by verifier:

- Unity version is `6000.5.2f1`.
- Active build target is iOS on this machine.
- Default graphics pipeline is URP.
- Current quality render pipeline is URP.
- Understory URP asset exists.
- Understory Universal Renderer asset exists.
- Scene 01 starter scene exists.
- Scene 01 is enabled in build settings.

## Next Recommended Goal

```markdown
/goal
Implement Scene 01 visual proof pass 1 in the Unity project.

Repo: `/Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil`
Unity project: `UnityProject/Understory_Unity_Proof`

Read first:
- `SOURCE_OF_TRUTH.md`
- `Docs/START_HERE_UNDERSTORY_HANDOFF_v7.md`
- `Reports/2026-07-03-final-readiness-audit.md`

Goal:
Create the first visible Scene 01 blockout: high mountain summit workyard, damaged shelter/windbreak, broken terrace edge, tiny dead soil bed, shallow material-gathering cut, hidden hatch states, visible Steward placeholder, one or two worker placeholders, and a first Bore Room shell with protected rim plus editable debris/material area.

Scope:
Visual/blockout proof only. Do not build monetization, water sim, combat, multiplayer, character creator, full city sim, or full procedural mountain. Use placeholder geometry/materials where needed, but structure the scene around editability classes and protected shells.

Return one copyable Markdown packet with files changed, validation, screenshots/evidence paths, risks, and next recommendation.
```
