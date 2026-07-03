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
        private const float ArrivalDistance = 0.55f;

        public Scene01Stage stage;
        public bool shelterRepaired;
        public bool terraceRepaired;
        public bool gardenRepaired;
        public bool playerBlockPlaced;
        public bool blockRemovalProofSeen;
        public bool sceneComplete;
        public bool traceGestureComplete;
        public bool blastBurialActive;
        public bool blastBurialCleared;
        public bool coreSampleBandAdded;
        public bool archiveSeedPlaced;
        public bool draftWantListVisible;
        public float instability;
        public string lastExtractionMode = "None";

        private readonly List<string> eventLog = new();
        private readonly Dictionary<string, int> inventory = new();
        private readonly Dictionary<string, Scene01Interactable> interactablesById = new();
        private Transform sceneRoot;
        private Transform steward;
        private Transform stewardTarget;
        private Camera mainCamera;
        private Vector3 cameraTargetPosition;
        private Quaternion cameraTargetRotation;
        private bool hasCameraTarget;
        private bool isHaulAnimating;
        private bool isRefining;
        private bool isTraceGestureActive;
        private bool hasTracePointer;
        private float traceProgress;
        private float refineStartedAt;
        private Vector2 lastTracePointer;
        private string queuedInteractionId;

        private void Awake()
        {
            CacheSceneReferences();
            ResetSceneRuntime();
        }

        private void Update()
        {
            HandlePointerInput();
            HandleKeyboardInput();
            HandleTraceGesture();
            MoveSteward();
            ResolveQueuedInteractionIfArrived();
            MoveCamera();
            RefreshInteractableMarkers();
            PulseInteractableMarkers();
        }

        private void OnGUI()
        {
            var box = new Rect(18f, 18f, Mathf.Min(430f, Screen.width - 36f), Screen.height - 36f);
            GUILayout.BeginArea(box, GUI.skin.box);
            GUILayout.Label("Understory");
            GUILayout.Label($"Stage: {GetStageTitle()}");
            GUILayout.Space(4f);
            GUILayout.Label(GetObjective());
            GUILayout.Space(8f);
            GUILayout.Label(GetInventoryText());
            GUILayout.Space(8f);
            GUILayout.Label(GetTouchPrompt());
            if (isTraceGestureActive)
            {
                GUILayout.Space(6f);
                GUILayout.Label($"Trace seam: {Mathf.RoundToInt(traceProgress * 100f)}%");
                GUILayout.HorizontalSlider(traceProgress, 0f, 1f);
            }
            GUILayout.Space(8f);
            GUILayout.Label("Event trail");
            var firstVisibleLog = Mathf.Max(0, eventLog.Count - 6);
            for (var i = firstVisibleLog; i < eventLog.Count; i++)
            {
                var line = eventLog[i];
                GUILayout.Label("- " + line);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Restart scene", GUILayout.Height(30f)))
                ResetSceneRuntime();
            GUILayout.EndArea();
        }

        public bool RunDeterministicVerification(string extractionInteractionId, out string report)
        {
            CacheSceneReferences();
            ResetSceneRuntime();

            var sequence = new List<string>
            {
                "surface_cut",
                "hatch_crust",
                "hatch",
                "bore_shoring",
                extractionInteractionId
            };

            if (extractionInteractionId == "blast_extract")
                sequence.Add("reexcavate_burial");

            sequence.AddRange(new[]
            {
                "haul_table",
                "kiln",
                "core_sample",
                "archive_shelf",
                "surface_build_zone",
                "player_build_block",
                "surface_build_zone",
                "repair_shelter",
                "repair_terrace",
                "repair_garden"
            });

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
                && coreSampleBandAdded
                && archiveSeedPlaced
                && draftWantListVisible
                && (extractionInteractionId != "trace_extract" || traceGestureComplete)
                && (extractionInteractionId != "blast_extract" || (blastBurialActive && blastBurialCleared))
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
                    SetStage(Scene01Stage.HatchOpened, "The Steward clears the mineral crust and finds the hatch handle.");
                    return true;

                case "hatch":
                    if (stage != Scene01Stage.HatchOpened)
                        return false;
                    MoveStewardTo("Scene01Node_Hatch");
                    SetStage(Scene01Stage.BoreRoomRevealed, "The Bore opens. Darkness, old stone, and a vertical drop answer.");
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
                    if (instant)
                        return CompleteTraceExtraction(true);
                    return BeginTraceGesture();

                case "blast_extract":
                    return BlastExtractMaterial();

                case "reexcavate_burial":
                    if (stage != Scene01Stage.MaterialExtracted || !blastBurialActive || blastBurialCleared)
                        return false;
                    blastBurialCleared = true;
                    instability = Mathf.Max(0f, instability - 0.2f);
                    SetActive("CollapseBurial_BoreDebris", false);
                    SetActive("BlastImpact_Dust", false);
                    eventLog.Add("Crew clears the blast burial. The haul path is open again.");
                    PulseFeedback();
                    return true;

                case "haul_table":
                    if (stage != Scene01Stage.MaterialExtracted || (blastBurialActive && !blastBurialCleared))
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

                case "core_sample":
                    if (stage < Scene01Stage.MaterialRefined || coreSampleBandAdded)
                        return false;
                    coreSampleBandAdded = true;
                    SetActive("CoreSampleBand_FirstDescent", true);
                    eventLog.Add("Core Sample gains its first band from the Bore Room seam.");
                    MoveStewardTo("Scene01Node_CoreSample");
                    PulseFeedback();
                    return true;

                case "archive_shelf":
                    if (stage < Scene01Stage.MaterialRefined || archiveSeedPlaced)
                        return false;
                    archiveSeedPlaced = true;
                    SetActive("ArchiveSingular_FirstSeam", true);
                    eventLog.Add(traceGestureComplete
                        ? "Archive shelf receives the first intact seam fragment."
                        : "Archive shelf receives a scarred bulk fragment from the blast.");
                    MoveStewardTo("Scene01Node_ArchiveShelf");
                    PulseFeedback();
                    CheckCompletion();
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
            interactablesById.Clear();
            foreach (var interactable in (sceneRoot != null ? sceneRoot : transform).GetComponentsInChildren<Scene01Interactable>(true))
            {
                if (!string.IsNullOrWhiteSpace(interactable.interactionId))
                    interactablesById[interactable.interactionId] = interactable;
            }

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
            traceGestureComplete = false;
            blastBurialActive = false;
            blastBurialCleared = false;
            coreSampleBandAdded = false;
            archiveSeedPlaced = false;
            draftWantListVisible = false;
            instability = 0f;
            lastExtractionMode = "None";
            isHaulAnimating = false;
            isRefining = false;
            isTraceGestureActive = false;
            hasTracePointer = false;
            traceProgress = 0f;
            refineStartedAt = 0f;
            queuedInteractionId = null;

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
            SetActive("TraceSeam_Glow", false);
            SetActive("BlastImpact_Dust", false);
            SetActive("CollapseBurial_BoreDebris", false);
            SetActive("MaterialHaul_RoughStone", false);
            SetActive("MaterialHaul_Clay", false);
            SetActive("BuildMaterial_DressedStone", false);
            SetActive("BuildMaterial_Seedclay", false);
            SetActive("BuildMaterial_FiredBrick", false);
            SetActive("D_PlayerBuilt_ShelterWindbreak_Repaired", false);
            SetActive("D_PlayerBuilt_TerracePatch_Repaired", false);
            SetActive("D_PlayerBuilt_ViableGarden_Repaired", false);
            SetActive("D_PlayerBuilt_Block_01", false);
            SetActive("CoreSampleBand_FirstDescent", false);
            SetActive("ArchiveSingular_FirstSeam", false);
            SetActive("WantList_EastWallDraft", false);

            SetMaterial("D_PlayerBuiltGhostDraft_ShelterPatch", "ghost_draft");
            SetMaterial("D_PlayerBuiltGhostDraft_TerraceStone", "ghost_draft");
            SetMaterial("D_PlayerBuiltGhostDraft_ViableSoilPreview", "ghost_draft");

            MoveStewardTo("Scene01Node_RepairCluster", true);
            AimCameraAt(SurfaceCameraName, true);
            SetDesignerGuidesVisible(false);
            ApplyStageVisuals();
            RefreshInteractableMarkers();
        }

        private bool BeginTraceGesture()
        {
            if (stage != Scene01Stage.ShoringPlaced)
                return false;

            isTraceGestureActive = true;
            hasTracePointer = false;
            traceProgress = 0f;
            MoveStewardTo("Scene01Node_BoreInspection");
            SetActive("TraceSeam_Glow", true);
            eventLog.Add("Trace the lit seam with the field tool. Slow drag beats force.");
            PulseFeedback();
            return true;
        }

        private bool CompleteTraceExtraction(bool instant)
        {
            if (stage != Scene01Stage.ShoringPlaced && !isTraceGestureActive)
                return false;

            isTraceGestureActive = false;
            traceGestureComplete = true;
            hasTracePointer = false;
            traceProgress = 1f;
            instability = Mathf.Max(0f, instability - 0.1f);
            SetActive("TraceSeam_Glow", false);
            PulseFeedback();
            return ExtractMaterial("Trace", instant);
        }

        private bool BlastExtractMaterial()
        {
            if (stage != Scene01Stage.ShoringPlaced)
                return false;

            blastBurialActive = true;
            blastBurialCleared = false;
            instability = 0.75f;
            SetActive("BlastImpact_Dust", true);
            SetActive("CollapseBurial_BoreDebris", true);
            PulseFeedback();
            var extracted = ExtractMaterial("Blast", false);
            if (extracted)
                eventLog.Add("The blast frees more stone, but buries the return path.");
            return extracted;
        }

        private bool ExtractMaterial(string mode, bool instant)
        {
            if (stage != Scene01Stage.ShoringPlaced)
                return false;

            MoveStewardTo("Scene01Node_BoreInspection");
            lastExtractionMode = mode;
            inventory["rawStone"] += mode == "Blast" ? 3 : 2;
            inventory["rawClay"] += mode == "Trace" ? 3 : 2;
            SetStage(Scene01Stage.MaterialExtracted, $"{mode} extraction frees stone and clay without damaging the protected Bore.");
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
            SetStage(Scene01Stage.HaulReturned, "The Return Ritual brings the haul to the shafthead table.");
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
            SetStage(Scene01Stage.MaterialRefined, "Raw haul becomes buildable material. The Core Sample and Archive can record the first find.");
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
            SetActive("WantList_EastWallDraft", true);
            draftWantListVisible = true;
            MoveStewardTo("Scene01Node_BuildZone");
            eventLog.Add(blockRemovalProofSeen ? "Support block replaced. The larger draft still wants more material." : "First draft block committed. The want-list now gives the next dig a reason.");
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
            if (shelterRepaired && terraceRepaired && gardenRepaired && playerBlockPlaced && coreSampleBandAdded && archiveSeedPlaced)
            {
                sceneComplete = true;
                stage = Scene01Stage.SceneComplete;
                ApplyStageVisuals();
                eventLog.Add("Scene complete: down, up, return, build, and want all breathe.");
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

            if (isTraceGestureActive)
                return;

            if (Input.GetMouseButtonUp(0))
                TryInteractAt(Input.mousePosition);

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
                TryInteractAt(Input.GetTouch(0).position);
        }

        private void HandleKeyboardInput()
        {
            if (!Application.isPlaying)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                ResetSceneRuntime();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTraceGestureActive)
                {
                    traceProgress = Mathf.Min(1f, traceProgress + 0.18f);
                    if (traceProgress >= 1f)
                        CompleteTraceExtraction(false);
                    return;
                }

                var nextAction = GetPrimaryAvailableInteraction();
                if (!string.IsNullOrWhiteSpace(nextAction))
                    RequestWorldInteraction(nextAction);
            }
        }

        private void HandleTraceGesture()
        {
            if (!Application.isPlaying || !isTraceGestureActive)
                return;

            if (ReadPointerDown(out var pointerPosition))
            {
                if (hasTracePointer)
                {
                    var delta = Vector2.Distance(pointerPosition, lastTracePointer);
                    traceProgress += delta / Mathf.Max(240f, Screen.width * 0.42f);
                }

                hasTracePointer = true;
                lastTracePointer = pointerPosition;
                if (PointerHitsNamedObject(pointerPosition, "TraceSeam_Glow") || PointerHitsNamedObject(pointerPosition, "E_ExtractionVolume_BoreMaterialCache"))
                    traceProgress += Time.deltaTime * 0.28f;

                traceProgress = Mathf.Clamp01(traceProgress);
                if (traceProgress >= 1f)
                    CompleteTraceExtraction(false);
            }
            else
            {
                hasTracePointer = false;
            }
        }

        private void TryInteractAt(Vector2 screenPosition)
        {
            var ray = mainCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 500f))
                return;

            var interactable = hit.collider.GetComponentInParent<Scene01Interactable>();
            if (interactable != null)
            {
                RequestWorldInteraction(interactable.interactionId);
                return;
            }

            var node = FindNearestRuntimeNode(hit.point);
            if (node != null)
            {
                queuedInteractionId = null;
                stewardTarget = node;
                eventLog.Add("Steward moves to the nearest work node.");
            }
        }

        private void RequestWorldInteraction(string interactionId)
        {
            if (!IsInteractionAvailable(interactionId))
            {
                eventLog.Add(GetUnavailableReason(interactionId));
                return;
            }

            queuedInteractionId = interactionId;
            MoveStewardTo(GetApproachNodeName(interactionId));
            var interactable = interactablesById.TryGetValue(interactionId, out var found) ? found : null;
            var targetName = interactable != null && !string.IsNullOrWhiteSpace(interactable.displayName)
                ? interactable.displayName
                : interactionId;
            eventLog.Add($"Steward crossing to {targetName}.");
            ResolveQueuedInteractionIfArrived();
        }

        private void ResolveQueuedInteractionIfArrived()
        {
            if (string.IsNullOrWhiteSpace(queuedInteractionId) || steward == null)
                return;

            var node = FindTransformByName(GetApproachNodeName(queuedInteractionId));
            if (node != null && Vector3.Distance(steward.position, node.position) > ArrivalDistance)
                return;

            var interactionId = queuedInteractionId;
            queuedInteractionId = null;
            TryInteract(interactionId);
        }

        private bool ReadPointerDown(out Vector2 pointerPosition)
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                pointerPosition = touch.position;
                return touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            }

            pointerPosition = Input.mousePosition;
            return Input.GetMouseButton(0);
        }

        private bool PointerHitsNamedObject(Vector2 screenPosition, string objectName)
        {
            if (mainCamera == null)
                return false;

            var ray = mainCamera.ScreenPointToRay(screenPosition);
            return Physics.Raycast(ray, out var hit, 500f)
                && hit.collider.GetComponentsInParent<Transform>(true).Any(parent => parent.name == objectName);
        }

        private Transform FindNearestRuntimeNode(Vector3 point)
        {
            var root = sceneRoot != null ? sceneRoot : transform;
            return root.GetComponentsInChildren<Transform>(true)
                .Where(child => child.name.StartsWith("Scene01Node_", StringComparison.Ordinal))
                .OrderBy(child => Vector3.Distance(child.position, point))
                .FirstOrDefault();
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

        private bool IsInteractionAvailable(string interactionId)
        {
            return interactionId switch
            {
                "surface_cut" => stage == Scene01Stage.WorkersGathering,
                "hatch_crust" => stage == Scene01Stage.HatchPartiallyExposed,
                "hatch" => stage == Scene01Stage.HatchOpened,
                "bore_shoring" => stage == Scene01Stage.BoreRoomRevealed,
                "trace_extract" => stage == Scene01Stage.ShoringPlaced && !isTraceGestureActive,
                "blast_extract" => stage == Scene01Stage.ShoringPlaced && !isTraceGestureActive,
                "reexcavate_burial" => stage == Scene01Stage.MaterialExtracted && blastBurialActive && !blastBurialCleared,
                "haul_table" => stage == Scene01Stage.MaterialExtracted && (!blastBurialActive || blastBurialCleared),
                "kiln" => stage == Scene01Stage.HaulReturned,
                "core_sample" => stage >= Scene01Stage.MaterialRefined && !coreSampleBandAdded,
                "archive_shelf" => stage >= Scene01Stage.MaterialRefined && !archiveSeedPlaced,
                "surface_build_zone" => stage >= Scene01Stage.MaterialRefined && !playerBlockPlaced && inventory["roughStone"] >= 1,
                "player_build_block" => stage >= Scene01Stage.MaterialRefined && playerBlockPlaced && !sceneComplete,
                "repair_shelter" => stage >= Scene01Stage.MaterialRefined && !shelterRepaired && inventory["firedBrick"] >= 1 && inventory["timberBrace"] >= 1,
                "repair_terrace" => stage >= Scene01Stage.MaterialRefined && !terraceRepaired && inventory["roughStone"] >= 2,
                "repair_garden" => stage >= Scene01Stage.MaterialRefined && !gardenRepaired && inventory["seedclay"] >= 1,
                _ => false
            };
        }

        private string GetApproachNodeName(string interactionId)
        {
            return interactionId switch
            {
                "surface_cut" => "Scene01Node_ShallowCut",
                "hatch_crust" => "Scene01Node_Hatch",
                "hatch" => "Scene01Node_Hatch",
                "bore_shoring" => "Scene01Node_BoreInspection",
                "trace_extract" => "Scene01Node_BoreInspection",
                "blast_extract" => "Scene01Node_BoreInspection",
                "reexcavate_burial" => "Scene01Node_BoreInspection",
                "haul_table" => "Scene01Node_HaulTable",
                "kiln" => "Scene01Node_Kiln",
                "core_sample" => "Scene01Node_CoreSample",
                "archive_shelf" => "Scene01Node_ArchiveShelf",
                "surface_build_zone" => "Scene01Node_BuildZone",
                "player_build_block" => "Scene01Node_BuildZone",
                "repair_shelter" => "Scene01Node_RepairCluster",
                "repair_terrace" => "Scene01Node_RepairCluster",
                "repair_garden" => "Scene01Node_RepairCluster",
                _ => "Scene01Node_RepairCluster"
            };
        }

        private string GetPrimaryAvailableInteraction()
        {
            return GetAvailableActions().Select(action => action.id).FirstOrDefault();
        }

        private string GetUnavailableReason(string interactionId)
        {
            if (stage == Scene01Stage.MaterialExtracted && blastBurialActive && !blastBurialCleared && interactionId == "haul_table")
                return "The blast buried the return path. Clear the debris first.";
            if (stage < Scene01Stage.MaterialRefined && interactionId.StartsWith("repair_", StringComparison.Ordinal))
                return "The repair cluster needs refined material from below.";
            if (stage >= Scene01Stage.MaterialRefined && interactionId == "surface_build_zone" && inventory["roughStone"] < 1)
                return "Need one rough stone to commit that build block.";
            if (interactionId == "archive_shelf" && stage < Scene01Stage.MaterialRefined)
                return "The Archive needs a find from the first haul.";
            return "That is not the next useful action yet.";
        }

        private string GetTouchPrompt()
        {
            if (isTraceGestureActive)
                return "Drag along the glowing seam until the trace completes.";
            if (!string.IsNullOrWhiteSpace(queuedInteractionId))
                return "Steward is moving. The action will resolve at the work node.";

            var next = GetPrimaryAvailableInteraction();
            if (!string.IsNullOrWhiteSpace(next) && interactablesById.TryGetValue(next, out var interactable))
                return $"Tap the glowing marker: {interactable.displayName}.";

            return sceneComplete ? "Scene complete." : "Tap a glowing work target.";
        }

        private void RefreshInteractableMarkers()
        {
            foreach (var interactable in interactablesById.Values)
            {
                if (interactable == null || interactable.actionMarker == null)
                    continue;

                var isActiveTarget = IsInteractionAvailable(interactable.interactionId)
                    || queuedInteractionId == interactable.interactionId
                    || (isTraceGestureActive && interactable.interactionId == "trace_extract");
                interactable.actionMarker.gameObject.SetActive(isActiveTarget);
            }
        }

        private void PulseInteractableMarkers()
        {
            var pulse = 0.88f + Mathf.Sin(Time.time * 5.4f) * 0.12f;
            foreach (var interactable in interactablesById.Values)
            {
                if (interactable == null || interactable.actionMarker == null || !interactable.actionMarker.gameObject.activeSelf)
                    continue;
                interactable.actionMarker.localScale = Vector3.one * (0.52f * pulse);
                interactable.actionMarker.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
            }
        }

        private void SetDesignerGuidesVisible(bool visible)
        {
            var root = sceneRoot != null ? sceneRoot : transform;
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name.StartsWith("Label_", StringComparison.Ordinal)
                    || child.name == "00_EditabilityLegend_And_Guardrails")
                    child.gameObject.SetActive(visible);
            }
        }

        private void PulseFeedback()
        {
            if (Application.isBatchMode)
                return;

#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
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
                Scene01Stage.HaulReturned => "Return Ritual",
                Scene01Stage.Refining => $"Refining {Mathf.Max(0f, 2.25f - (Time.time - refineStartedAt)):0.0}s",
                Scene01Stage.MaterialRefined => "Build Material Ready",
                Scene01Stage.SceneComplete => "Scene Complete",
                _ => stage.ToString()
            };
        }

        private string GetObjective()
        {
            if (sceneComplete)
                return "The first loop breathes: descend, extract, return, refine, build, record, and want the next haul.";

            return stage switch
            {
                Scene01Stage.WorkersGathering => "The Summit Refuge is short on material. Tap the shallow cut where workers found worked stone.",
                Scene01Stage.HatchPartiallyExposed => "Clear the mineral crust from the hidden hatch.",
                Scene01Stage.HatchOpened => "Open the hatch into the Bore.",
                Scene01Stage.BoreRoomRevealed => "Set shoring before cutting the Bore Room material cache.",
                Scene01Stage.ShoringPlaced => "Choose a careful trace or a rough blast extraction.",
                Scene01Stage.MaterialExtracted => blastBurialActive && !blastBurialCleared ? "The blast buried the return path. Clear it before the haul can ascend." : "Return the haul through the Return Ritual.",
                Scene01Stage.HaulReturned => "Refine the raw haul at the kiln.",
                Scene01Stage.Refining => "The kiln is making the first buildable material.",
                Scene01Stage.MaterialRefined => "Record the Core Sample, place the Archive fragment, commit a draft block, and repair the summit.",
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
                yield return ("hatch", "Open Bore");
            if (stage == Scene01Stage.BoreRoomRevealed)
                yield return ("bore_shoring", "Set first shoring");
            if (stage == Scene01Stage.ShoringPlaced)
            {
                yield return ("trace_extract", "Trace extraction");
                yield return ("blast_extract", "Blast extraction");
            }
            if (stage == Scene01Stage.MaterialExtracted)
            {
                if (blastBurialActive && !blastBurialCleared)
                    yield return ("reexcavate_burial", "Clear blast burial");
                yield return ("haul_table", "Return haul");
            }
            if (stage == Scene01Stage.HaulReturned)
                yield return ("kiln", "Refine haul");
            if (stage >= Scene01Stage.MaterialRefined)
            {
                if (!coreSampleBandAdded)
                    yield return ("core_sample", "Add Core Sample band");
                if (!archiveSeedPlaced)
                    yield return ("archive_shelf", "Place Archive fragment");
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
