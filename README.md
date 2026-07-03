# Understory Below the Veil

Clean production-proof repo for Understory, an iOS-first vertical restoration
tapper-builder using Hemstitch DNA as reference, not canon.

## Start Here

Read these in order:

1. `SOURCE_OF_TRUTH.md` - current Chief/Codex source of truth for the Unity proof.
2. `Docs/START_HERE_UNDERSTORY_HANDOFF_v7.md` - full canon, product, mechanics,
   art direction, Unity plan, and open decisions.
3. `Docs/Archive/` - older context only. Newer files supersede contradictions.

## Active Target

First deliverable:

`Scene 01: Summit Hatch + Bore Room Proof`

The first slice is a tightly scoped Unity 6 + URP proof:

- damaged summit repair cluster
- workers uncovering a hidden hatch
- visible Summit Steward
- Bore Room reveal
- first extract / haul / refine / repair loop
- broad editable/buildable zones with protected structural shells

Do not build monetization, a full character creator, water sim, combat,
multiplayer, Android, full procedural terrain, or a full production game in this
proof.

## Language Locks

Use:

- The Works
- Clearers
- Mist Engines
- The Lines
- Black Vaults

Do not revive current-design language around Old Lungs, Breathstone,
Breathlines, or the mountain breathing.

## Setup State

As of `7/3 10:47AM PT`:

- GitHub remote exists: `https://github.com/matt-bamboo/Understory-Below-the-Veil`
- `main` is pushed and tracking `origin/main`
- Unity Hub is installed locally
- Unity Editor `6000.5.2f1` is installed
- Unity iOS Build Support is installed
- Unity project exists at `UnityProject/Understory_Unity_Proof`
- URP `17.5.0` is active for the project
- Scene 01 starter scene exists at
  `UnityProject/Understory_Unity_Proof/Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity`
- Unity batch verification passes
- Xcode `26.6` is installed at `/Applications/Xcode.app`
- Xcode license check passes
- Xcode first-launch setup passes
- Xcode iOS SDK and iOS Simulator SDK `26.5` are installed
- iOS Simulator runtime `26.5` is installed
- iPhone and iPad simulator devices are available
- Apple Command Line Tools are installed

Gameplay implementation can begin in Unity. iOS device signing/provisioning is
future Apple developer-account work; the local install/setup gate is cleared.
