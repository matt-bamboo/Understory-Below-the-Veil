using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Understory.Editor
{
    public static class UnderstoryWebGLPreviewBuilder
    {
        private const string ScenePath = "Assets/Understory/Scenes/Scene01_SummitHatch_BoreRoom.unity";
        private const string DefaultOutputPath = "/tmp/understory-scene01-webgl";

        public static void BuildScene01WebGLPreview()
        {
            var outputPath = Environment.GetEnvironmentVariable("UNDERSTORY_WEBGL_OUT");
            if (string.IsNullOrWhiteSpace(outputPath))
                outputPath = DefaultOutputPath;

            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
            Directory.CreateDirectory(outputPath);

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            if (!scenes.Contains(ScenePath))
                scenes = new[] { ScenePath };

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
            PlayerSettings.WebGL.dataCaching = true;

            var report = BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.WebGL, BuildOptions.None);
            if (report.summary.result != BuildResult.Succeeded)
            {
                UnityEngine.Debug.LogError($"Scene01 WebGL preview build failed: {report.summary.result}");
                EditorApplication.Exit(1);
                return;
            }

            UnityEngine.Debug.Log($"Scene01 WebGL preview exported: {outputPath}");
            EditorApplication.Exit(0);
        }
    }
}
