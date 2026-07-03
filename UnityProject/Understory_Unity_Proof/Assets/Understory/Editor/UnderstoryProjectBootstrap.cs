using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Understory.Editor
{
    public static class UnderstoryProjectBootstrap
    {
        private const string Root = "Assets/Understory";
        private const string ScenePath = Root + "/Scenes/Scene01_SummitHatch_BoreRoom.unity";
        private const string SettingsPath = Root + "/Settings";
        private const string RendererPath = SettingsPath + "/Understory_UniversalRenderer.asset";
        private const string PipelinePath = SettingsPath + "/Understory_URP.asset";

        public static void Configure()
        {
            EnsureFolders();
            var pipeline = EnsureUrpAssets();
            ConfigureProject(pipeline);
            EnsureScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Understory project bootstrap complete.");
        }

        private static void EnsureFolders()
        {
            foreach (var path in new[]
            {
                Root,
                Root + "/Art",
                Root + "/Editor",
                Root + "/Materials",
                Root + "/Prefabs",
                Root + "/Scenes",
                Root + "/Scripts",
                SettingsPath
            })
            {
                if (!AssetDatabase.IsValidFolder(path))
                    Directory.CreateDirectory(path);
            }
        }

        private static UniversalRenderPipelineAsset EnsureUrpAssets()
        {
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
                EditorUtility.SetDirty(renderer);
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            pipeline.supportsHDR = true;
            pipeline.msaaSampleCount = 4;
            pipeline.renderScale = 1f;
            pipeline.supportsCameraDepthTexture = true;
            pipeline.supportsCameraOpaqueTexture = true;

            EditorUtility.SetDirty(pipeline);
            return pipeline;
        }

        private static void ConfigureProject(RenderPipelineAsset pipeline)
        {
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
            QualitySettings.vSyncCount = 0;
            PlayerSettings.companyName = "Bamboo";
            PlayerSettings.productName = "Understory Unity Proof";
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        }

        private static void EnsureScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            {
                EditorBuildSettings.scenes = new[]
                {
                    new EditorBuildSettingsScene(ScenePath, true)
                };
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = "Scene01_SummitHatch_BoreRoom";

            var marker = new GameObject("Scene01_START_HERE");
            marker.transform.position = Vector3.zero;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }
    }
}
