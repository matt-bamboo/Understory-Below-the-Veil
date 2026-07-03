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
                "Surface_BroadEditableZone_C_D_F",
                "F_RepairAnchor_ShelterWindbreak",
                "F_RepairAnchor_BrokenTerraceEdge",
                "F_RepairAnchor_TinySoilBed",
                "E_ExtractionVolume_ShallowSummitCut",
                "B_ProtectedLandmark_HiddenHatch_Current",
                "B_ProtectedLandmark_HiddenHatch_StateLine",
                "StewardPlaceholder_SummitSteward",
                "WorkerPlaceholder_ShallowCut_A",
                "02_BoreRoom_Blockout",
                "A_ProtectedShell_MainBoreShaft",
                "B_ProtectedLandmark_ProtectedShaftRim",
                "E_ExtractionVolume_BoreMaterialCache",
                "Works_MistEngine_ClearerHint_Inactive",
                "TheLines_InactiveWallConduit_A",
                "BlackVault_SealedHint"
            })
            {
                if (!allNames.Contains(requiredName))
                    failures.Add($"Scene01 blockout missing required object `{requiredName}`.");
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
                "world lungs"
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
