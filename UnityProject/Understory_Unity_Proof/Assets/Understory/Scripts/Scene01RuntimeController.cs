using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Understory
{
    public enum Scene01Stage
    {
        WorkersGathering,
        HatchPartiallyExposed,
        HatchOpened,
        BoreRoomRevealed,
        ShoringPlaced,
        MaterialExtracted,
        HaulReturned,
        Refining,
        MaterialRefined,
        SceneComplete
    }

    public sealed class Scene01RuntimeController : MonoBehaviour
    {
        private const string SurfaceCameraName = "CaptureCamera_SurfaceRepairCluster";
        private const string BoreCameraName = "CaptureCamera_BoreRoomReveal";

        public Scene01Stage stage;
        public bool shelterRepaired;
        public bool terraceRepaired;
        public bool gardenRepaired;
        public bool playerBlockPlaced;
        public bool blockRemovalProofSeen;
        public bool sceneComplete;
        public string lastExtractionMode = "None";

        private readonly List<string> eventLog = new();
        private readonly Dictionary<string, int> inventory = new();
        private Transform sceneRoot;
        private Transform steward;
        private Transform stewardTarget;
        private Camera mainCamera;
        private Vector3 cameraTargetPosition;
        private Quaternion cameraTargetRotation;
        private bool hasCameraTarget;
        private bool isHaulAnimating;
        private bool isRefining;
        private float refineStartedAt;

        private void Awake()
        {
            CacheSceneReferences();
            ResetSceneRuntime();
        }

        private void Update()
        {
            HandlePointerInput();
            MoveSteward();
            MoveCamera();
        }

        private void OnGUI()
        {
            var box = new Rect(18f, 18f, 430f, Screen.height - 36f);
            GUILayout.BeginArea(box, GUI.skin.box);
            GUILayout.Label("Understory Scene 01");
            GUILayout.Label($"Stage: {GetStageTitle()}");
            GUILayout.Space(4f);
            GUILayout.Label(GetObjective());
            GUILayout.Space(8f);
            GUILayout.Label(GetInventoryText());
            GUILayout.Space(8f);

            foreach (var action in GetAvailableActions())
            {
                if (GUILayout.Button(action.label, GUILayout.Height(32f)))
                    TryInteract(action.id);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Event trail");
            var firstVisibleLog = Mathf.Max(0, eventLog.Count - 6);
            for (var i = firstVisibleLog; i < eventLog.Count; i++)
            {
                var line = eventLog[i];
                GUILayout.Label("- " + line);
            }
            GUILayout.EndArea();
        }

        public bool RunDeterministicVerification(string extractionInteractionId, out string report)
        {
            CacheSceneReferences();
            ResetSceneRuntime();

            var sequence = new[]
            {
                "surface_cut",
                "hatch_crust",
                "hatch",
                "bore_shoring",
                extractionInteractionId,
                "haul_table",
                "kiln",
                "surface_build_zone",
                "player_build_block",
                "surface_build_zone",
                "repair_shelter",
                "repair_terrace",
                "repair_garden"
            };

            foreach (var interactionId in sequence)
            {
                if (!TryInteract(interactionId, true))
                {
                    report = $"Scene01 deterministic flow failed at `{interactionId}` while stage was `{stage}`.";
                    return false;
                }
            }

            var complete = sceneComplete
                && shelterRepaired
                && terraceRepaired
                && gardenRepaired
                && playerBlockPlaced
                && blockRemovalProofSeen
                && stage == Scene01Stage.SceneComplete;

            report = complete
                ? $"Scene01 deterministic flow completed via `{extractionInteractionId}`."
                : "Scene01 deterministic flow ended without all completion flags.";
            return complete;
        }

        public bool TryInteract(string interactionId)
        {
            return TryInteract(interactionId, false);
        }

        public bool TryInteract(string interactionId, bool instant)
        {
            if (string.IsNullOrWhiteSpace(interactionId))
                return false;

            switch (interactionId)
            {
                case "surface_cut":
                    if (stage != Scene01Stage.WorkersGathering)
                        return false;
                    MoveStewardTo("Scene01Node_ShallowCut");
                    SetStage(Scene01Stage.HatchPartiallyExposed, "Workers hit worked stone below the shallow cut.");
                    return true;

                case "hatch_crust":
                    if (stage != Scene01Stage.HatchPartiallyExposed)
                        return false;
                    MoveStewardTo("Scene01Node_Hatch");
                    SetStage(Scene01Stage.HatchOpened, "The Steward clears the crust and finds the hatch handle.");
                    return true;

                case "hatch":
                    if (stage != Scene01Stage.HatchOpened)
                        return false;
                    MoveStewardTo("Scene01Node_Hatch");
                    SetStage(Scene01Stage.BoreRoomRevealed, "The hatch opens. The camera drops into the Bore Room.");
                    AimCameraAt(BoreCameraName);
                    return true;

                case "bore_shoring":
                    if (stage != Scene01Stage.BoreRoomRevealed)
                        return false;
                    MoveStewardTo("Scene01Node_BoreInspection");
                    SetActive("D_PlayerBuilt_BoreShoring_Committed", true);
                    SetActive("D_PlayerBuiltGhostDraft_BoreShoring", false);
                    SetStage(Scene01Stage.ShoringPlaced, "First shoring is set before the material face is touched.");
                    return true;

                case "trace_extract":
                    return ExtractMaterial("Trace", instant);

                case "blast_extract":
                    return ExtractMaterial("Blast", instant);

                case "haul_table":
                    if (stage != Scene01Stage.MaterialExtracted)
                        return false;
                    MoveStewardTo("Scene01Node_HaulTable");
                    if (instant)
                        CompleteHaulReturn();
                    else if (!isHaulAnimating)
                        StartCoroutine(AnimateHaulReturn());
                    return true;

                case "kiln":
                    if (stage != Scene01Stage.HaulReturned)
                        return false;
                    MoveStewardTo("Scene01Node_Kiln");
                    if (instant)
                        CompleteRefine();
                    else if (!isRefining)
                        StartCoroutine(RefineMaterial());
                    return true;

                case "surface_build_zone":
                    if (stage < Scene01Stage.MaterialRefined)
                        return false;
                    PlacePlayerBuildBlock();
                    return true;

                case "player_build_block":
                    if (stage < Scene01Stage.MaterialRefined || !playerBlockPlaced)
                        return false;
                    RemovePlayerBuildBlock();
                    return true;

                case "repair_shelter":
                    return RepairAnchor("shelter");

                case "repair_terrace":
                    return RepairAnchor("terrace");

                case "repair_garden":
                    return RepairAnchor("garden");

                default:
                    return false;
            }
        }

        private void CacheSceneReferences()
        {
            sceneRoot = FindTransformByName("Scene01_VisualProofPass1") ?? transform;
            steward = FindTransformByName("StewardPlaceholder_SummitSteward");
            mainCamera = Camera.main ?? FindObjectsByType<Camera>(FindObjectsSortMode.None).FirstOrDefault(camera => camera.enabled);
            if (mainCamera != null)
            {
                cameraTargetPosition = mainCamera.transform.position;
                cameraTargetRotation = mainCamera.transform.rotation;
            }
        }

        private void ResetSceneRuntime()
        {
            stage = Scene01Stage.WorkersGathering;
            shelterRepaired = false;
            terraceRepaired = false;
            gardenRepaired = false;
            playerBlockPlaced = false;
            blockRemovalProofSeen = false;
            sceneComplete = false;
            lastExtractionMode = "None";
            isHaulAnimating = false;
            isRefining = false;
            refineStartedAt = 0f;

            inventory.Clear();
            inventory["rawStone"] = 0;
            inventory["rawClay"] = 0;
            inventory["roughStone"] = 0;
            inventory["firedBrick"] = 0;
            inventory["seedclay"] = 0;
            inventory["timberBrace"] = 1;

            eventLog.Clear();
            eventLog.Add("Summit repair crew is short on usable material.");

            SetActive("02_BoreRoom_Blockout", false);
            SetActive("C_DestroyableRuin_HatchMineralCrust", true);
            SetLocalScale("C_DestroyableRuin_HatchMineralCrust", new Vector3(1.9f, 0.16f, 1.9f));
            SetActive("B_HatchState_TransitionGlow", false);
            SetActive("D_PlayerBuilt_BoreShoring_Committed", false);
            SetActive("D_PlayerBuiltGhostDraft_BoreShoring", true);
            SetActive("E_ExtractionVolume_BoreMaterialCache", true);
            SetActive("C_DestroyableRuin_EditableBoreDebris_A", true);
            SetActive("MaterialHaul_RoughStone", false);
            SetActive("MaterialHaul_Clay", false);
            SetActive("BuildMaterial_DressedStone", false);
            SetActive("BuildMaterial_Seedclay", false);
            SetActive("BuildMaterial_FiredBrick", false);
            SetActive("D_PlayerBuilt_ShelterWindbreak_Repaired", false);
            SetActive("D_PlayerBuilt_TerracePatch_Repaired", false);
            SetActive("D_PlayerBuilt_ViableGarden_Repaired", false);
            SetActive("D_PlayerBuilt_Block_01", false);

            SetMaterial("D_PlayerBuiltGhostDraft_ShelterPatch", "ghost_draft");
            SetMaterial("D_PlayerBuiltGhostDraft_TerraceStone", "ghost_draft");
            SetMaterial("D_PlayerBuiltGhostDraft_ViableSoilPreview", "ghost_draft");

            MoveStewardTo("Scene01Node_RepairCluster", true);
            AimCameraAt(SurfaceCameraName, true);
            ApplyStageVisuals();
        }

        private bool ExtractMaterial(string mode, bool instant)
        {
            if (stage != Scene01Stage.ShoringPlaced)
                return false;

            MoveStewardTo("Scene01Node_BoreInspection");
            lastExtractionMode = mode;
            inventory["rawStone"] += mode == "Blast" ? 3 : 2;
            inventory["rawClay"] += mode == "Trace" ? 3 : 2;
            SetStage(Scene01Stage.MaterialExtracted, $"{mode} extraction frees stone and clay without touching the protected shaft.");
            SetActive("E_ExtractionVolume_BoreMaterialCache", false);
            SetActive("C_DestroyableRuin_EditableBoreDebris_A", mode == "Trace");
            return true;
        }

        private IEnumerator AnimateHaulReturn()
        {
            isHaulAnimating = true;
            SetActive("MaterialHaul_RoughStone", true);
            SetActive("MaterialHaul_Clay", true);

            var stone = FindTransformByName("MaterialHaul_RoughStone");
            var clay = FindTransformByName("MaterialHaul_Clay");
            var cache = FindTransformByName("E_ExtractionVolume_BoreMaterialCache");
            var haulTable = FindTransformByName("Scene01Node_HaulTable");
            var start = cache != null ? cache.position : new Vector3(-7f, -8f, 21f);
            var endStone = haulTable != null ? haulTable.position + new Vector3(-0.35f, 0.35f, 0f) : new Vector3(1f, 1f, -8f);
            var endClay = haulTable != null ? haulTable.position + new Vector3(0.35f, 0.35f, 0f) : new Vector3(1.6f, 1f, -8f);

            var elapsed = 0f;
            while (elapsed < 1.25f)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.SmoothStep(0f, 1f, elapsed / 1.25f);
                if (stone != null)
                    stone.position = Vector3.Lerp(start, endStone, t);
                if (clay != null)
                    clay.position = Vector3.Lerp(start + Vector3.right * 0.45f, endClay, t);
                yield return null;
            }

            CompleteHaulReturn();
        }

        private void CompleteHaulReturn()
        {
            isHaulAnimating = false;
            SetActive("MaterialHaul_RoughStone", true);
            SetActive("MaterialHaul_Clay", true);
            SetStage(Scene01Stage.HaulReturned, "The haul returns physically through the table ritual stub.");
            AimCameraAt(SurfaceCameraName);
        }

        private IEnumerator RefineMaterial()
        {
            isRefining = true;
            refineStartedAt = Time.time;
            SetStage(Scene01Stage.Refining, "The kiln starts a short first-scene refine.");
            yield return new WaitForSeconds(2.25f);
            CompleteRefine();
        }

        private void CompleteRefine()
        {
            isRefining = false;
            inventory["rawStone"] = 0;
            inventory["rawClay"] = 0;
            inventory["roughStone"] += 4;
            inventory["firedBrick"] += 2;
            inventory["seedclay"] += 1;
            inventory["timberBrace"] += 2;

            SetActive("MaterialHaul_RoughStone", false);
            SetActive("MaterialHaul_Clay", false);
            SetActive("BuildMaterial_DressedStone", true);
            SetActive("BuildMaterial_Seedclay", true);
            SetActive("BuildMaterial_FiredBrick", true);
            SetStage(Scene01Stage.MaterialRefined, "Raw haul becomes buildable repair material.");
        }

        private bool RepairAnchor(string anchor)
        {
            if (stage < Scene01Stage.MaterialRefined)
                return false;

            switch (anchor)
            {
                case "shelter":
                    if (shelterRepaired || inventory["firedBrick"] < 1 || inventory["timberBrace"] < 1)
                        return false;
                    inventory["firedBrick"] -= 1;
                    inventory["timberBrace"] -= 1;
                    shelterRepaired = true;
                    SetActive("D_PlayerBuilt_ShelterWindbreak_Repaired", true);
                    SetMaterial("D_PlayerBuiltGhostDraft_ShelterPatch", "fired_brick");
                    eventLog.Add("Shelter windbreak repaired.");
                    break;

                case "terrace":
                    if (terraceRepaired || inventory["roughStone"] < 2)
                        return false;
                    inventory["roughStone"] -= 2;
                    terraceRepaired = true;
                    SetActive("D_PlayerBuilt_TerracePatch_Repaired", true);
                    SetActive("C_DestroyableRuin_TerraceGap_A", false);
                    SetMaterial("D_PlayerBuiltGhostDraft_TerraceStone", "rough_stone");
                    eventLog.Add("Terrace edge stabilized.");
                    break;

                case "garden":
                    if (gardenRepaired || inventory["seedclay"] < 1)
                        return false;
                    inventory["seedclay"] -= 1;
                    gardenRepaired = true;
                    SetActive("D_PlayerBuilt_ViableGarden_Repaired", true);
                    SetMaterial("D_PlayerBuiltGhostDraft_ViableSoilPreview", "seed_soil_viable");
                    eventLog.Add("Tiny soil bed becomes viable.");
                    break;

                default:
                    return false;
            }

            MoveStewardTo("Scene01Node_RepairCluster");
            CheckCompletion();
            return true;
        }

        private void PlacePlayerBuildBlock()
        {
            if (inventory["roughStone"] < 1 && !playerBlockPlaced)
            {
                eventLog.Add("Need one rough stone for the free-build proof.");
                return;
            }

            if (!playerBlockPlaced)
                inventory["roughStone"] -= 1;

            playerBlockPlaced = true;
            SetActive("D_PlayerBuilt_Block_01", true);
            MoveStewardTo("Scene01Node_BuildZone");
            eventLog.Add(blockRemovalProofSeen ? "Support block replaced in the build zone." : "Support block placed in the build zone.");
            CheckCompletion();
        }

        private void RemovePlayerBuildBlock()
        {
            playerBlockPlaced = false;
            blockRemovalProofSeen = true;
            inventory["roughStone"] += 1;
            SetActive("D_PlayerBuilt_Block_01", false);
            MoveStewardTo("Scene01Node_BuildZone");
            eventLog.Add("Support block removed cleanly; protected shells remain untouched.");
        }

        private void CheckCompletion()
        {
            if (shelterRepaired && terraceRepaired && gardenRepaired && playerBlockPlaced)
            {
                sceneComplete = true;
                stage = Scene01Stage.SceneComplete;
                ApplyStageVisuals();
                eventLog.Add("Scene complete: material from below repairs life above.");
            }
        }

        private void SetStage(Scene01Stage nextStage, string eventText)
        {
            stage = nextStage;
            eventLog.Add(eventText);
            ApplyStageVisuals();
        }

        private void ApplyStageVisuals()
        {
            SetActive("02_BoreRoom_Blockout", stage >= Scene01Stage.BoreRoomRevealed);
            SetActive("B_HatchState_TransitionGlow", stage >= Scene01Stage.HatchOpened);

            if (stage == Scene01Stage.HatchPartiallyExposed)
            {
                var crust = FindTransformByName("C_DestroyableRuin_HatchMineralCrust");
                if (crust != null)
                    crust.localScale = new Vector3(1.35f, 0.12f, 1.35f);
            }

            if (stage >= Scene01Stage.HatchOpened)
                SetActive("C_DestroyableRuin_HatchMineralCrust", false);
        }

        private void HandlePointerInput()
        {
            if (!Application.isPlaying || mainCamera == null)
                return;

            if (Input.GetMouseButtonUp(0))
                TryInteractAt(Input.mousePosition);

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
                TryInteractAt(Input.GetTouch(0).position);
        }

        private void TryInteractAt(Vector2 screenPosition)
        {
            var ray = mainCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 500f))
                return;

            var interactable = hit.collider.GetComponentInParent<Scene01Interactable>();
            if (interactable != null)
                TryInteract(interactable.interactionId);
        }

        private void MoveStewardTo(string nodeName, bool instant = false)
        {
            stewardTarget = FindTransformByName(nodeName);
            if (instant && steward != null && stewardTarget != null)
                steward.position = stewardTarget.position;
        }

        private void MoveSteward()
        {
            if (steward == null || stewardTarget == null)
                return;

            steward.position = Vector3.MoveTowards(steward.position, stewardTarget.position, 4.5f * Time.deltaTime);
            var direction = stewardTarget.position - steward.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
                steward.rotation = Quaternion.Slerp(steward.rotation, Quaternion.LookRotation(direction.normalized, Vector3.up), 8f * Time.deltaTime);
        }

        private void AimCameraAt(string cameraName, bool instant = false)
        {
            var targetCamera = FindTransformByName(cameraName);
            if (mainCamera == null || targetCamera == null)
                return;

            cameraTargetPosition = targetCamera.position;
            cameraTargetRotation = targetCamera.rotation;
            hasCameraTarget = true;

            if (instant)
            {
                mainCamera.transform.position = cameraTargetPosition;
                mainCamera.transform.rotation = cameraTargetRotation;
            }
        }

        private void MoveCamera()
        {
            if (!hasCameraTarget || mainCamera == null)
                return;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTargetPosition, 4f * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, cameraTargetRotation, 4f * Time.deltaTime);
        }

        private Transform FindTransformByName(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
                return null;

            var root = sceneRoot != null ? sceneRoot : transform;
            return root.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.name == objectName);
        }

        private void SetActive(string objectName, bool active)
        {
            var target = FindTransformByName(objectName);
            if (target != null)
                target.gameObject.SetActive(active);
        }

        private void SetLocalScale(string objectName, Vector3 scale)
        {
            var target = FindTransformByName(objectName);
            if (target != null)
                target.localScale = scale;
        }

        private void SetMaterial(string objectName, string materialName)
        {
            var target = FindTransformByName(objectName);
            if (target == null)
                return;

            var renderer = target.GetComponent<Renderer>();
            var material = FindMaterial(materialName);
            if (renderer != null && material != null)
                renderer.sharedMaterial = material;
        }

        private Material FindMaterial(string materialName)
        {
            var renderers = (sceneRoot != null ? sceneRoot : transform).GetComponentsInChildren<Renderer>(true);
            return renderers.Select(renderer => renderer.sharedMaterial).FirstOrDefault(material => material != null && material.name == materialName);
        }

        private string GetStageTitle()
        {
            return stage switch
            {
                Scene01Stage.WorkersGathering => "Damaged Summit",
                Scene01Stage.HatchPartiallyExposed => "Worked Stone Found",
                Scene01Stage.HatchOpened => "Hatch Ready",
                Scene01Stage.BoreRoomRevealed => "Bore Room Revealed",
                Scene01Stage.ShoringPlaced => "Shoring Placed",
                Scene01Stage.MaterialExtracted => "First Material Freed",
                Scene01Stage.HaulReturned => "Haul Returned",
                Scene01Stage.Refining => $"Refining {Mathf.Max(0f, 2.25f - (Time.time - refineStartedAt)):0.0}s",
                Scene01Stage.MaterialRefined => "Repair Material Ready",
                Scene01Stage.SceneComplete => "Scene Complete",
                _ => stage.ToString()
            };
        }

        private string GetObjective()
        {
            if (sceneComplete)
                return "The summit needed repair. The workers found the hatch. The Steward opened the Bore, returned material, and made the surface livable.";

            return stage switch
            {
                Scene01Stage.WorkersGathering => "Workers are gathering scarce material at the shallow cut.",
                Scene01Stage.HatchPartiallyExposed => "The Steward is called to clear the hatch crust.",
                Scene01Stage.HatchOpened => "Open the hidden hatch.",
                Scene01Stage.BoreRoomRevealed => "Set shoring before cutting the Bore Room material cache.",
                Scene01Stage.ShoringPlaced => "Choose a careful trace or a rough blast extraction.",
                Scene01Stage.MaterialExtracted => "Return the haul through the table ritual stub.",
                Scene01Stage.HaulReturned => "Refine the raw haul at the kiln.",
                Scene01Stage.Refining => "The kiln is making the first buildable material.",
                Scene01Stage.MaterialRefined => "Repair the windbreak, terrace edge, tiny garden, and prove one build block.",
                _ => "Continue the first scene loop."
            };
        }

        private string GetInventoryText()
        {
            return $"Inventory: raw stone {inventory["rawStone"]}, raw clay {inventory["rawClay"]}, rough stone {inventory["roughStone"]}, fired brick {inventory["firedBrick"]}, seedclay {inventory["seedclay"]}, timber {inventory["timberBrace"]}";
        }

        private IEnumerable<(string id, string label)> GetAvailableActions()
        {
            if (stage == Scene01Stage.WorkersGathering)
                yield return ("surface_cut", "Inspect shallow material cut");
            if (stage == Scene01Stage.HatchPartiallyExposed)
                yield return ("hatch_crust", "Clear hatch crust");
            if (stage == Scene01Stage.HatchOpened)
                yield return ("hatch", "Open hatch");
            if (stage == Scene01Stage.BoreRoomRevealed)
                yield return ("bore_shoring", "Set first shoring");
            if (stage == Scene01Stage.ShoringPlaced)
            {
                yield return ("trace_extract", "Trace extraction");
                yield return ("blast_extract", "Blast extraction");
            }
            if (stage == Scene01Stage.MaterialExtracted)
                yield return ("haul_table", "Return haul");
            if (stage == Scene01Stage.HaulReturned)
                yield return ("kiln", "Refine haul");
            if (stage >= Scene01Stage.MaterialRefined)
            {
                if (!playerBlockPlaced)
                    yield return ("surface_build_zone", "Place support block");
                else
                    yield return ("player_build_block", "Remove support block");
                if (!shelterRepaired)
                    yield return ("repair_shelter", "Repair shelter windbreak");
                if (!terraceRepaired)
                    yield return ("repair_terrace", "Repair terrace edge");
                if (!gardenRepaired)
                    yield return ("repair_garden", "Repair tiny garden");
            }
        }
    }
}
