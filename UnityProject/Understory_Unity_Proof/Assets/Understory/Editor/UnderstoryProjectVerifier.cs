using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    }
}
