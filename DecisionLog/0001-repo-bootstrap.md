# Decision 0001: Bootstrap Current Understory Repo

Timestamp: 7/3 9:07AM PT

## Decision

Use the current empty GitHub repo, `Understory-Below-the-Veil`, as the clean
Understory Unity production-proof repository.

## Evidence

- Local path is `/Users/matthewgrossman/Documents/GitHub/Understory-Below-the-Veil`.
- Remote is `https://github.com/matt-bamboo/Understory-Below-the-Veil`.
- Remote has no pushed heads yet.
- The handoff source suggested `Understory-Unity-Proof`, but this repo already
  exists as a fresh, empty Understory target.

## Consequence

The source packet is preserved here and the repo is ready for the first Unity
setup pass once Unity 6, URP/iOS Build Support, and Xcode are installed.

No Hemstitch stable branch or preserved reference branch is mutated by this
bootstrap.
