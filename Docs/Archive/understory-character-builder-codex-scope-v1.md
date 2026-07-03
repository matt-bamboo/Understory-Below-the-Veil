# UNDERSTORY Character Builder Scope Prompt v1
## Goal: adapt the existing Hem character builder into an Understory crew visual lab

Use this as a Codex handoff prompt. This is a scope and art-direction brief, not a request to rewrite the engine.

---

## 0. Product context

Understory is now the master direction: an iOS-first tapper-builder with an epic fossil record. The player lives with the Summit Kin in AD 9026, above the Veil, and reopens a sealed Bore at the crown of a high mountain. Below, crews descend into the Hollow and build upward through ancient human history. Above, the settlement carves downward as Old Lungs and Breathlines clear the Veil.

Characters need to support this tone:

> High-Air Heirloom Miniatures: small readable humans in crafted survival gear, built from cloth, ceramic, leather, glass, stone, soot, and lantern light. Not plastic toy mascots.

The current character builder already has useful foundations. Preserve them.

---

## 1. Repo review notes

Existing useful architecture:

- The character designer lives in `artifacts/rounded/src/designer/` and is already isolated from the core mesher, world renderer, camera controls, material registry, and builder.
- Characters are saved as compact descriptor DNA, not baked meshes.
- `buildCharacterParts(descriptor)` is the key reusable boundary. It returns serializable rounded-part data, anchors, and pose metadata. React/Three rendering happens above that in `CharacterModel.jsx`.
- Public exports already expose `buildCharacterParts`, descriptor helpers, and slot definitions/options.
- The designer already supports roster persistence, save, export to a JSON + PNG bundle, compatibility notices, randomize, shuffle colors, and responsive preview framing.
- Animation is transform-only: idle breathing, blinks, look-around, accessory motion, part-specific reactions, reduced-motion support. There are no bones, IK, walk cycles, RPG stats, or AI.

This is a good base. The Understory work should be a focused visual/descriptor pass and preset system, not a character-tech rewrite.

---

## 2. Creative target

### One-line target

Make the existing toy-readable builder produce believable AD 9026 crew: miniature humans adapted to thin air, scarcity, the Veil, and the sacred-practical work of reopening the mountain.

### Must feel like

- Hardy, quiet, practical, human.
- Premium tactile miniature, not plastic toy.
- High altitude, low resources, repaired gear, inherited tools.
- Crew who work carefully around ancient machines and dangerous strata.
- Warm and materially hopeful, not grim.

### Must not feel like

- Supernauts clone.
- Cute astronaut toy line.
- Chibi RPG class roster.
- Mascot villagers.
- Plush/fantasy workers.
- Console cinematic character system.

---

## 3. Use the existing sample images, but label them clearly

Yes, include sample images with the Codex request. Use only a small set and label intent so Codex does not overfit to the wrong parts.

Attach these images:

1. `understory-character-vision-board.png`
   - Use this as the concise visual summary.

2. `Generated image 1 (1).jpeg`
   - Use the middle/right panels as the material and lighting target.
   - Do not use the left panel as a target; it is too plastic.

3. `Material people concepts.jpeg`
   - Use as material/job archetype DNA.
   - Best seeds: brick/mason, glass/veil worker, moss/seed-keeper, wood/timberwright, metal/mechanic, textile/patchworker.
   - Do not make them literal material people.

4. `Builder helmet concepts.jpeg`
   - Use for visor, helmet, backpack, and tool readability.
   - De-saturate and convert from astronaut plastic into wrapped high-altitude survival gear.

5. `Generated image 2 (1).jpeg`
   - Use only the Tiny Giant-Backpack idea as a survival silhouette seed.
   - Treat the rest as negative reference: too plush, cute, fantasy, or arcade.

Optional negative references:

- `Supernauts adjacent concepts.jpeg`
- `Generated image 3 (1).jpeg`

Use these only to say: the readability is good, but the soul is wrong for Understory.

---

## 4. Scope objective

Create an **Understory Crew Lab** built on top of the existing Hem character designer.

This can be one of two forms depending on what is fastest and safest:

1. A mode/toggle inside the existing character designer, or
2. A separate entry next to the existing designer using the same builder and descriptor pipeline.

The output should prove whether the existing character system can support Understory’s people by changing proportion tuning, slot curation, material finish, palettes, gear, presets, and preview lighting.

---

## 5. Deliverables

### 5.1 Understory visual mode

Add a clear Understory mode to the character preview/designer.

The mode should:

- Keep descriptor compatibility.
- Reuse `buildCharacterParts(descriptor)` or extend it safely.
- Keep current Hem designer intact.
- Allow a curated Understory preset set.
- Allow export of descriptor + PNG.
- Show crew on a neutral/cinematic miniature stage.

### 5.2 Three required crew presets

Build these first. They are the benchmark.

#### A. Shorer / Mason

Emotional read: makes the mountain safe.

Visual traits:

- Wrapped hood or low helmet.
- Smoked goggles or narrow visor.
- Ceramic breath filter or cloth mask.
- Masonry pads, timber pegs, chalk tags.
- Timber brace or hammer.
- Heavy boots.
- Muted ochre, clay, slate, dusty blue.
- Focused or neutral expression. No big smile.

#### B. Breathline Mechanic

Emotional read: makes the mountain breathe.

Visual traits:

- Compact helmet or half visor.
- Oxidized metal fittings.
- Pressure gauge, wrench, tuning fork, or small pipe tool.
- Air canister or filter pack.
- Subtle blue-green indicator light.
- Soot marks, repaired cloth, leather straps.
- Calm, concentrated posture.

#### C. Seed-Keeper

Emotional read: makes survival become home.

Visual traits:

- Moss-lined hood or cloth cap.
- Seed pouches, small pot, water flask, plant tray.
- Patched gloves, soft cloth layers.
- Warm earth palette with restrained green.
- Reverent, quiet, not cutesy.

### 5.3 Optional fourth and fifth presets

If the first three are fast:

#### D. Far-Glass Surveyor

- Lens pack, small scope, weather hood, map tube.
- Connects to telescope/Far Peak fiction.

#### E. Patchworker / Clothwright

- Pressure cloth, sewing kit, scarf wraps, mask repair tools.
- Connects to summit culture and human trace props.

---

## 6. Art-direction changes to test

### 6.1 Faces

Current face options are broad and toy-friendly. Understory needs restraint.

For Understory mode:

- Favor neutral, focused, determined, worried, content.
- Avoid big-grin, laughing, crying-laughing, starry-eyed, blushing, wink for core crew presets.
- Consider adding face-cover options: cloth mask, ceramic filter, narrow visor, smoked visor, Veil goggles.
- Faces can be partially hidden. That helps the people feel less mascot-like and supports the Veil/altitude fiction.

### 6.2 Headwear and gear

Current options include useful seeds: hard hat, bandana, goggles, ski goggles, space helmet, astronaut helmet, pilot cap, oxygen tank, backpack, work gloves, work boots, hammer, wrench, watering can, shovel, pickaxe, lantern.

Understory needs new or adapted variants:

- Veil hood.
- Breath mask.
- Smoked square visor.
- Ceramic filter helmet.
- Dust scarf.
- Shoring helmet.
- Survey lens hood.
- Patched pressure hood.
- Pipe pack / Breathline pack.
- Sample tube pack.
- Rope bundle / climb pack.

Use the existing helmet silhouettes where possible, but remove the astronaut read.

### 6.3 Materials and finish

Current material rendering is readable but too toy-clean for Understory.

Test an Understory material pass with:

- Cloth/fabric roughness.
- Ceramic and fired-clay surfaces.
- Leather straps.
- Oxidized metal.
- Smoked glass / translucent visor.
- Moss/felt accents.
- Stone/masonry pads.
- Soot/dust overlays.

Implementation note for Codex: if the existing part schema only carries color/opacity/radius, add a small optional material/style field rather than replacing the schema. Old descriptors must still render.

### 6.4 Palette

Use an Understory palette story:

- Clay red / fired brick.
- Ochre / old warning paint.
- Slate grey / stone.
- Dust blue / high-air cloth.
- Moss green, restrained.
- Lantern amber.
- Smoked glass blue-grey.
- Oxidized copper/green.
- Worn leather brown.
- Bone ceramic / warm off-white.

Avoid saturated primary toy colors except for tiny functional accents.

### 6.5 Animation tone

Current transform-only animation is good. Retune Understory mood.

Understory mode should:

- Slow down idle movement.
- Reduce bounce and hop amplitude.
- Keep breathing, blinking, small look-around.
- Prefer weighted micro-motions over cheerful reactions.
- Slot-change pop can stay in the designer, but core crew should not feel bouncy in-world.

Possible mode values:

- `understoryIdle`: slower breath, less sway, more head stillness.
- `understorySave`: small nod or hand-to-chest, not a big hop/wave.
- `understoryFocus`: steady posture for Shorer/Mechanic.

---

## 7. Designer UX changes

### 7.1 Add an Understory preset strip

Add a quick preset strip or tab:

- Shorer
- Breathline Mechanic
- Seed-Keeper
- Far-Glass Surveyor
- Patchworker

Each preset should be editable after selection.

### 7.2 Add curated randomize

Add an Understory randomize mode that only draws from Understory-safe options.

Rules:

- No wizard, crown, animal-ear hood, party, top hat, hero cape, royal mantle, fairy/angel/butterfly wings, balloon, ice cream, toy sword, magic wand, sunglasses/star shades as core options.
- Allow tools: hammer, wrench, watering can, shovel, pickaxe, lantern, book, torch.
- Allow back: backpack, oxygen tank, possibly new Breathline pack/sample pack.
- Allow headwear: hard hat, beanie, bucket, bandana, pilot-cap adapted, helmets adapted.
- Favor focused/neutral/determined expressions.

### 7.3 Stage / render modes

Add three preview environments if not too heavy:

1. Studio Material
   - Neutral background, shows surfaces clearly.

2. Summit Lamplight
   - Warm lantern and fog/haze feel.

3. Bore Worklight
   - Darker, directional, shows visor/lamp/glass.

Do not build a world scene yet. Simple lighting/background changes are enough.

---

## 8. Character-builder acceptance criteria

### Visual pass

The first three presets must pass this test:

> Would I believe these tiny people live above the Veil and carefully reopen an ancient machine mountain?

And must fail this test:

> Do these look like generic toy astronauts or mascot workers?

If yes to the second question, the pass is wrong.

### Technical pass

- Existing Hem character designer still opens and works.
- Existing saved descriptors still normalize and render.
- Existing export still works.
- Understory presets can be saved/exported.
- No full rig, IK, walk cycle, AI, or pathing added.
- No rewrite of R1 mesher, world renderer, camera controls, material registry, or core builder.
- Performance remains acceptable in the existing preview.

---

## 9. Non-goals

Do not build:

- A full crew simulation.
- Full in-world NPC pathing.
- New gameplay jobs.
- Monetization UI.
- Store flows.
- Dialogue system.
- Portrait generator.
- Unreal/console-grade character tech.
- New engine stack.
- A realistic human art direction.

This is a visual and descriptor-system proof, not a people-system implementation.

---

## 10. Recommended first task order

1. Add Understory palette constants / color story.
2. Add or adapt a small set of Understory-safe slot options.
3. Add Understory curated randomize/preset descriptors.
4. Add the three required presets.
5. Add an Understory preview lighting mode.
6. Add any minimal material/style metadata needed to make cloth, ceramic, metal, glass, and moss read differently.
7. Retune animation amplitude for Understory mode.
8. Export three PNGs: Shorer, Breathline Mechanic, Seed-Keeper.
9. Provide a short note on what existing builder limits prevented the art target from landing.

---

## 11. Expected output from Codex

Codex should return:

- A working Understory Crew Lab or mode.
- Three editable crew presets.
- Screenshots/exports of the three presets.
- A concise implementation note listing changed files.
- A short limitation note: what still looks too toy-like and what would require deeper engine/art work.

---

## 12. Copy-paste prompt to Codex

Please adapt the existing Hemstitch character designer into an Understory Crew Lab without rewriting the renderer or character system.

The creative target is **High-Air Heirloom Miniatures**: small readable humans in crafted survival gear, built from cloth, ceramic, leather, glass, stone, soot, and lantern light. They should feel like AD 9026 summit people adapted to thin air, the Veil, scarcity, and careful work inside an ancient machine mountain. They should not feel like plastic toy mascots or Supernauts-style astronauts.

Use the existing descriptor/builder architecture. Preserve compatibility with existing descriptors. Keep the existing Hem designer working. Reuse the current transform-only animation system, but reduce bounce and cheer for Understory mode.

Build three editable presets first:

1. **Shorer / Mason**: wrapped hood or low helmet, smoked goggles/visor, ceramic breath filter, timber/masonry gear, hammer or brace, heavy boots, muted clay/slate/dust-blue palette, focused expression.
2. **Breathline Mechanic**: compact helmet or half visor, oxidized metal fittings, pressure gauge/wrench/pipe tool, air canister or filter pack, subtle blue-green indicator, soot and repaired cloth, calm concentrated posture.
3. **Seed-Keeper**: moss-lined hood or cloth cap, seed pouches, small pot or water flask, patched gloves, soft cloth layers, restrained green/earth palette, reverent and practical rather than cute.

Add an Understory curated randomize/preset mode that avoids silly/fantasy/plush options. Add or adapt slot options for Veil hood, breath mask, smoked square visor, ceramic filter helmet, dust scarf, shoring helmet, survey lens hood, pressure hood, pipe pack, sample tube pack, rope bundle, and climb pack where practical.

Add a simple Understory preview/render mode, ideally with Studio Material, Summit Lamplight, and Bore Worklight environments. Do not build a full world scene.

If material differentiation requires changing the part data, add a small optional material/style field while keeping old descriptors rendering correctly. Test cloth, ceramic, leather, oxidized metal, smoked glass, moss/felt, stone/masonry, soot/dust.

Return the working mode plus exported PNGs of the three presets and a short note on limitations.
