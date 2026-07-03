# UNDERMINE / UNDERSTORY UNITY PROOF SOURCE OF TRUTH v1

Timestamp: 7/3 8:45PM PT

## 0. Purpose

This packet is the new working source of truth for the Unity production proof and for the Chief/Codex operating relationship.

It consolidates:
- `understory-new-repo-handoff-manual-v7.md`
- `understory-new-repo-handoff-pack-v7.zip`
- Material Spectrum Lab closeout packet
- Character Designer production-art spike closeout packet
- Hem Production Art Pipeline final decision packet
- Current Chief decisions from the Hem/Understory pivot

Use this to resume as if the work never stopped.

## 1. Current Title Decision

Working title remains **Understory** unless Matt explicitly renames it.

**Undermine** is a strong candidate title, but not an automatic replacement.

Chief read:
- **Understory** feels mysterious, ecological, emotional, and premium. It carries the world tone better.
- **Undermine** is punchier and immediately communicates digging, but it also means sabotage. It may push the game toward mining/destruction instead of repair/restoration.
- Best current branding posture: use **Understory** as the world/IP name and keep **Undermine** as a title candidate, chapter name, mechanic name, or marketing test.
- Possible title structures:
  - `Understory`
  - `Undermine`
  - `Understory: Undermine`
  - `The Undermine`

Do not hard-lock the final commercial title during the Unity setup phase.

## 2. The North Star

Understory is an iOS-first vertical restoration tapper-builder where a high-altitude people in AD 9026 rediscover a hidden Bore inside their mountain, extract scarce materials from ancient human history below, and use those materials to repair, rebuild, and reclaim the lands and structures above the Veil.

The product should feel:
- premium
- tactile
- human
- mysterious
- grounded
- materially rich
- emotionally hopeful
- vast without becoming unbuildable
- epic in implication, intimate in interaction

The player should feel like they are standing inside a crafted mountain-world and carefully deciding what gets cut, repaired, preserved, and built next.

## 3. Operating SOP: Matt + Chief + Worker Threads

### Chief Role

The Chief is coordinator, not just another worker.

Chief responsibilities:
- preserve source of truth
- decide Active / Ready / Merged / Parked / Rejected / Retired / Preserved Reference
- route every report Matt brings back
- give next action with every decision
- prevent branch/thread sprawl
- preserve useful work before pivots
- block merges that are not explicitly approved
- keep Matt from needing to know where every packet goes

### Thread Routing

Every prompt handed to Matt should say exactly where it goes:
- `Thread: Existing "Material Lab"`
- `Thread: Existing "Character Designer"`
- `Thread: New thread named "Understory Unity Production Proof"`
- `Thread: Chief`

Every worker goal must start with `/goal`.

Keep the main GOAL under roughly 4k characters. Use a supplement only when needed.

### Timestamps

All worker prompts and Chief routing packets must end with a timestamp including date and time, e.g.:

`Timestamp: 7/3 8:45PM PT`

Do not use date-only timestamps.

### Remote Preview Links

When Matt needs to view a local preview while away from the same network, Chief
should expose the already-running local server through a Cloudflare quick tunnel,
not localtunnel.

Required flow:
- first verify the local preview returns `200 OK`
- run a Cloudflare quick tunnel to the local server port
- verify the public Cloudflare URL returns `200 OK` and serves the expected page
- share the Cloudflare URL, not a localhost or LAN-only URL
- say clearly that the link is temporary, public to anyone with the URL, and
  depends on the Mac/Codex session staying awake

Avoid localtunnel for Matt-facing preview links because it may ask viewers for an
IP/password anti-abuse prompt.

### Worker Return Packets

Worker final reports must be one single copyable Markdown packet.

Required return fields:
- branch/base/head
- push status
- final git status
- files changed
- validation
- screenshots/evidence paths
- asset/source/license list when assets are involved
- risks
- merge recommendation
- next recommendation

### Merge Rules

Do not assume a pushed branch is merged.

Do not merge unless Matt explicitly authorizes merge for that branch/task.

Use clean worktrees for merge-only or preservation work. Avoid noisy checkouts with duplicate `... 2` files.

### Preservation Rules

Before any major pivot:
- push the branch
- preserve screenshots/evidence
- preserve source/license manifests
- keep draft PRs open if they are useful references
- do not delete branches
- do not clean duplicate files unless separately approved

### Purchase Rules

Do not buy paid assets, subscriptions, Unity Asset Store packages, Fab/Sketchfab models, Adobe/Substance, or plugins without explicit Matt approval.

Before purchase, return:
- exact source URL
- exact price
- license summary
- why it materially improves the path
- what risk it creates

### Scope Protection

For Hem/Hemstitch:
- stable source of truth was `origin/codex/hem-playable-loop-stable-integration`
- latest known stable head after Tower Floor was `e359b341a54301067246387ffe4deed1cba05893`
- R1 is locked
- R3/R4 stay dev-only/off unless explicitly reopened
- do not edit docs/reports without explicit approval
- restore `MATERIAL_TEST_RESULTS.md` after validation rewrites it

For Unity:
- new repo should be separate
- do not mutate Hem stable
- import lessons and references, not tangled history

## 4. Current Project Dispositions

### Active

**Unity production proof**

Create a new repo:

`Understory-Unity-Proof`

This becomes the production spike, not a branch inside Hemstitch.

### Preserved Reference

**Hemstitch / Three.js prototype**

Use for:
- rounded block feel
- touch/build lessons
- Stitcher input lessons
- material atlas proof
- screenshots/reference
- future flip-back path

Do not keep pushing it toward production while Unity proof is active.

### Preserved Reference

**Material Spectrum Lab**

Repo:

`/Users/matthewgrossman/Documents/GitHub/Hemstitch-material-spectrum-lab`

Branch:

`codex/hem-material-spectrum-lab`

Final closeout head:

`2555af752427791cc7f995069455955a56cf6d7b`

PR:

`https://github.com/matt-bamboo/Hemstitch/pull/5`

Status:
- preserved
- draft
- not merged
- clean
- refreshed onto stable `e359b341`

Use it as:
- material identity proof
- CC0 source/license proof
- compact atlas proof
- Hem flip-back path

Do not merge wholesale.

### Preserved Reference / Parked

**Character Designer production-art spike**

Repo:

`/Users/matthewgrossman/Documents/GitHub/Hemstitch-character-v2-production-art-spike`

Branch:

`codex/character-v2-production-art-spike`

Final head:

`96d9f19441634635fe617abd9f180a1a5f858c52`

Status:
- pushed
- no PR
- not merged
- preserved reference

Use it as:
- GLB import proof
- asset/license proof
- evidence that current Character Designer should not be final art

Do not merge.

### Rejected / Retired

- current Hem Character Designer as final production art
- rounded-box procedural Character V2 as final art path
- broad manual material shader/palette tweaking as main strategy
- Kenney Blocky as final art
- Poly Pizza Worker as final art
- breathing mountain language
- old `Old Lungs / Breathlines / Breathstone` terminology

## 5. Hem Proofs To Preserve

### Material Proof

Final Material Lab evidence:

`/Users/matthewgrossman/Documents/GitHub/Hemstitch-material-spectrum-lab/artifacts/rounded/audit-runs/material-spectrum-closeout-final/screenshots/desktop-materialSpectrumLab-spectrum-hero.png`

Final atlas source manifest:

`/Users/matthewgrossman/Documents/GitHub/Hemstitch-material-spectrum-lab/artifacts/rounded/public/material-lab/atlas/hem-hero-material-atlas-sources.json`

Key result:
- atlas budget: `1.8 MB`
- desktop FPS: `121`
- iPad FPS: `120`
- phone FPS: `119`
- audit: `8 pass / 0 warn / 0 fail`

Material sources:
- ambientCG `Bricks097`, CC0
- Poly Haven `aerial_rocks_02`, CC0
- Poly Haven `aerial_grass_rock`, CC0
- Poly Haven `bark_brown_02`, CC0
- Poly Haven `blue_metal_plate`, CC0
- Poly Haven `book_pattern`, CC0
- generated Hem glass tile
- generated Hem glow tile

### Scratch Material Proof

`/tmp/hem-production-art-proof/production-art-proof-cc0-pbr.png`

Use this as a proof that real PBR maps can make brick/steel/wood/cloth/grass read materially on Hem-like rounded blocks.

### Character Proof

`/Users/matthewgrossman/Documents/GitHub/Hemstitch-character-v2-production-art-spike/artifacts/rounded/screenshots/character-v2-production-art-spike/final-character-route-comparison.png`

Key result:
- current primitive designer failed final art target
- Kenney Blocky failed final art target
- Poly Pizza Worker failed final art target
- imported assets are useful as pipeline smoke tests only

## 6. Final Engine Decision

Proceed with **Unity 6 + URP production proof**.

Reason:
- Hem/Three material proof passed preservation, but combined character + lighting + world-building + production workflow is more likely to move faster in Unity.
- Unity should not rebuild the whole game yet.
- Unity should prove one premium vertical slice.

Do not buy Unity asset packs on day one.

Do not switch to a full voxel survival kit immediately.

First prove the architecture with free/CC0 assets and small custom systems.

## 7. New Repo Plan

Create a new GitHub repo:

`Understory-Unity-Proof`

Reason:
- clean history
- no Hem branch clutter
- Unity project files do not pollute Hemstitch
- Hem remains a flip-back reference
- new repo can become production if the proof works

Suggested local path:

`/Users/matthewgrossman/Documents/GitHub/Understory-Unity-Proof`

Initial folders:

```text
Understory-Unity-Proof/
  README.md
  SOURCE_OF_TRUTH.md
  References/
    Visual/
    Characters/
    HemProofs/
    Docs/
  UnityProject/
  DecisionLog/
  Reports/
  Prompts/
```

Do not place Unity project inside Hemstitch.

## 8. Unity Setup Reality Check

Known local setup as of this packet:
- Unity app not found in `/Applications`
- Blender not found
- Substance not found
- Xcode app not found
- Apple Command Line Tools found at `/Library/Developer/CommandLineTools`

First setup tasks:
- install Unity Hub
- install Unity 6
- include Universal Render Pipeline template/support
- include iOS Build Support
- install full Xcode
- open Xcode at least once
- create `Understory_Unity_Proof`

TestFlight is not day-one work.

## 9. Core Product Definition

Understory is a premium iOS vertical restoration builder where the player, a visible Summit Steward in AD 9026, sends crews through a rediscovered Bore, extracts real materials from ancient human layers below, and uses those materials to repair and freely rebuild livable land above the Veil.

Game lane:
- tapper-builder
- idle-supported
- authored-world
- broad editable zones
- protected shells
- extraction/destruction where it matters
- premium miniature material finish

Not:
- Minecraft clone
- combat RPG
- walking sim
- console platformer
- full survival sim
- infinite procedural terrain
- static diorama

The best phrase:

**A tapper-builder with an epic fossil record.**

## 10. Story So Far

In AD 9026, humanity survives in thin air on a high mountain above the Veil. The lower world has begun recovering through the Long Cooling and old Drawdown systems, but generations of fear, haze, broken machinery, and failed descents have kept the summit people above.

Local summit workers are gathering scarce repair material near the summit. They uncover a hidden hatch. The Steward is called. The hatch opens into the Bore Room: an ancient constructed chamber around a vertical shaft descending through the mountain.

The player was not looking for destiny. The settlement needed material. The hatch changes everything.

From there, the player descends, extracts, returns, refines, repairs, builds, waits, and returns again. Material from below makes life above possible.

The long mystery asks:

**Are we restoring the mountain, or are we being invited to repeat it?**

## 11. Tone Lock

The world is not post-apocalyptic misery. It is patient, strange, materially hopeful, and human.

Damage is not erased. It is:
- mended
- reinforced
- lit
- inhabited
- made beautiful

Tone should be:
- grounded but wondrous
- warm under pressure
- handmade, not primitive
- technically mysterious, not whimsical
- materially rich, not plastic
- human-scale, not chosen-one fantasy

Avoid:
- goofy magic jargon
- breathing mountain language
- plastic toy tone
- survival misery
- ancient-aliens framing
- lore dumps before the loop lands

## 12. Canon Locks

- Working title: `Understory`; `Undermine` is a serious title candidate.
- Standalone IP using Hem/Hemstitch DNA.
- Present date: AD 9026.
- Player role: visible Summit Steward.
- Platform: iOS first, iPhone + iPad.
- Locomotion: soft tap-to-move between meaningful nodes.
- No joystick.
- No combat.
- No platforming.
- No full adventure controller.
- Surface identity: future-collapse history experienced as livable recovery.
- Underground identity: ancient-first human history from c. 8000 BC to AD 2000.
- Before-Us / Otherhuman remains late-game mystery, not Phase 0.
- Recent ancestor / Hollower material is overlay, not primary architecture.
- Monetization: subscription + expansions + cosmetics, no exploitative scarcity.
- Ads: no ads at launch.
- Paid skips: no paid skips at launch.
- Coin/gem economy: maybe later, parked, not Phase 0.

## 13. Deprecated Terms

Do not use:
- Old Lungs
- Breathstone
- Breathlines
- the mountain breathes
- breathing mountain
- wake the mountain's lungs
- world lungs

Approved machine/folk-engineering language:
- The Works
- Clearers
- Mist Engines
- The Lines
- Black Vaults
- Filterstone
- Carbonate Brick
- Blackglass
- Intake Ceramic
- Scrub Ceramic

Machines can:
- clear sections of the Veil
- stabilize air
- reveal land
- move water
- filter pollutants
- reconnect old infrastructure

They are old environmental machines hidden in the mountain: practical, massive, half-understood, and dangerous if mishandled.

## 14. Core Loop

The locked loop:

**descend -> extract -> ascend -> refine -> build -> wait -> return**

### Descend

Crews move through the Bore and authored chambers.

### Extract

The player makes hands-on decisions:
- trace a seam
- blast for bulk
- place shoring
- inspect caches
- stabilize unsafe cuts
- preserve rare finds

### Ascend

The haul returns physically through the Return Ritual.

### Refine

Raw material becomes buildable material through refiners: kiln first, sawmill/forge/cutter later.

### Build

The player repairs anchors and places/removes/commits blocks in broad editable zones.

### Wait

Crews and refiners continue offline within caps.

### Return

Every return should have a reason: haul, note, hard stop, draft need, artifact, repair, exposed region.

## 15. World Architecture

Final architecture:

**Vast authored mountain world + broad editable/buildable regions + protected structural shells + simulated extraction/destruction volumes.**

Not:
- tiny editable pockets only
- full infinite procedural Minecraft mountain
- pure static diorama

The mountain shape is authored. The Bore Room is authored. Major shells and landmarks are protected. Ruins, terraces, structures, build pads, extraction faces, repair anchors, and many interior blocks are editable.

Most things should be editable unless explicitly structural, boundary, or protected landmark geometry.

## 16. Editability Classes

Every world object must have an editability class.

| Class | Name | Editable? | Examples |
|---|---|---:|---|
| A | Protected shell | No | mountain silhouette, distant cliffs, ocean/world boundary, main Bore shaft shell |
| B | Protected landmark | Not casually | hatch, Bore Room rim, first hoist, Core Sample, Archive shelf |
| C | Destroyable ruin | Mostly yes | old walls, damaged shelters, debris, abandoned supports |
| D | Player-built | Yes | committed blocks, repaired walls, shoring, platforms |
| E | Extraction volume | Yes | stone faces, clay pockets, mineral seams, buried caches |
| F | Repair anchor | Partially | shelter damage, terrace wound, dead soil bed, hatch obstruction |

Production rule:

**If an object is not explicitly protected, assume it can be altered.**

## 17. Phase 0 Unity Scene

Scene:

**Scene 01: Summit Hatch + Bore Room Proof**

Required surface:
- high mountain summit workyard
- visible Veil below
- damaged shelter/windbreak
- broken terrace edge
- tiny dead soil/garden bed
- shallow material-gathering cut
- visible Summit Steward
- one or two worker figures/silhouettes
- stone, clay, timber, crates, tools

Required hatch states:
1. covered by mineral crust/debris
2. partially exposed
3. opened
4. transition down into Bore Room

Required Bore Room:
- ancient constructed access chamber
- vertical shaft dropping beyond view
- broken guide rings or lift rails
- silent machine fragments
- old Lines / Clearer / Mist Engine hint, inactive
- Hollower warning mark over older stone
- extraction volume or material cache
- protected shaft rim
- editable debris area

First repair result:
- shelter/windbreak improves
- terrace edge stabilizes
- tiny soil bed becomes viable
- player sees material from below repair life above

## 18. Art Direction

Runtime target:

Middle panel of the reference image: **Real Materials**.

Use:
- real materials
- tactile surfaces
- cloth, wood, stone, glass, ceramic, metal
- soft rounded forms
- mobile-readable silhouettes
- premium miniature finish

Capture/marketing target:

Right panel of the reference image: cinematic capture.

Use:
- cinematic lighting
- depth
- bloom
- atmosphere
- golden hour or lamplit mood
- screenshot-ready framing

Avoid:
- plastic toy look
- saturated shiny blocks
- flat toy lighting
- cheerful mascot tone

## 19. Character Direction

Core people:

**High-Air Heirloom Miniatures**

They are:
- small stylized humans
- wrapped/masked/high-altitude adapted
- material-rich
- cloth, ceramic, leather, glass, worn metal
- readable but not mascot
- emotional through posture, gear, notes, and props

First archetypes:
- Shorer / Mason
- Breathline Mechanic is deprecated by language, replace with Works Mechanic / Lines Mechanic / Clearer Mechanic
- Seed-Keeper
- Far-Glass Surveyor
- Patchworker

Character Designer lesson:

Do not try to reach final art through rounded-box procedural characters.

Unity character path:
- use placeholder only for the proof
- then commission or build one custom Mason/Shorer hero model
- require source files, clean topology, GLB/FBX, texture maps, and modular accessories

## 20. Interaction Requirements

Steward movement:
- soft tap-to-move between nodes
- no joystick
- no platforming
- no combat

Nodes:
- repair cluster
- shallow cut / hatch site
- hatch
- haul table
- Bore Room inspection point
- material cache
- Archive shelf placeholder

Camera modes:
1. Builder camera: orbit/zoom/edit
2. Steward presence camera: closer, avatar standing in real spaces
3. Capture camera: cinematic screenshots/trailers

## 21. Build / Destruction / Extraction Requirements

Broad editable zones:
1. surface repair/build zone
2. interior Bore Room editable/extraction zone

Player building proof:
- place rounded block
- remove placed block
- ghost preview
- commit block
- material count decrement
- repair anchor accepts required material

Extraction proof:
- trace extraction
- blast extraction
- shoring/support
- instability
- collapse/burial
- re-excavation later

Collapse should bury, not delete.

Fake deep math if needed, but the player must feel the difference between careful trace and bulk blast.

## 22. Six-Week Unity Gauntlet

### Weeks 1-2: Visual Proof

Goal:

Does this look like a premium tactile miniature world, not a toy voxel demo?

Deliver:
- Unity URP project
- Summit Hatch scene
- Bore Room scene
- Steward placeholder
- material shader/material tests
- runtime lighting target
- capture lighting target
- surface repair cluster blockout
- protected shell vs editable ruin classification pass

### Weeks 3-4: Interaction Proof

Goal:

Can the player physically inhabit and alter this world?

Deliver:
- soft tap-to-move
- hatch reveal
- editable surface repair cluster
- editable ruin pieces
- first broad build zone
- first interior editable zone
- block placement/removal
- protected shell rules
- extraction volume with trace/blast
- first haptic seam test

### Weeks 5-6: Loop Proof

Goal:

Does descend, extract, ascend, refine, build, wait, return work as a satisfying loop?

Deliver:
- haul table
- Return Ritual v1
- material inventory
- first refiner/kiln
- repair cluster consumes materials
- ghost draft / want-list stub
- crew job duration selector
- short offline timer simulation
- first hard stop note
- repeatable return loop

## 23. Unity Proof Acceptance Criteria

Visual:
- runtime looks closer to real-material miniature than plastic toy
- cinematic capture is achievable with same assets
- Steward feels human enough, not mascot
- Bore Room feels built, old, deep, and practical

Spatial:
- player can see/feel large spaces
- Steward can stand inside structures
- surface has broad buildable land
- interior has meaningful editable areas
- protected shell rules prevent world-breaking

Interaction:
- ruin pieces can be removed/rebuilt
- player-built blocks are editable
- extraction face can be cut/traced/blasted
- shoring matters
- collapse buries rather than destroys

Loop:
- material from below repairs surface above
- first repair cluster visibly improves
- Return Ritual makes haul feel physical
- wait/return has a reason
- no store/monetization appears

## 24. Do Not Build Yet

Do not build:
- monetization
- subscription
- full character creator
- named crew store
- full water/flood system
- full Archive hall
- full Core Sample
- full age ladder
- Far Peak civilization
- procedural mountain
- multiplayer
- combat
- hunger/survival
- Android
- full city sim
- huge quest system

Seed only:
- Archive shelf placeholder
- Core Sample placeholder
- inactive Works/Mist Engine hint
- Veil atmosphere
- Far-Glass prop only if easy

## 25. Open Decisions

High priority:
- final title: Understory vs Undermine vs other
- final field tool name: Line-Key, Trace-Key, Survey Key, etc.
- Steward specialness: inherited craft-language is best current proposal
- exact character production path: commissioned hero model vs paid pack base
- exact Unity package purchase list after proof

Mechanics:
- how much tap-to-move vs node-presence
- how shoring/collapse math works
- how broad editable zones are stored
- how drafts and want-lists persist
- how offline timers are capped

Business:
- subscription timing
- Founder/Patron offer
- cosmetic boundaries
- whether coin/gem economy ever returns

## 26. First Prompt For New Repo

Thread:

New thread named `Understory Unity Production Proof`

Action:

Paste this after the new repo exists and the handoff pack is copied in.

```markdown
/goal
Understory Unity Production Proof setup and Scene 01 plan.

Repo: `/Users/matthewgrossman/Documents/GitHub/Understory-Unity-Proof`

Read first:
- `SOURCE_OF_TRUTH.md`
- `References/Docs/START_HERE_UNDERSTORY_HANDOFF_v7.md`
- `References/HemProofs/` if present
- `References/Visual/Generated image 1 (1).jpeg`

Goal:
Verify Unity setup and create the first implementation plan for Scene 01: Summit Hatch + Bore Room Proof.

Do not build the whole game.
Do not buy paid assets.
Do not touch Hemstitch.
Do not create monetization, combat, multiplayer, procedural mountain, or full city sim.

Tasks:
1. Verify local Unity / Unity Hub / Unity version / iOS Build Support / Xcode availability.
2. If Unity is not installed, return exact setup blocker and install checklist.
3. If Unity is installed, create/open a Unity 6 URP project under `UnityProject/`.
4. Create or verify repo folders:
   - `References/`
   - `DecisionLog/`
   - `Reports/`
   - `Prompts/`
5. Produce a Scene 01 build plan with:
   - visual proof deliverables
   - editability class architecture
   - protected shell vs editable zone model
   - required placeholder assets
   - validation/evidence plan

Return one single copyable Markdown packet with:
- repo path
- Unity setup state
- project path
- blockers
- files/folders created if any
- Scene 01 implementation plan
- next recommended `/goal`

Timestamp: 7/3 8:45PM PT
```

## 27. Preservation Checklist Before Unity

Before serious Unity implementation starts, preserve:
- this packet
- `understory-new-repo-handoff-pack-v7.zip`
- Material Lab PR #5
- Character production-art spike branch
- Hem Production Art Pipeline final decision packet
- reference image `Generated image 1 (1).png/jpeg`
- scratch material proof screenshot
- scratch character proof screenshot

Do not close or delete preserved branches yet.

## 28. Final Success Statement

The first proof succeeds when:

A local worker uncovers the hatch.
The Steward opens it.
The Bore Room drops into darkness.
The player extracts real material.
The haul returns.
The surface repair cluster becomes more livable.
The player understands:

**This mountain can be rebuilt because of what is below.**
