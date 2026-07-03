# UNDERSTORY NEW REPO HANDOFF MANUAL v7
## Working title: Understory
### Canon, Product, Mechanics, Monetization, Art Direction, Unity Production Plan, and Open Decisions
### Prepared for the new Codex repo handoff
### Date: 3 Jul 2026

---

## 0. Read this first

This document is the current source of truth for the new Understory repo.

It consolidates:

- Understory Scope v5.
- Understory Master Scope v6.
- Understory Codex Mockup Scope v6.
- Understory Decision Register v6.
- Character Builder / High-Air Heirloom Miniature direction.
- Later Q&A decisions made after those documents.
- The Unity production pivot.
- The latest architecture correction: broad editable world zones with protected shells, not a fully fake diorama and not a full procedural Minecraft mountain.

This manual **supersedes contradictory language in v5/v6**, especially the old “breathing mountain / Old Lungs / Breathlines / Breathstone” terminology.

### The new north star

> **Understory is an iOS-first vertical restoration tapper-builder where a high-altitude people in AD 9026 rediscover a hidden Bore inside their mountain, extract scarce materials from ancient human history below, and use those materials to repair, rebuild, and reclaim the lands and structures above the Veil.**

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

The player should not feel like they are playing a Minecraft clone. They should feel like they are standing inside a crafted mountain-world and carefully deciding what gets cut, what gets repaired, what gets preserved, and what gets built next.

---

## 1. Source document status

The older docs are still useful, but several items are now obsolete. Use this table before following any previous instruction.

| Source | Use | Current status |
|---|---|---|
| `understory-scope-v5.md` | Mechanical locks: core loop, scarcity, drafting, collapse-burial, Return Ritual, Core Sample, careful extraction, water, light, Archive | Still foundational. Some HEM-canon and World Core framing is outdated. |
| `understory-master-scope-v6.md` | AD 9026, Veil, Long Cooling + Drawdown Works, age ladder, monetization scaffold | Mostly useful, but **Old Lungs / Breathlines / Breathstone / mountain breathes** language is replaced. |
| `understory-codex-mockup-scope-v6.md` | First 10-minute mockup, interaction prototypes, economy scaffold | Useful, but update first underground discovery and machine terminology. |
| `understory-decision-register-v6.pdf` | Decision context and open items as of v6 | Superseded by the Q&A locks listed here. |
| `understory-character-builder-codex-scope-v1.md` | Character-builder retarget direction | Still useful. Use High-Air Heirloom Miniatures as the core character art lock. |
| Character concept images | Visual references | Use with caution labels. Some images are positive targets, others are caution references. |

### Do not accidentally revive old language

The previous docs repeatedly used:

- Old Lungs
- Breathlines
- Breathstone
- mountain breathes
- breathing mountain
- Breathwrights as a likely ancient culture name

Those terms are now **deprecated** unless deliberately used as obsolete folk myth in a future story pass. For current Codex work, replace them with the Future Folk-Engineering language below.

---

## 2. Current locked decision ledger

These are the latest locks from the Q&A sequence.

| # | Decision | Lock |
|---:|---|---|
| Q1 | Relationship to HEM | **Understory is a standalone IP using HEM/Hemstitch DNA.** HEM is donor DNA, not canon. |
| Q2 | Title status | **Understory is the working title**, not final branding. |
| Q3 | Present date | **AD 9026.** |
| Q4 | Underground identity | **Ancient-first human history, c. 8000 BC to AD 2000**, with authored capability layers, not textbook history. |
| Q5 | Surface identity | **Future-collapse history experienced as livable recovery.** |
| Q6 | Veil mechanics | **Visual/progression wall early; light hazard vocabulary later.** |
| Q7 | Surface build model | **Repair anchors plus free build.** |
| Q8 | Material model | **Two-layer materials: functional family + era identity.** |
| Q9 | Tapper math posture | **Hybrid authored cadence.** Player-chosen job bands. |
| Q10 | Monetization spine | **Subscription + expansions + cosmetics**, no exploitative scarcity. |
| Q11 | Monetization rollout | **Founder/Patron first, subscription second, expansions third.** |
| Q12 | Named crew model | **Core crew earned; optional patron crew sold directly.** No gacha. |
| Q13 | Paid crew strength | **Personality/convenience first; tiny capped bonuses allowed.** |
| Q14 | Paid skips | **No paid skips at launch.** Subscription convenience first. Refine-only skips may be revisited later. |
| Q15 | Ads | **No ads at launch.** Revisit only if needed. Never interstitial. |
| Q16 | Character art | **Core = High-Air Heirloom Miniatures.** Optional cosmetics can stretch but not break tone. |
| Q17 | Character builder rollout | **Authored crew first. Crew roster customization later. Light player customization after that.** No full dress-up launch. |
| Q18 | Player identity | **Visible in-world Summit Steward avatar.** |
| Q19 | Avatar behavior | **Soft tap-to-move between important nodes.** No joystick, combat, platforming, or full adventure controls. |
| Q20 | Underground play | **Idle dig plus authored playable chambers.** |
| Q21 | Phase 0 proof | **A + B staged:** pure core loop plus first emotional restoration. |
| Q22 | First surface repair anchor | **Small combo cluster: shelter/windbreak + terrace wall + tiny garden.** |
| Q23 | First underground discovery | **Local workers gathering scarce material uncover hidden hatch, revealing the Bore Room.** |
| Q24 | Machine terminology | **Future Folk-Engineering language:** The Works, Clearers, Mist Engines, The Lines, Black Vaults. |
| Q25 | Player specialness / tool | **Open.** Proposed direction: inherited craft-language + Hollower ancestry through work, not magic bloodline. |
| Q26 | Production platform | **Unity 6 + URP for iOS production proof.** Locked unless a technical spike proves a blocker. |
| Q27 | World architecture | **Vast authored mountain world, broad editable/buildable zones, protected structural shells.** |
| Q28 | Steward locomotion | **Soft tap-to-move is current lock, but spike-test feel early.** Presence-only fallback if it slows the loop. |
| Q29 | Traced seam haptics | **Move into early interaction proof.** Do not treat as late polish. |

---

## 3. One-sentence product definition

> **Understory is a premium iOS vertical restoration builder where the player, a visible Summit Steward in AD 9026, sends crews through a rediscovered Bore, extracts real materials from ancient human layers below, and uses those materials to repair and freely rebuild livable land above the Veil.**

---

## 4. One-paragraph pitch

In AD 9026, humanity survives in thin air on a high mountain above the Veil, a vast lowland layer of cloud, toxic haze, storm residue, machine failure, and myth. The lower world has begun recovering through the Long Cooling and ancient Drawdown systems, but the summit people cannot see or safely reach it. While gathering scarce repair material, local workers uncover a hidden hatch under the summit. Below it is the Bore Room: an ancient constructed access chamber around a vertical shaft descending into the mountain. The player, the Summit Steward, reopens this route, sends crews into ancient human layers, extracts stone, clay, metal, glass, and machine materials, and uses them to repair the shelter, terraces, gardens, ruins, conduits, and future homes above. Outside, the settlement carves downward into newly recoverable land. Inside, the dig uncovers the rise of human capability from c. 8000 BC to AD 2000, with recent ancestor overlays and a deeper Before-Us mystery held for later.

---

## 5. Tone lock

> **The world is not post-apocalyptic misery. It is patient, strange, materially hopeful, and human. Damage is not erased. It is mended, reinforced, lit, inhabited, and made beautiful.**

This is still locked.

Tone should be:

- grounded but wondrous
- warm under pressure
- handmade, not primitive
- technically mysterious, not whimsical
- materially rich, not plastic
- human-scale, not chosen-one fantasy

Avoid:

- goofy magic jargon
- “the mountain breathes” language
- plastic toy tone
- survival misery
- full console adventure drift
- lore dumps before the loop lands
- ancient-aliens framing
- real-world ancient culture replacement claims

---

## 6. Product lane

Understory is still a tapper-builder / idle-builder hybrid.

It is not:

- Minecraft
- a console RPG
- a combat adventure
- a walking sim
- a pure idle clicker
- a city builder with lore pasted on
- a full procedural voxel survival game

It is:

- iOS-first
- touch-first
- build/edit/extract-first
- timer-supported, not timer-dominated
- authored-world, editable-zone-based
- visually premium
- emotionally patient
- built around scarcity that feels honest

The story can be enormous. The sessions should remain small, repeatable, and satisfying.

The correct phrase is:

> **A tapper-builder with an epic fossil record.**

---

## 7. The core circulation loop

The locked loop remains:

> **descend → extract → ascend → refine → build → wait → return**

### Descend

Crews move through the Bore and authored chambers. Most progress can happen on timers, but meaningful stops call the Steward.

### Extract

The player makes hands-on decisions:

- trace a seam carefully
- blast for bulk
- place shoring
- inspect a cache
- stabilize an unsafe cut
- preserve a rare find
- choose what gets removed and what stays

### Ascend

The haul returns physically through the Return Ritual.

### Refine

Raw material becomes buildable material through simple refiners: kiln first, then sawmill/forge/cutter equivalents later.

### Build

The player repairs anchors and places/removes/commits blocks in broad editable zones.

### Wait

Crews and refiners continue offline within caps.

### Return

The return should always have a reason: haul, note, hard stop, draft need, artifact, surface repair, new exposed region.

---

## 8. Player identity

### Locked

The player has a visible in-world avatar.

Working role:

> **The Summit Steward**

The Steward is not a ruler, prophet, superhero, or invisible god-hand. They are a hands-on repair lead and expedition authority trusted to reopen dangerous sites, assign crews, inspect strange finds, authorize cuts, and decide how scarce material is used.

The Steward can soft tap-to-move between important nodes:

- hatch site
- Bore Room inspection point
- repair anchor
- haul table
- kiln/refiner
- Archive shelf
- Core Sample placeholder
- Far-Glass
- crew staging point

No joystick. No combat. No platforming. No direct-control adventure. No precision navigation puzzles.

### Movement feel

The Steward should move like someone adapted to thin air and dangerous edges:

- slow enough to feel real
- careful footfalls
- bracing in wind
- small pauses near drops
- purposeful routes
- no bouncy mascot movement

### Player specialness is still open

The next canon decision should pin down why the Steward can use the field tool / “zapper.”

Current best proposal:

> The Steward is trained in inherited Summit craft-language: seam marks, line-reading, repair gestures, knot logic, terrace measuring, and old safety rituals. These traditions unknowingly preserve degraded operating gestures from the mountain’s older systems. The tool responds because the Steward knows the old patterns through culture and training, not because of magic blood.

Possible late reveal:

> Those craft traditions came through the Hollowers, recent ancestors who once occupied the Bore and sealed it. The player is connected to the past through work, not destiny.

### What to avoid

Do not make the player special because:

- unique DNA
- chosen one prophecy
- mountain chose them
- reincarnation
- magic bloodline
- “only you can hear the Bore”
- zapper is a weapon

---

## 9. The tool / zapper / Line-Key open item

The user has called it the “zapper” informally, but that should not become player-facing language without a conscious decision.

### Working functional definition

The tool is a field instrument that lets the Steward:

| Verb | Player action | Game meaning |
|---|---|---|
| Read | hover/scan/reveal lines | reveals seams, instability, repair anchors, old marks, ghost maps |
| Cut | trace or blast | extracts material, opens seams, removes pieces |
| Set | place/commit/repair | turns draft blocks and repair anchors into real structures |

It should not be a weapon. It should not feel like a magic wand. It should feel like an inherited survey/repair/extraction instrument with ancient interface compatibility.

### Name candidates

| Candidate | Notes |
|---|---|
| Line-Key | Best current proposal: links seam tracing, The Lines, and unlocking old systems. |
| Trace-Key | Very clear, touch-forward. |
| Survey Key | Grounded, less iconic. |
| Cutkey | Gritty, maybe too harsh. |
| Handpiece | Practical, but weak as a brand object. |
| Tether | Emotional, maybe too soft. |
| The Mark | Strong but vague. |

### Status

**Open.** Do not hardcode a final name in UI. Use `field tool`, `tool`, or internal placeholder until locked.

---

## 10. World canon

### Present

- Current date: **AD 9026**.
- Humanity lives on a great high mountain above the Veil.
- The people are culturally rich, materially reduced, and adapted to high-altitude life.
- The lower world has been hidden by the Veil for generations.
- The planet has recovered more than humanity has.

### The Veil

The Veil is not carbon dioxide. It is visible and dangerous:

- cloud
- haze
- toxic weather
- industrial aerosol residue
- machine exhaust
- storm inversion
- humidity
- spores or biological haze if needed later
- cultural fear built from real failed descents

Mechanically:

- Phase 0: visual/progression wall.
- Phase 1+: may become light hazard vocabulary.
- No early gas meters, survival oxygen bars, or punitive exposure systems.

### The Long Cooling + Drawdown systems

The world’s recovery comes from both:

- **The Long Cooling:** planet-scale background, long climate rhythm.
- **The Drawdown systems:** old atmospheric/environmental machines that still operate in damaged, partial, or misaligned ways.

The machines helped the world recover, but their damaged operations also contributed to or sustained the Veil.

### The mountain as memory trap

Civilizations keep rediscovering the mountain when the world becomes desperate.

The question under the whole game:

> **Are we restoring the mountain, or are we being invited to repeat it?**

---

## 11. New machine terminology: Future Folk-Engineering

Q24 replaced the old breathing language.

### Approved vocabulary lane

Use these terms:

| Concept | Preferred language |
|---|---|
| ancient environmental machine network | **The Works** |
| Veil-clearing devices | **Clearers** |
| old atmospheric/environmental machines | **Mist Engines** |
| pipes / ducts / conduits | **The Lines** |
| sealed carbon / filtration / machine vaults | **Black Vaults** |
| machine repair nodes | **Works stations / Clearer stations** |
| machine materials | **Filterstone, Carbonate Brick, Blackglass, Intake Ceramic, Scrub Ceramic** |

### Deprecated terms

Do not use as current design language:

- Old Lungs
- Breathstone
- Breathlines
- the mountain breathes
- breathing mountain
- wake the mountain’s lungs
- world lungs
- breath as sacred machine language

### Revised machine fantasy

The machines can:

- clear sections of the Veil
- stabilize air in lower terraces
- reveal land
- make old paths safe enough to reclaim
- move water
- filter pollutants
- power or connect old infrastructure
- reactivate environmental systems

Describe them as:

> **Old environmental machines hidden in the mountain. Practical. Massive. Half-understood. Dangerous if mishandled. Useful if repaired.**

Not as:

> “The mountain breathes again.”

---

## 12. World geometry and spatial model

### The world

One great mountain surrounded by ocean, forested lowlands, and the Veil.

### Outside: the Carve

The surface journey descends the mountain.

- Player begins at the summit.
- New terraces are revealed as the Veil clears.
- Surface history reads as future-collapse history in reverse.
- Emotionally, each terrace is new livable recovery.

### Inside: the Bore and Hollow

The Bore is the found vertical shaft. The Hollow is the deep interior foothold / cavern system.

- Crews descend through the Bore.
- They establish footholds below.
- The player digs/builds upward through ancient human history.
- Interior spaces should eventually feel vast and inhabitable.

### Final architecture correction

The latest direction is **not** just tiny simulated pockets inside static dioramas.

The current architecture is:

> **Vast authored mountain world + broad editable/buildable regions + protected structural shells + simulated extraction/destruction volumes.**

The player should feel like most constructed things can be removed, edited, rebuilt, repaired, or replaced unless explicitly protected.

---

## 13. Editability hierarchy

Every object should have an editability class.

| Class | Name | Editable? | Examples |
|---|---|---:|---|
| A | Protected shell | No | mountain silhouette, distant cliffs, outer world boundary, main Bore shaft shell, major chamber containment walls |
| B | Protected landmark | Not casually | hatch, Bore Room rim, first hoist, Core Sample, Archive shelf, key story machinery |
| C | Destroyable ruin | Mostly yes | broken shelters, old walls, damaged terrace edge, debris, abandoned constructed pieces, non-critical supports |
| D | Player-built | Yes | committed blocks, repaired walls, shoring, platforms, terraces, shelter extensions |
| E | Extraction volume | Yes | stone faces, clay pockets, mineral seams, buried caches, weak ceiling pockets |
| F | Repair anchor | Partially | shelter damage, terrace wound, dead soil bed, hatch obstruction, future Clearer station |

### Production rule

> **If an object is not explicitly protected, assume it can be altered.**

### Protected boundaries

Players should not destroy:

- outer mountain silhouette
- hard exterior cliffs / world boundary
- main Bore shell
- critical chamber containment walls
- critical navigation walls
- protected story landmarks

### Editable by default

Most starting structures should be modular, destructible, removable, and rebuildable:

- surface ruins
- shelter pieces
- terrace blocks
- interior walls
- debris
- player-built structures
- extraction faces
- shoring

---

## 14. Age ladder and canon timeline

### Major timeline

| Layer | Approx date | Who | Role |
|---|---:|---|---|
| Before-Us / Otherhuman | c. 5,000,000 BC, flexible | not Homo sapiens, but human-like in behavior/culture | original Bore mystery, late reveal |
| Ancient human origin layer | c. 8000 BC onward | ancient human ritual-engineers / first stewards | primary underground architecture and early machine interface |
| Seed / calendar cultures | c. 6500 BC to 4500 BC | ancient stewards | life preservation, water, food, seasons |
| Monument civic cultures | c. 4500 BC to 2500 BC | ancient public builders | grand halls, public works, memory architecture |
| Furnace / alloy cultures | c. 2500 BC to 800 BC | fire and metal workers | metal, forge, hoists, braces |
| Road / water / law cultures | c. 800 BC to AD 500 | civic infrastructure cultures | roads, cisterns, channels, standards |
| Quiet Orders | AD 500 to AD 1400 | preservers | Archive, manuscripts, lamps, map rooms |
| Glass / mechanism cultures | AD 1400 to AD 1750 | observers and instrument makers | lenses, Far-Glass roots, gauges |
| Steam / smoke age | AD 1750 to AD 1910 | industrial users | rails, pumps, soot, forced reuse |
| Wire / digital edge | AD 1910 to AD 2000 | recognizable modern humanity | sensors, labs, cables, shock of “that was us” |
| Break / Veil / Retreat | AD 2000 to AD 7600 | future humanity | climate disruption, Veil, retreat upward |
| Hollowers | AD 7600 to AD 8600 | recent ancestors | reuse, occupation, failure, sealing |
| Sealing / Forgetting | AD 8600 to AD 9026 | summit ancestors | taboo, myth, partial memory |
| Summit Present | AD 9026 | current player people | rediscovery and repair |

### Guardrail

The underground is not a textbook museum. Each layer is an authored Understory interpretation of human capability:

- breath/air/filtering, now described through Works/Clearers/Mist Engines
- seed
- stone
- furnace
- water
- law
- archive
- glass
- mechanism
- wire

---

## 15. Surface bands

Surface identity is **future collapse in reverse, experienced as livable recovery**.

| Band | Date | Visual identity | Player read |
|---|---:|---|---|
| Summit Present | AD 9026 | handmade high-air settlement, windbreaks, repair scarcity | we live here now |
| Sealing / Forgetting | AD 8600-9026 | blocked entrances, taboo marks, old warning masonry | why the Bore became myth |
| Hollower remnants | AD 7600-8600 | old lift camps, patched vents, sealed routes | our ancestors tried |
| High Retreat | AD 6000-7600 | cliff towns, water catchers, emergency terraces | humanity moved upward |
| Veil Age | AD 4000-6000 | haze shelters, descent stations, storm walls | why descent became fear |
| Drawdown / Break Echoes | AD 2000-4000 | old climate infrastructure, labs, roads, machines | civilization tried to respond |
| Base / Low Before | AD 2000 and earlier echoes | forested ruins, ports, transit, old city bones | the old world is visible again |

The immediate player read should always be:

> **The Veil cleared. We can live here now.**

The deeper Archive read can explain what era the land belongs to.

---

## 16. Underground layers, visual and mechanical direction

### Layer 0: Before-Us Threshold, c. 5,000,000 BC, late-game only

Emotional purpose: the player learns that “human-like” culture has happened before.

Use:

- near-human but wrong-scale architecture
- domestic spaces before alien spectacle
- tools that feel like stone, bone, and interface
- impossible joints
- nonmodern hearths, rest niches, child/care spaces

Avoid early reveal.

### Layer 1: Ancient human origin, c. 8000 BC to 6500 BC

This is the primary ancient machine-ritual layer.

Use:

- stone control slabs
- old conduits / The Lines
- Clearer stations
- Mist Engine fragments
- Black Vaults
- pressure gates
- mineralized machine faces
- star shafts
- water/air/light balance as practical ritual

Do not use “Old Lungs” language.

### Layer 2: Seed and Calendar Halls, c. 6500 BC to 4500 BC

Use:

- seed vaults
- soil chambers
- water clocks
- underground garden remains
- crop calendars
- flood basins
- nursery/storage spaces

Early gameplay: visual life return and material identity, not a full farming sim.

### Layer 3: Monument Civic Age, c. 4500 BC to 2500 BC

Use:

- processional ramps
- public stone records
- star-aligned shafts
- memorial chambers
- ancestor walls
- giant ghost maps
- dressed stone and public works

Avoid implying real ancient cultures were secretly built by hidden supertech.

### Layer 4: Furnace and Alloy Age, c. 2500 BC to 800 BC

Use:

- copper stains
- bronze fittings
- slag glass
- bellows chambers
- casting molds
- metal-lined ducts
- tool shrines

Mechanical value: forge, supports, fittings, hoist upgrades.

### Layer 5: Road, Water, and Law Age, c. 800 BC to AD 500

Use:

- aqueduct blocks
- cisterns
- road galleries
- civic seals
- mile markers
- standardized blocks
- assembly spaces

Mechanical value: movement, channels, water routing later.

### Layer 6: Quiet Orders, AD 500 to AD 1400

Use:

- dry libraries
- lamp niches
- scriptoria
- map rooms
- lens fragments
- songs encoded as instructions

Mechanical value: Archive expansion and story fragments.

### Layer 7: Glass and Mechanism Age, AD 1400 to AD 1750

Use:

- lenses
- mirrors
- brass mechanisms
- pressure gauges
- calibration wheels
- Far-Glass roots

Mechanical value: telescope/Far-Glass restoration and precision systems.

### Layer 8: Steam and Smoke Age, AD 1750 to AD 1910

Use:

- rails
- pump houses
- boilers
- soot ducts
- piston lifts
- iron retrofits forced through older stone

Mechanical value: pumps, rails, heavy movement.

### Layer 9: Wire, Atomic, and Digital Edge, AD 1910 to AD 2000

Use sparingly for shock:

- cables
- sensors
- warning labels
- broken monitors
- lab doors
- field recorders
- hard hats, mugs, photos, badges

If this layer dominates visually, the ancient mood dies.

### Overlays: Hollowers and current crew

Hollower traces:

- bedrolls
- warning paint
- sealed doors
- family marks
- patched pipes
- emergency braces
- cooking corners

Current crew traces:

- lantern strings
- chalk marks
- fresh braces
- haul tags
- crates
- notes
- new ghost drafts

---

## 17. First playable / Phase 0

Phase 0 must prove both:

1. the pure core loop
2. the first emotional restoration

### Phase 0 must include

- high summit workyard above the Veil
- visible damaged repair cluster
- workers gathering scarce local material
- hidden hatch discovery
- Bore Room reveal
- one Steward avatar
- one or two worker figures/silhouettes
- one extraction volume or material cache
- trace/blast extraction proof
- first haul table / Return Ritual v1
- first refiner / kiln
- first surface repair cluster
- small free-build/drafting area
- Archive shelf seed
- Core Sample placeholder or first band if feasible
- no store

### Phase 0 should not include

- full Veil clearing
- Far Peak communication
- full water/flood system
- full Archive hall
- full player-facing character creator
- monetization UI
- full timeline exposition
- full procedural mountain
- combat
- multiplayer

### First surface repair cluster

Q22 lock:

> **shelter/windbreak + terrace wall + tiny garden**

This is one tight cluster, not three missions.

| Element | Purpose | Materials |
|---|---|---|
| shaft-head shelter / windbreak | survival and practical repair | rough stone, timber brace, fired brick |
| broken terrace edge | safe buildable land | rough stone, dressed stone, timber brace |
| tiny dead soil bed | life returning | seedclay/soil material, water, warmth |

### First underground discovery

Q23 lock:

> **Local workers gathering scarce material uncover a hidden hatch. The hatch reveals the Bore Room.**

This replaces old “sealed Bore opens” or “first seam chamber” starts.

#### Discovery beats

1. Summit is visibly damaged and material-poor.
2. Workers gather clay/stone/mineral from a shallow cut.
3. They hit worked stone / a plate / hidden handle.
4. Steward is called.
5. Player inspects/clears the hatch.
6. Hatch opens.
7. Camera drops into Bore Room.
8. Bore Room reveals the constructed shaft access.
9. First haul supports surface repair.

---

## 18. Bore Room design

The Bore Room is the first true underground space.

It should contain:

- vertical shaft dropping beyond visible range
- old guide rings / lift rails
- sealed platform or rim
- mineral crust and dust
- practical ancient construction
- one inactive Mist Engine / Clearer / Lines hint
- one Hollower warning mark painted over older structure
- one extraction volume or material cache
- protected shaft rim
- editable debris / material faces

It should feel:

- old
- built
- practical
- heavy
- hidden
- deep
- not magical
- not sci-fi clean
- not a cartoon mine

Good copy examples:

- “The workers found a hatch under the ridge cut.”
- “The chamber below is worked stone. Old. Deep.”
- “The shaft continues past the lantern.”
- “There are Lines in the wall, but none are active.”
- “A Clearer mark. Maybe. The old word is uncertain.”
- “The Black Vault is sealed.”
- “This place was not dug. It was built.”

Bad copy examples:

- “The mountain breathes again.”
- “The Old Lung awakens.”
- “Breathstone pulses with life.”
- “The Heart of the mountain calls.”

---

## 19. Mechanics

### 19.1 Broad editable zones

The game should support large buildable spaces inside and outside the mountain. The player should feel like they can stand in structures, build in them, remove most ruined elements, and reshape livable space.

Editable/buildable areas include:

- surface terraces
- shaft-head workyard
- ruined surface structures
- Bore Room debris/material areas
- interior chamber floors
- wall-town ledges
- extraction faces
- shoring zones
- ghost-map rebuild zones

### 19.2 Protected shells

Protected geometry is needed to preserve world shape, camera framing, performance, and narrative landmarks.

Protected:

- mountain exterior silhouette
- main cliff boundaries
- primary Bore shaft shell
- major chamber containment walls unless explicitly knockdown
- story landmarks

### 19.3 Free drafting

Free drafting remains locked.

Rules:

- ghost blueprints can be placed without material
- drafts persist
- drafts generate material want-lists
- material shortages should create desire, not dead sessions
- committed blocks spend real material

### 19.4 Repair anchors

Surface build model is repair anchors plus free build.

Every revealed terrace should have:

1. a wound
2. a material need
3. visible restoration payoff
4. free-build space after repair

### 19.5 Extraction

Extraction must be a real verb.

Need:

- trace seam
- blast for bulk
- shoring/support
- instability risk
- collapse/burial
- re-excavation
- yield differences
- haptics on iPhone where possible
- visual/audio fallback on iPad

### 19.6 Support and collapse

Locked rule:

> **Collapse buries. It never destroys.**

Collapse should feel like a detour, not a punishment or monetization event.

### 19.7 Water

Water is not Phase 0 lead.

Later:

- authored seep hard stop
- pump / channel / drain systems
- flooding conceals, draining recovers
- water weakens supports

No full fluid sim until core proves itself.

### 19.8 Light

Light is expressive first.

- darkness is not hostile
- lantern strings make spaces human
- emissive materials become desirable
- no survival darkness in early game

---

## 20. Materials

Q8 lock:

> **Every material has a functional family and era identity.**

### Functional families

- Terrain / clay / soil / fill
- Timber / fiber / support
- Masonry / stone / brick
- Mineral / rare / optical
- Metal / fittings / rails / pumps
- Glass / lens / light
- Machine materials / filtering / conduits / Black Vaults

### Early material cap

Phase 0 should use about 5-7 materials maximum.

Recommended starter set:

| Material | Family | Era/source | Main use |
|---|---|---|---|
| Rough Stone | masonry | local cut / first Bore cache | terrace wall, shelter repair |
| Clay / Seedclay | terrain/growth | first material pocket | soil bed, kiln, early blocks |
| Timber Brace | timber/support | summit salvage/current crew | shoring, windbreak, shelter |
| Fired Brick | refined masonry | kiln output | first clean building blocks |
| Filterstone | machine/common | early Works hint | later Clearer/Line repair, maybe not Phase 0 full use |
| Blackglass | glass/rare machine | inactive Bore Room hint | Archive/Far-Glass/machine mystery |
| Patch Ceramic | surface handmade | Summit culture | starting shelter aesthetic |

### Deprecated material names

- Breathstone → use Filterstone / Carbonate Brick / Intake Ceramic / Blackglass depending on function.
- Lungstone → do not use unless deliberately as obsolete folk myth.

### Material purpose

The latest lock:

> **Materials from below primarily repair/reclaim above-ground lands and structures.**

Free building remains important, but repair is the emotional reason the material matters.

---

## 21. Tapper math and cadence

Q9 lock:

> **Hybrid authored cadence.**

First session should be authored and fast. Do not make the player wait before they understand the full loop.

### Job bands

| Job band | Use |
|---|---|
| 30 sec to 2 min | tutorial, first hard stop, tiny repair |
| 5 to 10 min | short check-in jobs |
| 20 to 30 min | normal pocket return loop |
| 2 to 4 hours | workday jobs |
| 8 to 12 hours | overnight / long absence cap |

### Economy assumptions

These are scaffolds, not final balance.

- first authored haul: enough for 12-24 committed blocks
- first draft: 40-60 blocks total
- early daily target: 60-120 placed block equivalents
- 10-minute player: can make a room, not a castle
- castles/major structures should take weeks and matter
- free drafting prevents material-poor sessions from feeling dead

### Instrumentation needed later

- time to first block
- time to first draft
- time to first material shortage
- percentage returning after hard stop
- number of draft blocks placed while material-poor
- Return Ritual skip rate
- D1/D7/D30 retention
- subscription conversion after attachment moment

---

## 22. Return Ritual, Core Sample, Archive

### Return Ritual

Returning should be physical theater, not a claim screen.

Elements:

- hoist / carts / crates / haul table
- physical display of materials
- short crew note
- skippable after familiarity
- no monetization prompts here

### Core Sample

The Core Sample is the vertical progress object.

Role:

- physical stratum record
- depth/history index
- screenshot object
- truth column that eventually contradicts summit myth

Phase 0 can use placeholder/first band only.

### Archive

The Archive is where Singulars and story fragments live.

Phase 0 seed:

- one shelf
- one object or empty slot
- no encyclopedia wall of text

Longer term:

- conflicting evidence: summit myth vs. Hollower warning vs. ancient mark vs. physical object
- empty slots create retention without gacha

---

## 23. Crew

### Crew role

Crew are not full Sims in Phase 0.

They provide:

- return notes
- hard-stop warmth
- job timers
- world presence
- named attachment later

### Named crew monetization lock

- core crew earned through play
- optional patron crew sold directly
- no gacha
- no randomization
- no duplicates
- no required paid roles
- paid crew are personality/convenience first
- tiny capped bonuses allowed, roughly 3-5%, non-stackable, not tied to rare finds or land

### Character builder rollout

- Phase 0: authored crew generated internally
- Later: crew roster customization
- Later after that: light player customization
- Full dress-up not in launch scope

---

## 24. Character art direction

Q16 lock:

> **Core characters are High-Air Heirloom Miniatures.**

They are:

- small stylized humans
- high-altitude adapted
- wrapped and masked
- practical
- made of real-material surfaces: cloth, ceramic, leather, glass, stone, worn metal
- readable at mobile scale
- human through posture, gear, notes, and props

They are not:

- toy mascots
- plastic astronauts
- giant-eyed chibi figures
- bouncy villagers
- silly costume piles

### Visual reference interpretation

Use the provided sample images carefully.

| File | Use |
|---|---|
| `Generated image 1 (1).jpeg` or duplicate | Best global reference. Left panel = caution. Middle = runtime target. Right = capture/marketing target. |
| `Material people concepts.jpeg` | Best source for material/job archetype ideas. Adapt, do not copy literally. |
| `Builder helmet concepts.jpeg` | Useful for helmet, visor, tool, pack readability. De-plasticize. |
| `Supernauts adjacent concepts.jpeg` | Caution reference: good readability, wrong soul. Too bright/plastic/sci-fi. |
| `Job rescue concepts.jpeg` | Mostly caution. Tiny backpack idea useful, plush/puppet/knight wrong for core. |
| `understory-character-vision-board-v1.png/pdf` | Use as early art summary but update with latest canon. |
| `understory-crew-archetype-targets-v1.png/pdf` | Useful for Shorer / Mechanic / Seed-Keeper direction. |

### Crew archetypes for first art pass

1. **First Shorer**
   - steady, dusty, practical
   - timber brace, hammer, goggles, scarf/filter, lantern

2. **Works Mechanic / Clearer Mechanic**
   - replaces Breathline Mechanic language
   - pressure pack, valve key/wrench, soot, Blackglass/Filterstone accents

3. **Seed-Keeper**
   - calm, hopeful, practical
   - seed pouches, water flask, soil tray, worn green/earth palette

---

## 25. Monetization and business

Q10-Q15 current locks supersede v5/v6 monetization details.

### Monetization spine

- subscription + expansions + cosmetics
- founder/patron first
- subscription second
- expansions third
- named crew direct-purchase later if crew warmth proves out
- no ads at launch
- no paid skips at launch

### Anti-alienation covenant

Never:

- coins
- premium currency
- loot boxes
- gacha
- randomized paid rewards
- purchasable rare materials
- purchasable land
- paid hard-stop rescue
- paid collapse/flood rescue
- paid rare-find boost
- purchase prompt at a missing-material moment
- purchase prompt at a Veil block
- purchase prompt during Return Ritual

Money may buy:

- more authored content
- more cosmetics
- extra world slots
- smoother expedition management
- extra refine queue through subscription
- bigger storage through subscription
- Archive convenience
- batch claim / scheduling comfort
- named crew personality/convenience

### Paid skips

Launch: no.

Future: refine-queue-only skips may be revisited, but only after the economy proves fair. Never skips for dig, rare, land, Veil, collapse, flood, hard stop, or story gate.

### Ads

Launch: no ads.

Future: only opt-in rewarded ads if needed, non-subscribers only, never interruptive, never tied to rare materials/land/hard stops.

---

## 26. IP strategy

Q1 locked standalone IP.

Use original IP first.

Do not license a major external IP for v1.

Public-domain-inspired events can be considered later after:

- retention proof
- legal review
- trademark review
- territory review
- translation / derivative / artwork checks

Possible inspiration directions only:

- Journey to the Center of the Earth
- At the Earth’s Core
- The Time Machine
- Lost world / hidden world literature

Do not let a license define the first product.

---

## 27. Production platform

### Current lock

> **Unity 6 + URP for iOS production proof.**

Target:

- iPhone = business platform
- iPad = showcase/building platform
- one shared world state later

### Do not build from scratch

Do not keep trying to build a full production voxel world from the old custom web engine.

Carry over from Hemstitch as design/tech knowledge:

- rounded block feel
- character builder concepts
- material palette lessons
- validation ideas
- ghost preview behavior
- touch-first lessons

But the new production proof should be Unity.

### Stack suggestions

- Unity 6
- URP
- iOS Build Support
- Xcode for device builds
- TestFlight later
- RevenueCat or equivalent later for subscriptions/IAP management
- Blender for custom assets
- bought asset packs only for blockout/reference, not final identity

### Runtime/capture quality targets

- Runtime target: middle panel of the sample image, “real materials.”
- Capture/marketing target: right panel, cinematic lighting.
- Avoid left panel plastic toy look.

---

## 28. What to buy vs. custom-build

### Buy/use for blockout and speed

- Unity Asset Store / Fab / Synty / Quaternius style packs for placeholder scale only
- ProBuilder or equivalent for quick blockout
- material/shader tools if they accelerate PBR tests
- simple environment props as temporary assets

### Custom-make early

- Understory block kit
- hatch/Bore Room pieces
- summit repair cluster
- material palette
- High-Air Heirloom characters
- extraction pockets
- repair anchors
- key UI/ritual objects

### Do not buy or build yet

- Minecraft clone kit
- full voxel survival template
- procedural terrain mega-system
- multiplayer framework
- combat/adventure template
- full city-builder template
- realistic human character system
- huge realistic environment pack that fights the art direction

---

## 29. New Unity repo first deliverable

Codex should not build the whole game.

First deliverable:

> **Scene 01: Summit Hatch + Bore Room Proof**

### Required scene components

#### Surface

- high mountain summit workyard
- visible Veil below
- first repair cluster: shelter/windbreak, broken terrace wall, tiny dead garden
- shallow material-gathering cut where workers discover hatch
- visible Summit Steward
- one or two worker figures/silhouettes
- stone/clay/timber/crate/tool props

#### Hidden hatch

States:

1. covered by debris/mineral crust
2. partially exposed
3. opened
4. transition into Bore Room

#### Bore Room

- constructed ancient access chamber
- vertical shaft beyond view
- guide rings/lift rails
- inactive machine hint using new language
- old Lines / Clearer / Mist Engine fragment
- possible Black Vault hint
- one Hollower warning mark overlay
- one extraction volume/material cache
- protected shaft rim
- editable debris/material face

#### First repair result

After haul/recovery:

- shelter improves
- terrace stabilizes
- tiny soil bed becomes viable
- player understands material from below repairs life above

---

## 30. Six-week gauntlet

### Weeks 1-2: Visual proof

Goal:

> **Does this look like a premium tactile miniature world, not a toy voxel demo?**

Deliver:

- Unity URP project
- Summit Hatch scene
- Bore Room scene
- Steward placeholder
- material shader/material tests
- runtime lighting close to middle reference
- capture lighting close to right reference
- surface repair cluster blockout
- protected shell vs editable ruin classification pass

### Weeks 3-4: Interaction / editability proof

Goal:

> **Can the player physically inhabit and alter this world?**

Deliver:

- soft tap-to-move Steward between nodes
- hatch reveal
- editable surface repair cluster
- editable ruin pieces
- broad build zone
- interior editable zone
- block placement/removal
- protected shell rules
- extraction volume with trace/blast
- first haptic seam test
- shoring/support first pass

### Weeks 5-6: Loop proof

Goal:

> **Does descend, extract, ascend, refine, repair, wait, return work?**

Deliver:

- haul table
- Return Ritual v1
- inventory counts
- first refiner/kiln
- repair cluster consumes material
- ghost draft / want-list stub
- crew job duration selector
- short offline timer simulation
- first hard stop note
- repeatable return loop

---

## 31. Acceptance criteria for the Unity proof

### Visual

- runtime looks like tactile real-material miniature, not plastic toy
- capture mode can make cinematic beauty shots using same assets
- Steward looks human enough, not mascot
- Bore Room feels old, built, deep, practical

### Spatial

- player feels large spaces
- Steward can stand inside structures
- surface has broad buildable land
- interior has meaningful editable/buildable areas
- protected shells prevent world-breaking

### Editability

- most ruins can be removed/rebuilt
- player-built blocks are editable
- extraction face can be traced/blasted
- shoring matters at least in first-pass form
- collapse buries rather than destroys if included

### Loop

- material from below repairs surface above
- first repair cluster visibly improves
- Return Ritual makes haul physical
- first wait/return loop has a reason
- no store/monetization appears

---

## 32. Do not build yet

Do not build these in the new repo proof:

- full monetization
- full subscription
- full character creator
- full named crew store
- full water/flood system
- full Archive hall
- full Core Sample system
- full age ladder content
- Far Peak communication
- full procedural mountain
- multiplayer
- combat
- hunger/survival
- Android
- full city sim
- huge quest system
- full AI crew sim

Seed only if easy:

- Archive shelf placeholder
- Core Sample placeholder
- inactive Works/Mist Engine hint
- Veil atmosphere
- Far-Glass prop without reveal

---

## 33. Open decisions that still matter

### Highest priority open decisions

| Priority | Decision | Why it matters | Current lean |
|---:|---|---|---|
| 1 | What is the field tool / zapper called? | UI, fiction, player identity | Line-Key / Trace-Key / Survey Key open |
| 2 | Why is the Steward uniquely able to use it? | player fantasy and lore | inherited craft-language + Hollower tradition |
| 3 | Exact player-culture name | copy, crew notes, world identity | Summit Kin / Highfolk / Crownward open |
| 4 | Ancient layer name | art/story docs | replace Breathwrights? First Stewards / Rootwardens / Clearwrights? open |
| 5 | Recent ancestor name | Archive and warning marks | Hollowers current lean |
| 6 | Machine material starter names | economy and UI | Filterstone, Blackglass, Carbonate Brick candidates |
| 7 | Exact first material | first haul clarity | rough stone + clay + timber current lean |
| 8 | How much of Phase 0 is truly editable? | scope and tech | broad zones but limited scene |
| 9 | Tap-to-move feel | product shape | locked but spike-tested |
| 10 | Character scale vs block scale | standing inside structures must feel real | needs Unity visual proof |

### Story open decisions

- final title / subtitle
- public-facing tagline
- player culture name
- ancient human culture names
- exact date range of ancient origin layer
- whether Before-Us date is exactly 5,000,000 BC or softer deep-time
- whether Far Peak exists in v1 or only later
- whether the Base reveal is v1, Season 1 finale, or long game
- exact Long Cooling / Drawdown science language
- whether the Line-Key/tool is ancient, patched, or current-built around ancient parts

### Mechanics open decisions

- final editability rules and UI feedback
- exact block grid size and scale
- whether removal is instant or staged
- exact trace seam gesture tolerance
- blast vs trace yield math
- collapse thresholds
- shoring material cost
- draft commit gestures on iPad/iPhone
- inventory caps
- refiner queue sizes
- first timer ladder exact values
- how broad the first build zone is
- whether the Bore Room itself can be heavily rebuilt early

### Art open decisions

- Summit palette: warm handmade vs more muted survival ceramic
- Veil look: fog ocean vs toxic storm wall vs layered haze
- Mist Engine / Clearer visual shape
- Lines visual language: pipe, duct, cable, groove, conduit, channel
- Black Vault visual language
- High-Air character exact proportions
- masks vs visible faces in Phase 0
- how worn/detailed runtime materials can be on iPhone

### Business open decisions

- subscription name
- monthly price
- annual price
- founder pack price/contents
- named crew price range
- season cadence
- whether ads ever return after launch
- whether refine-only paid skips ever return after launch
- whether RevenueCat is adopted in first production pass or later

---

## 34. Contradiction override list

If a previous doc says X, use Y instead.

| Old / contradictory | Use now |
|---|---|
| HEM canon open/shared universe | Understory standalone IP using HEM/Hemstitch DNA |
| Bore is visible/sealed from minute one | Workers uncover hidden hatch while gathering scarce material |
| First chamber = generic seam chamber | First underground = Bore Room reveal |
| Old Lungs | The Works / Clearers / Mist Engines, depending context |
| Breathlines | The Lines / conduits / old ducts |
| Breathstone / Lungstone | Filterstone / Carbonate Brick / Blackglass / Intake Ceramic |
| mountain breathes | The Works clear the Veil / machinery stabilizes the air / a Clearer comes online |
| rewarded ads at soft launch | no ads at launch, revisit later |
| refine skips promoted | no paid skips at launch, refine-only may be revisited later |
| mostly tiny simulated pockets only | broad editable zones with protected shells and extraction/destruction volumes |
| toy character direction | High-Air Heirloom Miniatures |
| player unseen steward | visible Summit Steward avatar |
| no locomotion assumed | soft tap-to-move between important nodes |

---

## 35. Codex new repo directive: paste this first

Use this as the first instruction in the new Codex repo thread.

> We are starting a new Unity 6 + URP iOS-first production proof for Understory. Do not port the old custom voxel engine. Use it only as reference for rounded tactile block feel and character-builder DNA. The first deliverable is **Scene 01: Summit Hatch + Bore Room Proof**. Build a vast-authored but tightly scoped mountain slice with broad editable/buildable zones, protected structural shells, and simulated extraction/destruction volumes. The player is a visible Summit Steward using soft tap-to-move between nodes. The first flow is: damaged summit repair cluster → workers gathering scarce material uncover hidden hatch → Steward opens hatch → Bore Room reveal → extract first material → return haul → refine → repair shelter/windbreak, terrace wall, and tiny garden. Runtime visual target is premium real-material miniature, not plastic toy. Capture target is cinematic. Do not build monetization, full character creator, water sim, multiplayer, combat, or full procedural terrain. Do implement early trace/blast extraction, block placement/removal, protected vs editable classes, and first Return Ritual stub.

---

## 36. Minimum Unity setup checklist for Codex

Codex should verify:

- Unity 6 installed.
- URP project created.
- iOS Build Support installed.
- Xcode installed and opened once.
- New repo contains `/Docs`, `/References`, `/UnityProject` or equivalent structure.
- Reference images copied into `/References/Visual`.
- Source scope docs copied into `/Docs/Archive`.
- This handoff manual copied into `/Docs/START_HERE_UNDERSTORY_HANDOFF_v7.md`.
- A clean empty scene can run in Unity editor.
- No production systems start before the visual proof scene exists.

---

## 37. Suggested repo folders

This is organizational guidance, not a hard technical requirement.

```text
/Docs
  START_HERE_UNDERSTORY_HANDOFF_v7.md
  /Archive
    understory-scope-v5.md
    understory-master-scope-v6.md
    understory-codex-mockup-scope-v6.md
    understory-decision-register-v6.pdf
    understory-character-builder-codex-scope-v1.md
/References
  /Visual
  /Characters
  /Materials
  /UI
/UnityProject
/DesignExports
/BuildNotes
```

---

## 38. Final success statement

The first Unity proof succeeds if someone can watch the first slice and say:

> **The summit needed repair. Workers found a hatch. The Steward opened the Bore. I extracted real material from below, brought it back up, and made the surface more livable. Now I understand why I want to go deeper.**

That is Understory.

Not a voxel engine.
Not a toy builder.
Not a console epic.
Not a lore pile.

A real-material vertical restoration builder where every block has a past and every repair makes the future more possible.
