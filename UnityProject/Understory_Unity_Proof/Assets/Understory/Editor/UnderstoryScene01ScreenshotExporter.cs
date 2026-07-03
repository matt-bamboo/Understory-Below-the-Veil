using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Understory.Editor
{
    public static class UnderstoryScene01ScreenshotExporter
    {
        private const string ScenePath = "Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity";

        public static void CaptureScene01ReferenceShots()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
                throw new InvalidOperationException($"Cannot capture Scene01 shots. Missing scene `{ScenePath}`.");

            EditorSceneManager.OpenScene(ScenePath);

            var reportFolder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../Reports"));
            Directory.CreateDirectory(reportFolder);

            CaptureCamera("Main Camera", Path.Combine(reportFolder, "scene01-main-camera.png"), 1600, 1000);
            CaptureCamera("CaptureCamera_SurfaceRepairCluster", Path.Combine(reportFolder, "scene01-surface-repair-cluster.png"), 1600, 1000);
            CaptureCamera("CaptureCamera_BoreRoomReveal", Path.Combine(reportFolder, "scene01-bore-room-reveal.png"), 1600, 1000);

            AssetDatabase.Refresh();
            Debug.Log("Scene01 reference screenshots exported.");
            EditorApplication.Exit(0);
        }

        private static void CaptureCamera(string cameraName, string path, int width, int height)
        {
            var cameraObject = GameObject.Find(cameraName);
            if (cameraObject == null)
                throw new InvalidOperationException($"Cannot capture Scene01 shot. Missing camera `{cameraName}`.");

            var camera = cameraObject.GetComponent<Camera>();
            if (camera == null)
                throw new InvalidOperationException($"Cannot capture Scene01 shot. `{cameraName}` has no Camera component.");

            var previousTarget = camera.targetTexture;
            var wasEnabled = camera.enabled;
            var previousActive = RenderTexture.active;

            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 4
            };
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            try
            {
                camera.enabled = true;
                camera.targetTexture = renderTexture;
                RenderTexture.active = renderTexture;
                camera.Render();
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();
                File.WriteAllBytes(path, texture.EncodeToPNG());
                Debug.Log($"Scene01 screenshot exported: {path}");
            }
            finally
            {
                camera.targetTexture = previousTarget;
                camera.enabled = wasEnabled;
                RenderTexture.active = previousActive;
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(renderTexture);
            }
        }
    }
}
