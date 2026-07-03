using System.Collections.Generic;
using System.Linq;
using Understory;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Understory.Editor
{
    public static class UnderstoryProjectVerifier
    {
        private const string ScenePath = "Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity";

        public static void VerifyReady()
        {
            var failures = new List<string>();

            if (!Application.unityVersion.StartsWith("6000.5.2f1"))
                failures.Add($"Expected Unity 6000.5.2f1, got {Application.unityVersion}.");

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                failures.Add($"Expected active build target iOS, got {EditorUserBuildSettings.activeBuildTarget}.");

            if (GraphicsSettings.defaultRenderPipeline is not UniversalRenderPipelineAsset)
                failures.Add("Default graphics render pipeline is not URP.");

            if (QualitySettings.renderPipeline is not UniversalRenderPipelineAsset)
                failures.Add("Current quality render pipeline is not URP.");

            if (AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Understory/Settings/Understory_URP.asset") == null)
                failures.Add("Missing Understory URP asset.");

            if (AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Understory/Settings/Understory_UniversalRenderer.asset") == null)
                failures.Add("Missing Understory Universal Renderer asset.");

            if (AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Understory/Settings/Scene01_CinematicVolumeProfile.asset") == null)
                failures.Add("Missing Scene01 cinematic volume profile.");

            foreach (var texturePath in new[]
            {
                "Assets/Understory/Materials/Scene01_Blockout/ProceduralTextures/rough_stone_Stone.asset",
                "Assets/Understory/Materials/Scene01_Blockout/ProceduralTextures/roof_tile_Brick.asset",
                "Assets/Understory/Materials/Scene01_Blockout/ProceduralTextures/steward_cloth_Cloth.asset",
                "Assets/Understory/Materials/Scene01_Blockout/ProceduralTextures/glass_pane_Glass.asset"
            })
            {
                if (AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath) == null)
                    failures.Add($"Missing real-material procedural texture `{texturePath}`.");
            }

            foreach (var assetPath in new[]
            {
                "Assets/ThirdParty/KayKit/MedievalBuilder/License.txt",
                "Assets/ThirdParty/KayKit/MedievalBuilder/Objects/house.fbx",
                "Assets/ThirdParty/KayKit/MedievalBuilder/Objects/mine.fbx",
                "Assets/ThirdParty/KayKit/MedievalBuilder/Objects/farm_plot.fbx",
                "Assets/ThirdParty/KayKit/MedievalBuilder/Tiles/square_forest_detail.fbx",
                "Assets/ThirdParty/KayKit/MedievalBuilder/Tiles/square_rock_detail.fbx"
            })
            {
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) == null)
                    failures.Add($"Missing imported CC0 KayKit scene asset `{assetPath}`.");
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
                failures.Add("Missing Scene01 starter scene.");

            if (!EditorBuildSettings.scenes.Any(scene => scene.path == ScenePath && scene.enabled))
                failures.Add("Scene01 starter scene is not enabled in build settings.");

            VerifyScene01Blockout(failures);

            if (failures.Count > 0)
            {
                foreach (var failure in failures)
                    Debug.LogError(failure);
                EditorApplication.Exit(1);
                return;
            }

            Debug.Log("Understory project verification passed.");
            EditorApplication.Exit(0);
        }

        private static void VerifyScene01Blockout(List<string> failures)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (sceneAsset == null)
                return;

            var scene = EditorSceneManager.OpenScene(ScenePath);
            var allTransforms = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .ToList();
            var allNames = allTransforms.Select(transform => transform.name).ToHashSet();

            foreach (var requiredName in new[]
            {
                "Scene01_VisualProofPass1",
                "Scene01_CinematicColorVolume",
                "Surface_BroadEditableZone_C_D_F",
                "SummitRefuge_RealMaterialMiniature",
                "KayKitHero_SurfaceRefuge_MaterialMiniature",
                "KayKit_SummitRefuge_House",
                "KayKit_HiddenHatch_MineMouth",
                "KayKit_HatchMineralCrust_Hero",
                "KayKit_FirstGarden_FarmPlot",
                "KayKit_Windbreak_WallStraight",
                "KayKit_Terrace_WallCorner",
                "KayKitHero_CrewVisibleMiniatures",
                "KayKitHero_StewardFigure",
                "KayKit_ReturnTable_Hero",
                "KayKit_FirstKiln_Hero",
                "KayKit_SurfaceBuildPad_Hero",
                "KayKit_PlayerSupportBlock_Hero",
                "KayKit_CoreSample_WellLikeShafthead",
                "KayKit_ArchiveShelf_Hero",
                "KayKit_RepairedShelterPatch_Hero",
                "KayKit_ViableGarden_Repaired_Hero",
                "RefugeRoof_LeftPlane",
                "RefugeWaterCatch_Glass",
                "Surface_TactileMaterialScatter",
                "F_RepairAnchor_ShelterWindbreak",
                "F_RepairAnchor_BrokenTerraceEdge",
                "F_RepairAnchor_TinySoilBed",
                "E_ExtractionVolume_ShallowSummitCut",
                "B_ProtectedLandmark_HiddenHatch_Current",
                "B_ProtectedLandmark_HiddenHatch_StateLine",
                "StewardPlaceholder_SummitSteward",
                "Steward_HighAirHelmet_BrickCap",
                "WorkerPlaceholder_ShallowCut_A",
                "02_BoreRoom_Blockout",
                "BoreRoom_RealMaterialDressings",
                "KayKitHero_BoreRoom_AncientWorks",
                "KayKit_BoreMaterialFace_MineAsset",
                "KayKit_BoreShoring_RoofedTimber",
                "KayKit_BoreShoring_Committed_Hero",
                "KayKit_BoreTraceSeam_Hero",
                "KayKit_BoreCollapseBurial_Hero",
                "BoreFloor_AncientPaver_00",
                "BoreLanternString_WarmPoint_00",
                "A_ProtectedShell_MainBoreShaft",
                "B_ProtectedLandmark_ProtectedShaftRim",
                "E_ExtractionVolume_BoreMaterialCache",
                "TraceSeam_Glow",
                "BlastImpact_Dust",
                "CollapseBurial_BoreDebris",
                "Works_MistEngine_ClearerHint_Inactive",
                "TheLines_InactiveWallConduit_A",
                "BlackVault_SealedHint",
                "BlackVault_SealedFrame_Top",
                "D_PlayerBuilt_BoreShoring_Committed",
                "CoreSample_GlassColumn",
                "CoreSampleBand_FirstDescent",
                "ArchiveShelf_Phase0",
                "ArchiveSingular_FirstSeam",
                "WantList_EastWallDraft",
                "FarGlass_BlockedByVeil",
                "D_PlayerBuilt_ShelterWindbreak_Repaired",
                "D_PlayerBuilt_TerracePatch_Repaired",
                "D_PlayerBuilt_ViableGarden_Repaired",
                "D_PlayerBuilt_Block_01",
                "BuildMaterial_DressedStone",
                "BuildMaterial_Seedclay",
                "BuildMaterial_FiredBrick",
                "Scene01Node_RepairCluster",
                "Scene01Node_BoreInspection"
            })
            {
                if (!allNames.Contains(requiredName))
                    failures.Add($"Scene01 blockout missing required object `{requiredName}`.");
            }

            var interactables = allTransforms
                .Select(transform => transform.GetComponent<Scene01Interactable>())
                .Where(interactable => interactable != null)
                .ToList();
            var interactableIds = interactables.Select(interactable => interactable.interactionId).ToHashSet();

            foreach (var requiredInteraction in new[]
            {
                "surface_cut",
                "hatch_crust",
                "hatch",
                "bore_shoring",
                "trace_extract",
                "blast_extract",
                "reexcavate_burial",
                "haul_table",
                "kiln",
                "core_sample",
                "archive_shelf",
                "surface_build_zone",
                "player_build_block",
                "repair_shelter",
                "repair_terrace",
                "repair_garden"
            })
            {
                if (!interactableIds.Contains(requiredInteraction))
                    failures.Add($"Scene01 runtime missing interactable `{requiredInteraction}`.");
            }

            foreach (var interactable in interactables)
            {
                if (interactable.actionMarker == null)
                    failures.Add($"Scene01 interactable `{interactable.interactionId}` is missing an in-world action marker.");
            }

            var runtimeController = scene.GetRootGameObjects()
                .Select(root => root.GetComponentInChildren<Scene01RuntimeController>(true))
                .FirstOrDefault(controller => controller != null);
            if (runtimeController == null)
            {
                failures.Add("Scene01 runtime is missing Scene01RuntimeController.");
            }
            else
            {
                if (!runtimeController.RunDeterministicVerification("trace_extract", out var traceReport))
                    failures.Add(traceReport);
                else
                    Debug.Log(traceReport);
                if (!runtimeController.RunDeterministicVerification("blast_extract", out var blastReport))
                    failures.Add(blastReport);
                else
                    Debug.Log(blastReport);
            }

            var markers = allTransforms
                .Select(transform => transform.GetComponent<UnderstoryEditabilityMarker>())
                .Where(marker => marker != null)
                .ToList();

            if (markers.Count < 20)
                failures.Add($"Expected at least 20 editability markers in Scene01, found {markers.Count}.");

            foreach (var requiredClass in new[]
            {
                UnderstoryEditabilityClass.ProtectedShell,
                UnderstoryEditabilityClass.ProtectedLandmark,
                UnderstoryEditabilityClass.DestroyableRuin,
                UnderstoryEditabilityClass.PlayerBuilt,
                UnderstoryEditabilityClass.ExtractionVolume,
                UnderstoryEditabilityClass.RepairAnchor
            })
            {
                if (!markers.Any(marker => marker.editabilityClass == requiredClass))
                    failures.Add($"Scene01 blockout missing editability class `{requiredClass}`.");
            }

            foreach (var marker in markers.Where(marker => marker.phaseZeroRequired))
            {
                if (string.IsNullOrWhiteSpace(marker.gameplayRole))
                    failures.Add($"Required marker `{marker.name}` is missing a gameplay role.");
            }

            var forbiddenTerms = new[]
            {
                "Old Lung",
                "Old Lungs",
                "Breathline",
                "Breathlines",
                "Breathstone",
                "Lungstone",
                "mountain breathes",
                "breathing mountain",
                "world lungs",
                "magic bloodline",
                "chosen one",
                "loot box",
                "gacha",
                "coins",
                "premium currency"
            };

            foreach (var transform in allTransforms)
            {
                var marker = transform.GetComponent<UnderstoryEditabilityMarker>();
                var haystack = $"{transform.name} {marker?.gameplayRole} {marker?.sourceOfTruthNote}";
                foreach (var forbiddenTerm in forbiddenTerms)
                {
                    if (haystack.IndexOf(forbiddenTerm, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        failures.Add($"Scene01 uses deprecated term `{forbiddenTerm}` on `{transform.name}`.");
                }
            }
        }
    }
}
