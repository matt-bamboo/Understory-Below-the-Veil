using System;
using System.Collections.Generic;
using System.IO;
using Understory;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Understory.Editor
{
    public static class UnderstoryScene01VisualBlockoutBuilder
    {
        private const string Root = "Assets/Understory";
        private const string ScenePath = Root + "/Scenes/Scene01_SummitHatch_BoreRoom.unity";
        private const string MaterialFolder = Root + "/Materials/Scene01_Blockout";
        private const string ImportedMaterialFolder = MaterialFolder + "/Imported";
        private const string TextureFolder = MaterialFolder + "/ProceduralTextures";
        private const string KayKitObjectFolder = "Assets/ThirdParty/KayKit/MedievalBuilder/Objects";
        private const string KayKitTileFolder = "Assets/ThirdParty/KayKit/MedievalBuilder/Tiles";

        private static readonly Dictionary<string, Material> Materials = new();
        private static readonly Dictionary<string, Texture2D> MaterialTextures = new();
        private static readonly Dictionary<string, Material> ImportedMaterialCache = new();

        private enum SurfacePattern
        {
            Stone,
            Brick,
            Clay,
            Wood,
            Grass,
            Soil,
            Cloth,
            Glass,
            Ceramic,
            Metal,
            Glow,
            Snow,
            Veil,
            Ghost
        }

        public static void BuildScene01VisualProofPass1()
        {
            EnsureFolders();
            EnsureMaterials();
            PrepareImportedHeroAssets();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Scene01_SummitHatch_BoreRoom";

            var root = new GameObject("Scene01_VisualProofPass1");
            AddMarker(root, UnderstoryEditabilityClass.StoryGuide, "Scene 01 playable proof root. Scope: v7 first-scene loop.", true);

            BuildLightingAndCameras(root.transform);
            BuildSurface(root.transform);
            BuildBoreRoom(root.transform);
            BuildLegend(root.transform);
            BuildRuntimeFlow(root.transform);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);

            AssetDatabase.SaveAssets();
            NormalizeGeneratedAssetWhitespace();
            AssetDatabase.Refresh();
            Debug.Log("Scene 01 visual proof pass 1 blockout built.");
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory(MaterialFolder);
            Directory.CreateDirectory(ImportedMaterialFolder);
            Directory.CreateDirectory(TextureFolder);
            Directory.CreateDirectory(Root + "/Scripts");
            Directory.CreateDirectory(Root + "/Scenes");
            Directory.CreateDirectory(Root + "/Settings");
        }

        private static void EnsureMaterials()
        {
            Materials.Clear();
            MaterialTextures.Clear();
            ImportedMaterialCache.Clear();

            CreateMaterial("summit_snow", new Color(0.48f, 0.48f, 0.42f), 0.92f, pattern: SurfacePattern.Snow);
            CreateMaterial("summit_stone", new Color(0.36f, 0.34f, 0.29f), 0.9f, pattern: SurfacePattern.Stone);
            CreateMaterial("rough_stone", new Color(0.34f, 0.32f, 0.28f), 0.9f, pattern: SurfacePattern.Stone);
            CreateMaterial("clay_cut", new Color(0.58f, 0.34f, 0.24f), 0.92f, pattern: SurfacePattern.Clay);
            CreateMaterial("timber_brace", new Color(0.43f, 0.27f, 0.15f), 0.78f, pattern: SurfacePattern.Wood);
            CreateMaterial("fired_brick", new Color(0.62f, 0.27f, 0.17f), 0.86f, pattern: SurfacePattern.Brick);
            CreateMaterial("warm_plaster", new Color(0.67f, 0.57f, 0.44f), 0.88f, pattern: SurfacePattern.Clay);
            CreateMaterial("roof_tile", new Color(0.64f, 0.26f, 0.16f), 0.88f, pattern: SurfacePattern.Brick);
            CreateMaterial("moss_grass", new Color(0.23f, 0.31f, 0.15f), 0.94f, pattern: SurfacePattern.Grass);
            CreateMaterial("seed_soil_dead", new Color(0.2f, 0.16f, 0.12f), 0.96f, pattern: SurfacePattern.Soil);
            CreateMaterial("seed_soil_viable", new Color(0.24f, 0.33f, 0.17f), 0.94f, pattern: SurfacePattern.Grass);
            CreateMaterial("blackglass", new Color(0.03f, 0.05f, 0.06f), 0.18f, metallic: 0.05f, pattern: SurfacePattern.Glass);
            CreateMaterial("glass_pane", new Color(0.5f, 0.75f, 0.85f, 0.42f), 0.25f, transparent: true, pattern: SurfacePattern.Glass);
            CreateMaterial("water_cold", new Color(0.1f, 0.38f, 0.48f, 0.72f), 0.18f, transparent: true, pattern: SurfacePattern.Glass);
            CreateMaterial("filterstone", new Color(0.42f, 0.55f, 0.5f), 0.88f, pattern: SurfacePattern.Stone);
            CreateMaterial("line_ceramic", new Color(0.72f, 0.66f, 0.55f), 0.72f, pattern: SurfacePattern.Ceramic);
            CreateMaterial("worn_metal", new Color(0.48f, 0.43f, 0.36f), 0.48f, metallic: 0.55f, pattern: SurfacePattern.Metal);
            CreateMaterial("hollower_mark", new Color(0.55f, 0.08f, 0.05f), 0.86f, pattern: SurfacePattern.Clay);
            CreateMaterial("steward_cloth", new Color(0.16f, 0.28f, 0.31f), 0.9f, pattern: SurfacePattern.Cloth);
            CreateMaterial("worker_cloth", new Color(0.45f, 0.4f, 0.31f), 0.9f, pattern: SurfacePattern.Cloth);
            CreateMaterial("lantern_glow", new Color(1f, 0.67f, 0.28f), 0.28f, 2.6f, pattern: SurfacePattern.Glow);
            CreateMaterial("bore_dark", new Color(0.025f, 0.024f, 0.022f), 0.96f, pattern: SurfacePattern.Stone);
            CreateMaterial("ghost_draft", new Color(0.35f, 0.72f, 0.86f, 0.24f), 0.5f, transparent: true, pattern: SurfacePattern.Ghost);
            CreateMaterial("editable_zone", new Color(0.3f, 0.6f, 0.45f, 0.07f), 0.88f, transparent: true, pattern: SurfacePattern.Ghost);
            CreateMaterial("veil", new Color(0.55f, 0.72f, 0.74f, 0.28f), 0.95f, transparent: true, pattern: SurfacePattern.Veil);
            CreateMaterial("protected_zone", new Color(0.2f, 0.24f, 0.3f, 0.08f), 0.86f, transparent: true, pattern: SurfacePattern.Veil);
        }

        private static void PrepareImportedHeroAssets()
        {
            ImportAssetsInFolder(KayKitObjectFolder);
            ImportAssetsInFolder(KayKitTileFolder);
        }

        private static void ImportAssetsInFolder(string projectFolder)
        {
            if (!Directory.Exists(projectFolder))
                return;

            foreach (var path in Directory.EnumerateFiles(projectFolder, "*.fbx", SearchOption.TopDirectoryOnly))
                AssetDatabase.ImportAsset(path.Replace('\\', '/'), ImportAssetOptions.ForceSynchronousImport);
        }

        private static void CreateMaterial(string id, Color color, float roughness, float emission = 0f, bool transparent = false, float metallic = 0f, SurfacePattern pattern = SurfacePattern.Stone)
        {
            var path = $"{MaterialFolder}/{id}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, path);
            }

            mat.name = id;
            mat.color = color;
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", Mathf.Clamp01(1f - roughness));
            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", metallic);

            var texture = CreateTextureAsset(id, color, pattern);
            if (texture != null)
            {
                if (mat.HasProperty("_BaseMap"))
                    mat.SetTexture("_BaseMap", texture);
                if (mat.HasProperty("_MainTex"))
                    mat.SetTexture("_MainTex", texture);
                var tiling = GetTextureTiling(pattern);
                if (mat.HasProperty("_BaseMap"))
                    mat.SetTextureScale("_BaseMap", tiling);
                if (mat.HasProperty("_MainTex"))
                    mat.SetTextureScale("_MainTex", tiling);
            }

            if (emission > 0f)
            {
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", color * emission);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", Color.black);
            }

            if (transparent)
            {
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                if (mat.HasProperty("_Surface"))
                    mat.SetFloat("_Surface", 1f);
                if (mat.HasProperty("_Blend"))
                    mat.SetFloat("_Blend", 0f);
                if (mat.HasProperty("_AlphaClip"))
                    mat.SetFloat("_AlphaClip", 0f);
                if (mat.HasProperty("_ZWrite"))
                    mat.SetFloat("_ZWrite", 0f);
            }
            else
            {
                mat.SetOverrideTag("RenderType", "Opaque");
                mat.renderQueue = -1;
                if (mat.HasProperty("_Surface"))
                    mat.SetFloat("_Surface", 0f);
                if (mat.HasProperty("_ZWrite"))
                    mat.SetFloat("_ZWrite", 1f);
            }

            EditorUtility.SetDirty(mat);
            Materials[id] = mat;
        }

        private static Texture2D CreateTextureAsset(string id, Color baseColor, SurfacePattern pattern)
        {
            var path = $"{TextureFolder}/{id}_{pattern}.asset";
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
            {
                texture = new Texture2D(128, 128, TextureFormat.RGBA32, true)
                {
                    name = $"{id}_{pattern}",
                    wrapMode = TextureWrapMode.Repeat,
                    filterMode = FilterMode.Bilinear
                };
                AssetDatabase.CreateAsset(texture, path);
            }

            FillTexture(texture, baseColor, pattern, StableSeed(id));
            EditorUtility.SetDirty(texture);
            MaterialTextures[id] = texture;
            return texture;
        }

        private static void FillTexture(Texture2D texture, Color baseColor, SurfacePattern pattern, int seed)
        {
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var u = x / (float)texture.width;
                    var v = y / (float)texture.height;
                    var noise = Mathf.PerlinNoise((u * 9.5f) + seed * 0.017f, (v * 9.5f) + seed * 0.029f);
                    var fine = Hash01(seed, x, y);
                    var color = pattern switch
                    {
                        SurfacePattern.Brick => BrickPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Stone => StonePixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Clay => ClayPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Wood => WoodPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Grass => GrassPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Soil => SoilPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Cloth => ClothPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Glass => GlassPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Ceramic => CeramicPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Metal => MetalPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Glow => GlowPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Snow => SnowPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Veil => VeilPixel(baseColor, u, v, noise, fine),
                        SurfacePattern.Ghost => GhostPixel(baseColor, u, v, noise, fine),
                        _ => StonePixel(baseColor, u, v, noise, fine)
                    };
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply(true, false);
        }

        private static Color BrickPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var row = Mathf.FloorToInt(v * 8f);
            var offset = row % 2 == 0 ? 0f : 0.5f;
            var mortar = Mathf.Abs(Frac((u + offset) * 5f) - 0.5f) > 0.45f || Mathf.Abs(Frac(v * 8f) - 0.5f) > 0.43f;
            var color = Adjust(baseColor, -0.18f + noise * 0.28f + (fine - 0.5f) * 0.12f);
            return mortar ? Adjust(new Color(0.2f, 0.17f, 0.14f, baseColor.a), fine * 0.08f) : color;
        }

        private static Color StonePixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var hairline = Mathf.Abs(Mathf.Sin((u * 19f + v * 7f) * Mathf.PI)) > 0.985f;
            var color = Adjust(baseColor, -0.14f + noise * 0.24f + (fine - 0.5f) * 0.1f);
            return hairline ? Adjust(color, -0.22f) : color;
        }

        private static Color ClayPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var striation = Mathf.Sin((v * 32f + noise * 2f) * Mathf.PI) * 0.04f;
            return Adjust(baseColor, -0.1f + noise * 0.18f + striation + (fine - 0.5f) * 0.08f);
        }

        private static Color WoodPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var grain = Mathf.Sin((u * 30f + noise * 5f) * Mathf.PI) * 0.12f;
            var knot = Mathf.PerlinNoise(u * 3f + 8f, v * 5f + 3f) > 0.72f ? -0.18f : 0f;
            return Adjust(baseColor, grain + knot + (fine - 0.5f) * 0.05f);
        }

        private static Color GrassPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var blade = Mathf.Abs(Mathf.Sin((u * 70f + fine * 4f) * Mathf.PI)) > 0.84f ? 0.18f : 0f;
            return Adjust(baseColor, -0.12f + noise * 0.22f + blade);
        }

        private static Color SoilPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var pebble = fine > 0.94f ? 0.22f : 0f;
            return Adjust(baseColor, -0.12f + noise * 0.16f + pebble);
        }

        private static Color ClothPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var weave = (Mathf.Sin(u * 96f * Mathf.PI) + Mathf.Sin(v * 96f * Mathf.PI)) * 0.025f;
            return Adjust(baseColor, -0.08f + noise * 0.12f + weave + (fine - 0.5f) * 0.04f);
        }

        private static Color GlassPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var streak = Mathf.Abs(Mathf.Sin((u + v * 0.35f) * 28f * Mathf.PI)) > 0.95f ? 0.22f : 0f;
            return Adjust(baseColor, -0.05f + noise * 0.08f + streak);
        }

        private static Color CeramicPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var crack = Mathf.Abs(Mathf.Sin((u * 11f - v * 13f) * Mathf.PI)) > 0.988f ? -0.2f : 0f;
            return Adjust(baseColor, -0.08f + noise * 0.12f + crack);
        }

        private static Color MetalPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var scratch = Mathf.Abs(Mathf.Sin((u * 55f + v * 3f) * Mathf.PI)) > 0.965f ? 0.18f : 0f;
            return Adjust(baseColor, -0.09f + noise * 0.13f + scratch);
        }

        private static Color GlowPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var center = 1f - Mathf.Clamp01(Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f)) * 1.7f);
            return Adjust(baseColor, noise * 0.12f + center * 0.28f);
        }

        private static Color SnowPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var grit = fine > 0.9f ? -0.12f : 0f;
            return Adjust(baseColor, -0.04f + noise * 0.1f + grit);
        }

        private static Color VeilPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var cloud = Mathf.PerlinNoise(u * 3f + 4f, v * 2f + 10f) * 0.22f;
            return Adjust(baseColor, -0.06f + cloud);
        }

        private static Color GhostPixel(Color baseColor, float u, float v, float noise, float fine)
        {
            var grid = Mathf.Abs(Frac(u * 4f) - 0.5f) > 0.47f || Mathf.Abs(Frac(v * 4f) - 0.5f) > 0.47f;
            return grid ? Adjust(baseColor, 0.22f) : Adjust(baseColor, -0.04f + noise * 0.08f);
        }

        private static Vector2 GetTextureTiling(SurfacePattern pattern)
        {
            return pattern switch
            {
                SurfacePattern.Brick => new Vector2(2.4f, 2.4f),
                SurfacePattern.Stone => new Vector2(2.1f, 2.1f),
                SurfacePattern.Wood => new Vector2(1.6f, 2.8f),
                SurfacePattern.Grass => new Vector2(3.2f, 3.2f),
                SurfacePattern.Cloth => new Vector2(2.8f, 2.8f),
                _ => new Vector2(1.8f, 1.8f)
            };
        }

        private static Color Adjust(Color color, float delta)
        {
            return new Color(
                Mathf.Clamp01(color.r + delta),
                Mathf.Clamp01(color.g + delta),
                Mathf.Clamp01(color.b + delta),
                color.a);
        }

        private static float Frac(float value)
        {
            return value - Mathf.Floor(value);
        }

        private static int StableSeed(string id)
        {
            unchecked
            {
                var hash = 23;
                foreach (var c in id)
                    hash = hash * 31 + c;
                return Mathf.Abs(hash);
            }
        }

        private static float Hash01(int seed, int x, int y)
        {
            unchecked
            {
                uint n = (uint)seed;
                n ^= (uint)x * 374761393u;
                n ^= (uint)y * 668265263u;
                n = (n ^ (n >> 13)) * 1274126177u;
                n ^= n >> 16;
                return n / (float)uint.MaxValue;
            }
        }

        private static void NormalizeGeneratedAssetWhitespace()
        {
            NormalizeFileWhitespace(ScenePath);
            NormalizeFileWhitespace(MaterialFolder + ".meta");

            foreach (var path in Directory.EnumerateFiles(MaterialFolder))
            {
                if (path.EndsWith(".mat", StringComparison.Ordinal) || path.EndsWith(".meta", StringComparison.Ordinal))
                    NormalizeFileWhitespace(path);
            }

            if (Directory.Exists(ImportedMaterialFolder))
            {
                foreach (var path in Directory.EnumerateFiles(ImportedMaterialFolder))
                {
                    if (path.EndsWith(".mat", StringComparison.Ordinal) || path.EndsWith(".meta", StringComparison.Ordinal))
                        NormalizeFileWhitespace(path);
                }
            }
        }

        private static void NormalizeFileWhitespace(string path)
        {
            if (!File.Exists(path))
                return;

            var lines = File.ReadAllLines(path);
            for (var i = 0; i < lines.Length; i++)
                lines[i] = lines[i].TrimEnd();
            File.WriteAllLines(path, lines);
        }

        private static void BuildLightingAndCameras(Transform root)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.38f, 0.44f, 0.45f);
            RenderSettings.fogDensity = 0.017f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.62f, 0.56f, 0.46f);
            RenderSettings.ambientEquatorColor = new Color(0.28f, 0.35f, 0.34f);
            RenderSettings.ambientGroundColor = new Color(0.08f, 0.07f, 0.06f);
            RenderSettings.ambientIntensity = 0.88f;

            var sun = new GameObject("Sun_WarmLowAngle");
            sun.transform.SetParent(root);
            sun.transform.SetPositionAndRotation(new Vector3(-14f, 11f, -18f), Quaternion.Euler(28f, -41f, 0f));
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.78f, 0.48f);
            light.intensity = 2.75f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.78f;

            CreateLight("BoreRoom_LanternKey", root, new Vector3(0f, -7f, 17f), new Color(1f, 0.55f, 0.25f), 6.2f, 18f);
            CreateLight("Surface_WorkLanterns", root, new Vector3(-7.2f, 3.7f, -2.2f), new Color(1f, 0.63f, 0.28f), 2.35f, 13f);
            CreateLight("GoldenRim_FromVeil", root, new Vector3(14f, 5.6f, -14f), new Color(1f, 0.47f, 0.2f), 1.4f, 28f);
            CreateLight("Surface_GlassKick", root, new Vector3(-2.2f, 4.1f, 3.9f), new Color(0.55f, 0.78f, 1f), 1.15f, 10f);

            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            camera.transform.SetParent(root);
            camera.transform.position = new Vector3(-13.2f, 5.7f, -8.6f);
            LookAt(camera.transform, new Vector3(-6.2f, 0.9f, -0.28f));
            var cam = camera.AddComponent<Camera>();
            cam.fieldOfView = 34f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.29f, 0.28f, 0.25f);
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 500f;
            cam.allowHDR = true;
            EnablePostProcessing(cam);
            camera.AddComponent<AudioListener>();

            var boreCamera = new GameObject("CaptureCamera_BoreRoomReveal");
            boreCamera.transform.SetParent(root);
            boreCamera.transform.position = new Vector3(7.1f, -5.5f, 31.6f);
            LookAt(boreCamera.transform, new Vector3(-2.4f, -9.1f, 19.8f));
            var boreCam = boreCamera.AddComponent<Camera>();
            boreCam.enabled = false;
            boreCam.fieldOfView = 30f;
            boreCam.allowHDR = true;
            boreCam.clearFlags = CameraClearFlags.SolidColor;
            boreCam.backgroundColor = new Color(0.04f, 0.036f, 0.032f);
            EnablePostProcessing(boreCam);

            var surfaceCamera = new GameObject("CaptureCamera_SurfaceRepairCluster");
            surfaceCamera.transform.SetParent(root);
            surfaceCamera.transform.position = new Vector3(-13.2f, 5.7f, -8.6f);
            LookAt(surfaceCamera.transform, new Vector3(-6.2f, 0.9f, -0.28f));
            var surfaceCam = surfaceCamera.AddComponent<Camera>();
            surfaceCam.enabled = false;
            surfaceCam.fieldOfView = 34f;
            surfaceCam.clearFlags = CameraClearFlags.SolidColor;
            surfaceCam.backgroundColor = new Color(0.29f, 0.28f, 0.25f);
            surfaceCam.allowHDR = true;
            EnablePostProcessing(surfaceCam);

            BuildPostProcessVolume(root);
        }

        private static void EnablePostProcessing(Camera camera)
        {
            var cameraData = camera.GetComponent<UniversalAdditionalCameraData>() ?? camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            cameraData.renderPostProcessing = true;
            cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            cameraData.antialiasingQuality = AntialiasingQuality.High;
        }

        private static void BuildPostProcessVolume(Transform root)
        {
            var profilePath = Root + "/Settings/Scene01_CinematicVolumeProfile.asset";
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, profilePath);
            }

            profile.components.Clear();

            var bloom = profile.Add<Bloom>(true);
            bloom.threshold.Override(0.78f);
            bloom.intensity.Override(0.42f);
            bloom.scatter.Override(0.56f);

            var color = profile.Add<ColorAdjustments>(true);
            color.postExposure.Override(-0.18f);
            color.contrast.Override(12f);
            color.saturation.Override(-10f);
            color.colorFilter.Override(new Color(1f, 0.94f, 0.86f));

            var vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(0.18f);
            vignette.smoothness.Override(0.62f);

            var depth = profile.Add<DepthOfField>(true);
            depth.mode.Override(DepthOfFieldMode.Gaussian);
            depth.gaussianStart.Override(18f);
            depth.gaussianEnd.Override(44f);
            depth.gaussianMaxRadius.Override(0.55f);

            var volumeGo = new GameObject("Scene01_CinematicColorVolume");
            volumeGo.transform.SetParent(root);
            var volume = volumeGo.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.profile = profile;
            AddMarker(volumeGo, UnderstoryEditabilityClass.StoryGuide, "Global cinematic color/bloom volume for real-material miniature target.", false);
        }

        private static void BuildSurface(Transform root)
        {
            var surface = CreateGroup("01_Surface_SummitWorkyard", root);

            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_MountainSummitMass", surface, new Vector3(0f, -1.75f, 0f), Vector3.Scale(new Vector3(28f, 0.85f, 22f), Vector3.one), Materials["summit_stone"], UnderstoryEditabilityClass.ProtectedShell, "Authored mountain shell. Not editable.", true);
            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_SummitSnowCap", surface, new Vector3(0f, 0.12f, 0f), new Vector3(21f, 0.42f, 16f), Materials["summit_snow"], UnderstoryEditabilityClass.ProtectedShell, "Authored summit cap and silhouette.", true);
            var veil = CreatePrimitive(PrimitiveType.Cube, "Veil_Atmosphere_BelowSummit", surface, new Vector3(0f, -8.2f, 12f), new Vector3(48f, 3.5f, 18f), Materials["veil"], UnderstoryEditabilityClass.StoryGuide, "Visible Veil below the workyard. Progression wall only in this pass.", true);
            veil.SetActive(false);
            var editableOverlay = CreatePrimitive(PrimitiveType.Cube, "Surface_BroadEditableZone_C_D_F", surface, new Vector3(-2f, 0.38f, -2f), new Vector3(18f, 0.08f, 11f), Materials["editable_zone"], UnderstoryEditabilityClass.StoryGuide, "Broad editable/buildable zone overlay for ruined and repairable surface pieces.", true);
            editableOverlay.SetActive(false);

            BuildSummitRefugeMiniature(surface);
            BuildRepairCluster(surface);
            BuildShallowCutAndHatch(surface);
            BuildCharacters(surface);
            BuildReturnAndRefineStubs(surface);
            BuildShaftheadProgress(surface);
            BuildSurfaceMaterialScatter(surface);
            BuildKayKitSurfaceHero(surface);

            AddLabel(surface, "Label_SurfaceScope", "First playable: Summit Refuge + sealed Bore + Return Ritual + draft want-list", new Vector3(-2f, 5.2f, -10f), 0.42f);
        }

        private static void BuildSummitRefugeMiniature(Transform surface)
        {
            var refuge = CreateGroup("SummitRefuge_RealMaterialMiniature", surface);
            refuge.localPosition = new Vector3(-5.4f, 0.48f, 2.2f);

            for (var x = -2; x <= 2; x++)
            {
                for (var z = -2; z <= 1; z++)
                {
                    var lift = ((x + z) % 2 == 0) ? 0.02f : -0.015f;
                    CreatePrimitive(PrimitiveType.Cube, $"RefugeFoundation_Stone_{x + 2}_{z + 2}", refuge, new Vector3(x * 0.72f, 0.02f + lift, z * 0.72f), new Vector3(0.68f, 0.28f, 0.68f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Tactile stone foundation block.", false);
                }
            }

            CreatePrimitive(PrimitiveType.Cube, "RefugeWall_WarmPlaster_Back", refuge, new Vector3(0f, 1.02f, 0.92f), new Vector3(3.3f, 1.55f, 0.28f), Materials["warm_plaster"], UnderstoryEditabilityClass.RepairAnchor, "Warm worn plaster wall for summit refuge.", true);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWall_WarmPlaster_Left", refuge, new Vector3(-1.66f, 1.02f, 0f), new Vector3(0.28f, 1.55f, 1.8f), Materials["warm_plaster"], UnderstoryEditabilityClass.RepairAnchor, "Warm worn plaster wall for summit refuge.", true);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWall_WarmPlaster_Right", refuge, new Vector3(1.66f, 1.02f, 0f), new Vector3(0.28f, 1.55f, 1.8f), Materials["warm_plaster"], UnderstoryEditabilityClass.RepairAnchor, "Warm worn plaster wall for summit refuge.", true);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWall_FrontLowMended", refuge, new Vector3(0.48f, 0.85f, -0.92f), new Vector3(2.32f, 1.18f, 0.28f), Materials["warm_plaster"], UnderstoryEditabilityClass.RepairAnchor, "Front wall with repaired threshold.", true);
            CreatePrimitive(PrimitiveType.Cube, "RefugeDoor_Timber", refuge, new Vector3(-0.85f, 0.73f, -1.08f), new Vector3(0.54f, 1.08f, 0.16f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Weathered timber door.", false);

            CreatePrimitive(PrimitiveType.Cube, "RefugeBeam_Ridge", refuge, new Vector3(0f, 2.25f, 0f), new Vector3(3.85f, 0.22f, 0.22f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Timber ridge beam.", false);
            CreateRotatedPrimitive(PrimitiveType.Cube, "RefugeRoof_LeftPlane", refuge, new Vector3(-0.88f, 2.08f, 0f), new Vector3(2.0f, 0.18f, 2.45f), Quaternion.Euler(0f, 0f, -24f), Materials["roof_tile"], UnderstoryEditabilityClass.PlayerBuilt, "Real-material roof tile plane.", true);
            CreateRotatedPrimitive(PrimitiveType.Cube, "RefugeRoof_RightPlane", refuge, new Vector3(0.88f, 2.08f, 0f), new Vector3(2.0f, 0.18f, 2.45f), Quaternion.Euler(0f, 0f, 24f), Materials["roof_tile"], UnderstoryEditabilityClass.PlayerBuilt, "Real-material roof tile plane.", true);

            for (var side = -1; side <= 1; side += 2)
            {
                for (var row = 0; row < 5; row++)
                {
                    for (var col = -2; col <= 2; col++)
                    {
                        var y = 1.78f + row * 0.16f + Mathf.Abs(col) * 0.015f;
                        var x = side * (0.42f + row * 0.19f);
                        var z = col * 0.45f + (row % 2 == 0 ? 0.05f : -0.05f);
                        CreateRotatedPrimitive(PrimitiveType.Cube, $"RefugeRoof_Tile_{side}_{row}_{col + 2}", refuge, new Vector3(x, y, z), new Vector3(0.48f, 0.11f, 0.36f), Quaternion.Euler(0f, 0f, side * 24f), Materials["roof_tile"], UnderstoryEditabilityClass.PlayerBuilt, "Individual brick roof tile for tactile miniature read.", false);
                    }
                }
            }

            CreatePrimitive(PrimitiveType.Cube, "RefugeWindow_Glass_Left", refuge, new Vector3(-1.18f, 1.16f, -1.09f), new Vector3(0.34f, 0.45f, 0.08f), Materials["glass_pane"], UnderstoryEditabilityClass.PlayerBuilt, "Cold glass window catching light.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWindow_Glass_Right", refuge, new Vector3(0.98f, 1.16f, -1.09f), new Vector3(0.42f, 0.48f, 0.08f), Materials["glass_pane"], UnderstoryEditabilityClass.PlayerBuilt, "Cold glass window catching light.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWindow_Crossbar_A", refuge, new Vector3(0.98f, 1.16f, -1.15f), new Vector3(0.05f, 0.5f, 0.07f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Tiny timber window crossbar.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWindow_Crossbar_B", refuge, new Vector3(0.98f, 1.16f, -1.16f), new Vector3(0.44f, 0.05f, 0.07f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Tiny timber window crossbar.", false);

            CreatePrimitive(PrimitiveType.Cube, "RefugeStoneSteps_A", refuge, new Vector3(-0.85f, 0.26f, -1.54f), new Vector3(0.86f, 0.18f, 0.42f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Worn stone threshold step.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeStoneSteps_B", refuge, new Vector3(-0.85f, 0.12f, -1.9f), new Vector3(1.05f, 0.16f, 0.38f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Worn stone threshold step.", false);

            CreatePrimitive(PrimitiveType.Cube, "RefugeWaterCatch_Frame", refuge, new Vector3(1.86f, 0.62f, -1.34f), new Vector3(1.15f, 0.18f, 0.92f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Timber frame for summit water catchment.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWaterCatch_Glass", refuge, new Vector3(1.86f, 0.83f, -1.34f), new Vector3(0.98f, 0.08f, 0.78f), Materials["glass_pane"], UnderstoryEditabilityClass.PlayerBuilt, "Glass catchment panel with real-material transparency.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeWaterCatch_Water", refuge, new Vector3(1.86f, 0.72f, -1.34f), new Vector3(0.88f, 0.05f, 0.68f), Materials["water_cold"], UnderstoryEditabilityClass.StoryGuide, "Small water read: not a water system yet.", false);

            CreatePrimitive(PrimitiveType.Cube, "RefugeToolCrate_Timber", refuge, new Vector3(2.35f, 0.52f, 0.75f), new Vector3(0.58f, 0.48f, 0.58f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Crew tool crate.", false);
            CreatePrimitive(PrimitiveType.Cube, "RefugeMetalBlock_Sample", refuge, new Vector3(2.18f, 0.58f, -0.28f), new Vector3(0.44f, 0.34f, 0.44f), Materials["worn_metal"], UnderstoryEditabilityClass.StoryGuide, "Small worn metal material sample.", false);
            CreatePrimitive(PrimitiveType.Sphere, "RefugeLantern_WarmWindow", refuge, new Vector3(-1.28f, 1.68f, -1.18f), Vector3.one * 0.18f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm lantern glow in refuge window.", false);

            CreateShrub(refuge, "RefugeShrub_Left", new Vector3(-2.38f, 0.55f, -1.32f), 0.58f);
            CreateShrub(refuge, "RefugeShrub_Right", new Vector3(2.64f, 0.54f, -1.78f), 0.42f);
            CreateGrassTufts(refuge, "RefugeGrass", new Vector3(-0.2f, 0.42f, -2.28f), 12, 2.8f, 0.55f);

            refuge.gameObject.SetActive(false);
        }

        private static void BuildRepairCluster(Transform surface)
        {
            var cluster = CreateGroup("FirstRepairCluster_ShelterTerraceGarden", surface);

            CreatePrimitive(PrimitiveType.Cube, "F_RepairAnchor_ShelterWindbreak", cluster, new Vector3(-7.5f, 1.05f, -1.8f), new Vector3(4.5f, 1.8f, 0.55f), Materials["rough_stone"], UnderstoryEditabilityClass.RepairAnchor, "First surface repair anchor: shelter/windbreak improves after haul.", true);
            CreatePrimitive(PrimitiveType.Cube, "C_DestroyableRuin_ShelterBrokenWall_Left", cluster, new Vector3(-10.1f, 1.85f, -1.8f), new Vector3(0.55f, 1.8f, 0.7f), Materials["rough_stone"], UnderstoryEditabilityClass.DestroyableRuin, "Damaged shelter piece: removable/rebuildable ruin.", true);
            CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuiltGhostDraft_ShelterPatch", cluster, new Vector3(-5.25f, 1.9f, -1.8f), new Vector3(0.75f, 1.3f, 0.75f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Ghost draft for first repair placement.", true);
            var shelterComplete = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_ShelterWindbreak_Repaired", cluster, new Vector3(-5.65f, 1.48f, -1.8f), new Vector3(1.55f, 1.55f, 0.65f), Materials["fired_brick"], UnderstoryEditabilityClass.PlayerBuilt, "Committed shelter/windbreak repair result.", true);
            shelterComplete.SetActive(false);
            CreatePrimitive(PrimitiveType.Cube, "TimberBrace_Shelter_A", cluster, new Vector3(-8.8f, 2.25f, -1.35f), new Vector3(0.22f, 2.8f, 0.22f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Timber brace placeholder for repair material.", false);
            CreatePrimitive(PrimitiveType.Cube, "TimberBrace_Shelter_B", cluster, new Vector3(-6.3f, 2.25f, -1.35f), new Vector3(0.22f, 2.8f, 0.22f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Timber brace placeholder for repair material.", false);

            CreatePrimitive(PrimitiveType.Cube, "F_RepairAnchor_BrokenTerraceEdge", cluster, new Vector3(-2.2f, 0.55f, -6.4f), new Vector3(8.2f, 0.9f, 1.3f), Materials["rough_stone"], UnderstoryEditabilityClass.RepairAnchor, "Broken terrace edge repair anchor: stabilizes buildable land.", true);
            CreatePrimitive(PrimitiveType.Cube, "C_DestroyableRuin_TerraceGap_A", cluster, new Vector3(-0.2f, 0.95f, -6.4f), new Vector3(1.2f, 0.7f, 1.45f), Materials["bore_dark"], UnderstoryEditabilityClass.DestroyableRuin, "Visible wound in terrace edge before repair.", true);
            CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuiltGhostDraft_TerraceStone", cluster, new Vector3(1.1f, 1.05f, -6.4f), new Vector3(1.1f, 0.85f, 1.35f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Ghost stone patch for terrace repair.", true);
            var terraceComplete = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_TerracePatch_Repaired", cluster, new Vector3(0.45f, 1.02f, -6.4f), new Vector3(2.25f, 0.82f, 1.35f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Committed terrace edge repair result.", true);
            terraceComplete.SetActive(false);

            CreatePrimitive(PrimitiveType.Cube, "F_RepairAnchor_TinySoilBed", cluster, new Vector3(-9.2f, 0.42f, -5.8f), new Vector3(3.2f, 0.26f, 2.2f), Materials["seed_soil_dead"], UnderstoryEditabilityClass.RepairAnchor, "Tiny dead soil bed: first emotional restoration target.", true);
            CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuiltGhostDraft_ViableSoilPreview", cluster, new Vector3(-9.2f, 0.58f, -5.8f), new Vector3(2.2f, 0.08f, 1.2f), Materials["seed_soil_viable"], UnderstoryEditabilityClass.PlayerBuilt, "Preview of viable soil after repair.", true);
            var gardenComplete = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_ViableGarden_Repaired", cluster, new Vector3(-9.2f, 0.7f, -5.8f), new Vector3(2.15f, 0.16f, 1.15f), Materials["seed_soil_viable"], UnderstoryEditabilityClass.PlayerBuilt, "Committed viable garden repair result.", true);
            gardenComplete.SetActive(false);
            for (var i = 0; i < 5; i++)
            {
                var sprout = CreatePrimitive(PrimitiveType.Capsule, $"GardenSprout_{i:00}", gardenComplete.transform, new Vector3(-0.8f + i * 0.4f, 0.35f, 0f), new Vector3(0.08f, 0.22f, 0.08f), Materials["seed_soil_viable"], UnderstoryEditabilityClass.PlayerBuilt, "Tiny garden sprout repair read.", false);
                sprout.transform.localRotation = Quaternion.Euler(0f, i * 17f, 0f);
            }

            AddLabel(cluster, "Label_FirstRepairCluster", "First repair cluster: windbreak + terrace edge + tiny soil bed", new Vector3(-7.2f, 4.6f, -5.2f), 0.32f);
            cluster.gameObject.SetActive(false);
        }

        private static void BuildShallowCutAndHatch(Transform surface)
        {
            var cut = CreateGroup("BoreDiscovery_ShallowMaterialCut", surface);
            CreatePrimitive(PrimitiveType.Cube, "E_ExtractionVolume_ShallowSummitCut", cut, new Vector3(5.4f, 0.2f, -1.3f), new Vector3(6.5f, 0.42f, 3.6f), Materials["clay_cut"], UnderstoryEditabilityClass.ExtractionVolume, "Workers gather scarce clay/stone here and uncover the hidden hatch.", true);
            CreatePrimitive(PrimitiveType.Cube, "E_ExtractionVolume_RoughStonePocket", cut, new Vector3(7.2f, 0.65f, -1.2f), new Vector3(1.35f, 0.75f, 1.25f), Materials["rough_stone"], UnderstoryEditabilityClass.ExtractionVolume, "Trace/blast candidate material pocket.", true);
            CreatePrimitive(PrimitiveType.Cube, "B_ProtectedLandmark_HiddenHatch_Current", cut, new Vector3(4.2f, 0.62f, -1.1f), new Vector3(1.65f, 0.16f, 1.65f), Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Central hatch landmark. Protected; state is authored.", true);
            CreatePrimitive(PrimitiveType.Cube, "C_DestroyableRuin_HatchMineralCrust", cut, new Vector3(4.2f, 0.78f, -1.1f), new Vector3(1.9f, 0.16f, 1.9f), Materials["filterstone"], UnderstoryEditabilityClass.DestroyableRuin, "Removable mineral crust/debris covering the hatch.", true);

            var states = CreateGroup("B_ProtectedLandmark_HiddenHatch_StateLine", cut);
            var stateNames = new[]
            {
                "01_CoveredByDebris",
                "02_PartiallyExposed",
                "03_Opened",
                "04_TransitionToBoreRoom"
            };
            for (var i = 0; i < stateNames.Length; i++)
            {
                var x = 0.6f + i * 2.15f;
                CreatePrimitive(PrimitiveType.Cube, $"B_HatchState_{stateNames[i]}", states, new Vector3(x, 1.15f, 2.5f), new Vector3(1.45f, 0.18f, 1.45f), i < 2 ? Materials["filterstone"] : Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, $"Storyboard hatch state {i + 1}: {stateNames[i]}.", true);
                if (i == 3)
                    CreatePrimitive(PrimitiveType.Cube, "B_HatchState_TransitionGlow", states, new Vector3(x, 0.82f, 2.5f), new Vector3(0.7f, 0.44f, 0.7f), Materials["lantern_glow"], UnderstoryEditabilityClass.ProtectedLandmark, "Visual transition cue down into Bore Room.", true);
            }

            AddLabel(cut, "Label_HatchDiscovery", "Workers disturb the sealed Bore while gathering scarce material", new Vector3(5.1f, 3.7f, 2.4f), 0.3f);
            cut.gameObject.SetActive(false);
        }

        private static void BuildCharacters(Transform surface)
        {
            var characters = CreateGroup("VisibleCrewPlaceholders", surface);

            CreateCapsule("StewardPlaceholder_SummitSteward", characters, new Vector3(-1.5f, 1.25f, -1.4f), 0.55f, 1.9f, Materials["steward_cloth"], UnderstoryEditabilityClass.StoryGuide, "Visible Summit Steward placeholder. Soft tap-to-move node target.");
            CreatePrimitive(PrimitiveType.Sphere, "Steward_Head_WarmFace", characters, new Vector3(-1.5f, 2.28f, -1.4f), new Vector3(0.42f, 0.38f, 0.42f), Materials["warm_plaster"], UnderstoryEditabilityClass.StoryGuide, "Readable visible Steward head placeholder.", false);
            CreatePrimitive(PrimitiveType.Cube, "Steward_HighAirHelmet_BrickCap", characters, new Vector3(-1.5f, 2.66f, -1.4f), new Vector3(0.84f, 0.24f, 0.68f), Materials["roof_tile"], UnderstoryEditabilityClass.StoryGuide, "High-air heirloom helmet material read.", false);
            CreatePrimitive(PrimitiveType.Cube, "Steward_Headlamp_GlassGlow", characters, new Vector3(-1.5f, 2.68f, -1.86f), new Vector3(0.34f, 0.22f, 0.12f), Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Tiny headlamp glow for readable crew silhouette.", false);
            CreatePrimitive(PrimitiveType.Cube, "Steward_Backpack_TimberFrame", characters, new Vector3(-1.5f, 1.55f, -0.9f), new Vector3(0.72f, 0.92f, 0.22f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Weathered backpack frame for the hands-on repair lead.", false);
            CreatePrimitive(PrimitiveType.Cube, "Steward_Scarf_ClothWrap", characters, new Vector3(-1.5f, 2.02f, -1.67f), new Vector3(0.72f, 0.16f, 0.18f), Materials["worker_cloth"], UnderstoryEditabilityClass.StoryGuide, "Cloth wrap: tactile fabric material read.", false);
            CreatePrimitive(PrimitiveType.Cube, "StewardPlaceholder_LineKey_FieldTool", characters, new Vector3(-0.85f, 1.8f, -1.4f), new Vector3(0.12f, 0.9f, 0.12f), Materials["line_ceramic"], UnderstoryEditabilityClass.StoryGuide, "Field tool placeholder. Name intentionally not final.", true);
            CreateRotatedPrimitive(PrimitiveType.Cube, "Steward_FieldTool_HammerHead", characters, new Vector3(-0.85f, 2.25f, -1.4f), new Vector3(0.52f, 0.18f, 0.18f), Quaternion.Euler(0f, 0f, 9f), Materials["worn_metal"], UnderstoryEditabilityClass.StoryGuide, "Survey/repair field tool head; not a weapon.", false);

            CreateCapsule("WorkerPlaceholder_ShallowCut_A", characters, new Vector3(6.8f, 1.05f, -3.0f), 0.42f, 1.55f, Materials["worker_cloth"], UnderstoryEditabilityClass.StoryGuide, "Worker placeholder at shallow material cut.");
            CreatePrimitive(PrimitiveType.Sphere, "WorkerA_Head_WarmFace", characters, new Vector3(6.8f, 1.9f, -3.0f), new Vector3(0.31f, 0.3f, 0.31f), Materials["warm_plaster"], UnderstoryEditabilityClass.StoryGuide, "Readable worker head placeholder.", false);
            CreatePrimitive(PrimitiveType.Cube, "WorkerA_Helmet_RoofTile", characters, new Vector3(6.8f, 2.2f, -3.0f), new Vector3(0.62f, 0.18f, 0.5f), Materials["roof_tile"], UnderstoryEditabilityClass.StoryGuide, "Small worker helmet material read.", false);
            CreatePrimitive(PrimitiveType.Cube, "WorkerA_PickHandle_Timber", characters, new Vector3(7.25f, 1.55f, -3.2f), new Vector3(0.12f, 0.92f, 0.12f), Materials["timber_brace"], UnderstoryEditabilityClass.StoryGuide, "Worker extraction tool handle.", false);
            CreatePrimitive(PrimitiveType.Cube, "WorkerA_PickHead_Metal", characters, new Vector3(7.25f, 2.03f, -3.2f), new Vector3(0.54f, 0.12f, 0.16f), Materials["worn_metal"], UnderstoryEditabilityClass.StoryGuide, "Worker extraction tool head.", false);
            CreateCapsule("WorkerPlaceholder_ShallowCut_B", characters, new Vector3(3.3f, 1.05f, -3.25f), 0.42f, 1.55f, Materials["worker_cloth"], UnderstoryEditabilityClass.StoryGuide, "Worker placeholder calling the Steward to the hatch.");
            CreatePrimitive(PrimitiveType.Sphere, "WorkerB_Head_WarmFace", characters, new Vector3(3.3f, 1.9f, -3.25f), new Vector3(0.31f, 0.3f, 0.31f), Materials["warm_plaster"], UnderstoryEditabilityClass.StoryGuide, "Readable worker head placeholder.", false);
            CreatePrimitive(PrimitiveType.Cube, "WorkerB_Helmet_RoofTile", characters, new Vector3(3.3f, 2.2f, -3.25f), new Vector3(0.62f, 0.18f, 0.5f), Materials["roof_tile"], UnderstoryEditabilityClass.StoryGuide, "Small worker helmet material read.", false);
            CreatePrimitive(PrimitiveType.Cube, "WorkerB_SampleSatchel_Cloth", characters, new Vector3(3.68f, 1.2f, -3.05f), new Vector3(0.32f, 0.42f, 0.16f), Materials["steward_cloth"], UnderstoryEditabilityClass.StoryGuide, "Sample satchel carrying scarce material.", false);
            CreatePrimitive(PrimitiveType.Cube, "WorkerTool_TimberBraceBundle", characters, new Vector3(6.6f, 0.85f, -4.1f), new Vector3(1.2f, 0.18f, 0.22f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Readable worker prop: timber braces for shoring.", false);

            AddLabel(characters, "Label_VisibleSteward", "Visible Summit Steward + worker silhouettes", new Vector3(0f, 3.6f, -3.4f), 0.28f);
            characters.gameObject.SetActive(false);
        }

        private static void BuildReturnAndRefineStubs(Transform surface)
        {
            var loop = CreateGroup("LoopStubs_HaulRefineRepair", surface);
            CreatePrimitive(PrimitiveType.Cube, "ReturnRitualStub_HaulTable", loop, new Vector3(1.3f, 0.85f, -7.8f), new Vector3(2.8f, 0.32f, 1.15f), Materials["timber_brace"], UnderstoryEditabilityClass.StoryGuide, "Return Ritual v1 placeholder: material physically returns here.", true);
            CreatePrimitive(PrimitiveType.Cube, "RefinerStub_FirstKiln", loop, new Vector3(4.9f, 0.95f, -6.7f), new Vector3(1.2f, 1.05f, 1.2f), Materials["fired_brick"], UnderstoryEditabilityClass.StoryGuide, "First kiln/refiner placeholder for later loop proof.", true);
            var roughHaul = CreatePrimitive(PrimitiveType.Sphere, "MaterialHaul_RoughStone", loop, new Vector3(0.7f, 1.22f, -7.75f), new Vector3(0.34f, 0.34f, 0.34f), Materials["rough_stone"], UnderstoryEditabilityClass.StoryGuide, "Physical haul item placeholder.", false);
            roughHaul.SetActive(false);
            var clayHaul = CreatePrimitive(PrimitiveType.Sphere, "MaterialHaul_Clay", loop, new Vector3(1.35f, 1.22f, -7.75f), new Vector3(0.34f, 0.34f, 0.34f), Materials["clay_cut"], UnderstoryEditabilityClass.StoryGuide, "Physical haul item placeholder.", false);
            clayHaul.SetActive(false);
            var dressedStone = CreatePrimitive(PrimitiveType.Cube, "BuildMaterial_DressedStone", loop, new Vector3(4.15f, 1.68f, -6.2f), new Vector3(0.48f, 0.22f, 0.34f), Materials["rough_stone"], UnderstoryEditabilityClass.StoryGuide, "Refined rough stone ready for repair/build.", false);
            dressedStone.SetActive(false);
            var seedclay = CreatePrimitive(PrimitiveType.Sphere, "BuildMaterial_Seedclay", loop, new Vector3(4.9f, 1.7f, -6.15f), new Vector3(0.28f, 0.28f, 0.28f), Materials["seed_soil_viable"], UnderstoryEditabilityClass.StoryGuide, "Viable seedclay ready for garden repair.", false);
            seedclay.SetActive(false);
            var firedBrick = CreatePrimitive(PrimitiveType.Cube, "BuildMaterial_FiredBrick", loop, new Vector3(5.55f, 1.68f, -6.2f), new Vector3(0.48f, 0.22f, 0.34f), Materials["fired_brick"], UnderstoryEditabilityClass.StoryGuide, "Fired brick ready for shelter repair.", false);
            firedBrick.SetActive(false);
            var buildBlock = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_Block_01", loop, new Vector3(-1.1f, 0.8f, -3.7f), new Vector3(0.8f, 0.8f, 0.8f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Player-built support block for free-build placement/removal proof.", true);
            buildBlock.SetActive(false);
            AddLabel(loop, "Label_ReturnRefineStub", "Return Ritual + kiln + first committed draft", new Vector3(3.2f, 2.8f, -7.6f), 0.24f);
            loop.gameObject.SetActive(false);
        }

        private static void BuildShaftheadProgress(Transform surface)
        {
            var progress = CreateGroup("ShaftheadProgress_CoreSampleArchive", surface);

            CreatePrimitive(PrimitiveType.Cylinder, "CoreSample_GlassColumn", progress, new Vector3(8.6f, 1.25f, -6.4f), new Vector3(0.45f, 1.8f, 0.45f), Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Core Sample column: physical progress object at the shafthead.", true);
            var firstBand = CreatePrimitive(PrimitiveType.Cylinder, "CoreSampleBand_FirstDescent", progress, new Vector3(8.6f, 1.05f, -6.4f), new Vector3(0.5f, 0.16f, 0.5f), Materials["clay_cut"], UnderstoryEditabilityClass.StoryGuide, "First Core Sample band from the Bore Room seam.", true);
            firstBand.SetActive(false);

            CreatePrimitive(PrimitiveType.Cube, "ArchiveShelf_Phase0", progress, new Vector3(10.4f, 0.88f, -5.7f), new Vector3(1.85f, 0.24f, 0.75f), Materials["timber_brace"], UnderstoryEditabilityClass.ProtectedLandmark, "Phase 0 Archive shelf: Singulars and fragments live as objects, not a lore dump.", true);
            var archiveItem = CreatePrimitive(PrimitiveType.Sphere, "ArchiveSingular_FirstSeam", progress, new Vector3(10.4f, 1.22f, -5.7f), new Vector3(0.28f, 0.28f, 0.28f), Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "First Archive seed from careful seam or scarred bulk fragment.", true);
            archiveItem.SetActive(false);

            var wantList = CreateGroup("WantList_EastWallDraft", progress);
            wantList.localPosition = new Vector3(-2.8f, 1.15f, -8.35f);
            for (var i = 0; i < 5; i++)
                CreatePrimitive(PrimitiveType.Cube, $"WantList_GhostBlock_{i:00}", wantList, new Vector3(i * 0.62f, 0f, 0f), new Vector3(0.54f, 0.42f, 0.54f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Free draft ghost block: visible want-list pressure for the next dig.", true);
            AddLabel(wantList, "Label_WantList", "Draft wants 40 more dressed stone", new Vector3(1.25f, 0.9f, 0f), 0.16f);
            wantList.gameObject.SetActive(false);

            CreatePrimitive(PrimitiveType.Cube, "FarGlass_BlockedByVeil", progress, new Vector3(11.7f, 1.45f, -8.1f), new Vector3(0.32f, 1.35f, 0.32f), Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "Far-Glass seed: visible, still blocked by the Veil in Phase 0.", true);
            AddLabel(progress, "Label_CoreArchive", "Core Sample + Archive shelf make progress physical", new Vector3(9.7f, 3.25f, -6.6f), 0.23f);
            progress.gameObject.SetActive(false);
        }

        private static void BuildSurfaceMaterialScatter(Transform surface)
        {
            var scatter = CreateGroup("Surface_TactileMaterialScatter", surface);
            CreatePrimitive(PrimitiveType.Cube, "Surface_MossCarpet_RefugeWorkyard", scatter, new Vector3(-5.35f, 0.58f, 0.12f), new Vector3(7.4f, 0.05f, 4.65f), Materials["moss_grass"], UnderstoryEditabilityClass.PlayerBuilt, "Mossy workyard patch around the summit refuge for real-material miniature read.", false);
            CreatePrimitive(PrimitiveType.Cube, "Surface_MossCarpet_TinyGardenEdge", scatter, new Vector3(-8.7f, 0.6f, -5.78f), new Vector3(3.6f, 0.05f, 1.8f), Materials["moss_grass"], UnderstoryEditabilityClass.PlayerBuilt, "Green edge around first tiny garden repair target.", false);

            for (var i = 0; i < 8; i++)
            {
                CreateRotatedPrimitive(PrimitiveType.Cube, $"SurfaceStonePath_RefugeToHatch_{i:00}", scatter, new Vector3(-2.9f + i * 0.8f, 0.64f, -1.78f + (i % 2) * 0.12f), new Vector3(0.58f, 0.08f, 0.46f), Quaternion.Euler(0f, i * 8f, 0f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Worn stone path from refuge toward the hidden hatch.", false);
            }

            CreateGrassTufts(scatter, "SurfaceGrass_AroundRefuge", new Vector3(-5.6f, 0.58f, -0.5f), 28, 7.2f, 4.4f);
            CreateGrassTufts(scatter, "SurfaceGrass_TerraceEdge", new Vector3(-2.2f, 0.98f, -6.1f), 18, 7.4f, 0.65f);

            for (var i = 0; i < 24; i++)
            {
                var seed = StableSeed("surface_pebbles");
                var x = -11.2f + Hash01(seed, i, 3) * 22f;
                var z = -7.8f + Hash01(seed, i, 7) * 9.5f;
                var y = 0.55f + Hash01(seed, i, 11) * 0.15f;
                var scale = 0.12f + Hash01(seed, i, 13) * 0.18f;
                var material = Hash01(seed, i, 17) > 0.58f ? Materials["clay_cut"] : Materials["rough_stone"];
                CreateRotatedPrimitive(PrimitiveType.Cube, $"SurfaceLooseMaterial_Pebble_{i:00}", scatter, new Vector3(x, y, z), new Vector3(scale * 1.4f, scale * 0.65f, scale), Quaternion.Euler(0f, Hash01(seed, i, 19) * 180f, Hash01(seed, i, 23) * 9f), material, UnderstoryEditabilityClass.ExtractionVolume, "Loose gathered material read; small tactile scatter for scale.", false);
            }

            scatter.gameObject.SetActive(false);
        }

        private static void BuildKayKitSurfaceHero(Transform surface)
        {
            var hero = CreateGroup("KayKitHero_SurfaceRefuge_MaterialMiniature", surface);
            hero.localPosition = new Vector3(-5.8f, 0.64f, 0.2f);
            AddMarker(hero.gameObject, UnderstoryEditabilityClass.StoryGuide, "Imported CC0 KayKit hero art layer for the Scene 01 real-material miniature pass.", true);

            var tileRows = new[]
            {
                new[] { "square_rock_roadA_detail", "square_forest_roadA_detail", "square_forest_detail" },
                new[] { "square_rock_detail", "square_forest_detail", "square_forest_roadC_detail" },
                new[] { "square_rock_waterStraight", "square_forest_waterStraight", "square_rock_detail" }
            };

            for (var z = 0; z < tileRows.Length; z++)
            {
                for (var x = 0; x < tileRows[z].Length; x++)
                {
                    var id = tileRows[z][x];
                    CreateImportedModel(
                        $"{KayKitTileFolder}/{id}.fbx",
                        $"KayKit_SurfaceTile_{id}_{x}_{z}",
                        hero,
                        new Vector3((x - 1) * 2.05f, -0.08f, (z - 1) * 2.05f),
                        Vector3.one * 0.92f,
                        Quaternion.Euler(0f, (x + z) % 2 == 0 ? 0f : 90f, 0f),
                        UnderstoryEditabilityClass.PlayerBuilt,
                        "KayKit terrain tile composing the surface hero workyard.",
                        false);
                }
            }

            CreateImportedModel($"{KayKitObjectFolder}/house.fbx", "KayKit_SummitRefuge_House", hero, new Vector3(-0.78f, 0.02f, 0.28f), Vector3.one * 2.38f, Quaternion.Euler(0f, 155f, 0f), UnderstoryEditabilityClass.RepairAnchor, "Hero summit refuge: imported CC0 medieval miniature house.", true);
            CreateImportedModel($"{KayKitObjectFolder}/mine.fbx", "KayKit_HiddenHatch_MineMouth", hero, new Vector3(2.48f, 0.03f, -1.05f), Vector3.one * 1.62f, Quaternion.Euler(0f, -48f, 0f), UnderstoryEditabilityClass.ProtectedLandmark, "Imported mine mouth repurposed as hidden hatch discovery shelter.", true);
            CreateImportedModel($"{KayKitObjectFolder}/well.fbx", "KayKit_CoreSample_WellLikeShafthead", hero, new Vector3(1.88f, 0.04f, 1.42f), Vector3.one * 1.08f, Quaternion.Euler(0f, 22f, 0f), UnderstoryEditabilityClass.ProtectedLandmark, "Small shafthead object reinforcing Core Sample / Archive physicality.", false);
            CreateImportedModel($"{KayKitObjectFolder}/farm_plot.fbx", "KayKit_FirstGarden_FarmPlot", hero, new Vector3(-2.82f, 0.04f, -1.48f), Vector3.one * 1.22f, Quaternion.Euler(0f, 15f, 0f), UnderstoryEditabilityClass.RepairAnchor, "Imported farm plot for tiny garden repair target.", true);
            CreateImportedModel($"{KayKitObjectFolder}/bridge_roofed.fbx", "KayKit_ReturnPath_RoofedBridge", hero, new Vector3(-2.62f, 0.05f, 1.46f), Vector3.one * 1.02f, Quaternion.Euler(0f, 68f, 0f), UnderstoryEditabilityClass.PlayerBuilt, "Small roofed bridge / haul path material read.", false);

            CreateImportedModel($"{KayKitObjectFolder}/detail_treeA.fbx", "KayKit_Tree_A", hero, new Vector3(-3.18f, 0.05f, 0.52f), Vector3.one * 1.05f, Quaternion.Euler(0f, 22f, 0f), UnderstoryEditabilityClass.PlayerBuilt, "Hardy summit tree set dressing.", false);
            CreateImportedModel($"{KayKitObjectFolder}/detail_treeB.fbx", "KayKit_Tree_B", hero, new Vector3(2.55f, 0.05f, -2.18f), Vector3.one * 0.98f, Quaternion.Euler(0f, -40f, 0f), UnderstoryEditabilityClass.PlayerBuilt, "Hardy summit tree set dressing.", false);
            CreateImportedModel($"{KayKitObjectFolder}/detail_treeC.fbx", "KayKit_Tree_C", hero, new Vector3(-0.25f, 0.05f, -2.28f), Vector3.one * 0.82f, Quaternion.Euler(0f, 70f, 0f), UnderstoryEditabilityClass.PlayerBuilt, "Hardy summit tree set dressing.", false);

            CreateImportedModel($"{KayKitObjectFolder}/detail_rocks.fbx", "KayKit_Rocks_HatchScar", hero, new Vector3(2.25f, 0.05f, -0.08f), Vector3.one * 0.88f, Quaternion.Euler(0f, 33f, 0f), UnderstoryEditabilityClass.ExtractionVolume, "Imported rocks around the hidden hatch cut.", false);
            CreateImportedModel($"{KayKitObjectFolder}/detail_rocks_small.fbx", "KayKit_Rocks_ReturnTable", hero, new Vector3(-1.72f, 0.05f, -2.0f), Vector3.one * 0.82f, Quaternion.Euler(0f, -17f, 0f), UnderstoryEditabilityClass.ExtractionVolume, "Loose material rocks for return/refine read.", false);
            CreateImportedModel($"{KayKitObjectFolder}/wall_straight.fbx", "KayKit_Windbreak_WallStraight", hero, new Vector3(-3.45f, 0.06f, 2.02f), Vector3.one * 0.9f, Quaternion.Euler(0f, 18f, 0f), UnderstoryEditabilityClass.RepairAnchor, "Imported wall piece representing first shelter windbreak repair.", true);
            CreateImportedModel($"{KayKitObjectFolder}/wall_corner.fbx", "KayKit_Terrace_WallCorner", hero, new Vector3(1.35f, 0.06f, 2.1f), Vector3.one * 0.86f, Quaternion.Euler(0f, 122f, 0f), UnderstoryEditabilityClass.RepairAnchor, "Imported corner wall piece representing terrace repair.", true);

            CreatePrimitive(PrimitiveType.Cylinder, "KayKit_HatchMineralCrust_Hero", hero, new Vector3(2.36f, 0.24f, -1.0f), new Vector3(0.54f, 0.055f, 0.54f), Materials["filterstone"], UnderstoryEditabilityClass.DestroyableRuin, "Visible mineral crust over the imported hidden hatch mouth.", true);
            CreatePrimitive(PrimitiveType.Cube, "KayKit_GlassWaterCatch_Hero", hero, new Vector3(0.72f, 0.62f, 1.78f), new Vector3(1.05f, 0.08f, 0.74f), Materials["glass_pane"], UnderstoryEditabilityClass.PlayerBuilt, "Small glass catchment integrated with imported hero art.", false);
            CreatePrimitive(PrimitiveType.Sphere, "KayKit_WarmWindowLantern_Hero", hero, new Vector3(-1.34f, 1.88f, -0.34f), Vector3.one * 0.17f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm window lantern cue on imported refuge.", false);
            CreatePrimitive(PrimitiveType.Sphere, "KayKit_HatchGlow_Hero", hero, new Vector3(2.28f, 0.36f, -1.06f), Vector3.one * 0.18f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm glow cue for the hidden hatch discovery.", false);

            CreateHeroLoopProps(hero);
            CreateHeroCrew(hero);
            CreateHeroScatter(hero);
        }

        private static void CreateHeroLoopProps(Transform hero)
        {
            CreatePrimitive(PrimitiveType.Cube, "KayKit_ReturnTable_Hero", hero, new Vector3(0.35f, 0.45f, -2.9f), new Vector3(1.25f, 0.14f, 0.76f), Materials["timber_brace"], UnderstoryEditabilityClass.StoryGuide, "Visible Return Ritual haul table in the hero miniature.", true);
            CreatePrimitive(PrimitiveType.Cube, "KayKit_ReturnTable_Legs_A", hero, new Vector3(-0.12f, 0.24f, -2.62f), new Vector3(0.12f, 0.42f, 0.12f), Materials["timber_brace"], UnderstoryEditabilityClass.StoryGuide, "Return table timber leg.", false);
            CreatePrimitive(PrimitiveType.Cube, "KayKit_ReturnTable_Legs_B", hero, new Vector3(0.82f, 0.24f, -3.18f), new Vector3(0.12f, 0.42f, 0.12f), Materials["timber_brace"], UnderstoryEditabilityClass.StoryGuide, "Return table timber leg.", false);

            var haulStone = CreatePrimitive(PrimitiveType.Sphere, "KayKit_Haul_RoughStone_Hero", hero, new Vector3(0.02f, 0.68f, -2.93f), Vector3.one * 0.2f, Materials["rough_stone"], UnderstoryEditabilityClass.StoryGuide, "Returned rough stone haul visible on the hero table.", false);
            haulStone.SetActive(false);
            var haulClay = CreatePrimitive(PrimitiveType.Sphere, "KayKit_Haul_Clay_Hero", hero, new Vector3(0.58f, 0.68f, -2.88f), Vector3.one * 0.18f, Materials["clay_cut"], UnderstoryEditabilityClass.StoryGuide, "Returned clay haul visible on the hero table.", false);
            haulClay.SetActive(false);

            CreatePrimitive(PrimitiveType.Cylinder, "KayKit_FirstKiln_Hero", hero, new Vector3(1.72f, 0.52f, -2.95f), new Vector3(0.45f, 0.44f, 0.45f), Materials["fired_brick"], UnderstoryEditabilityClass.StoryGuide, "Visible first kiln/refiner in the hero miniature.", true);
            CreatePrimitive(PrimitiveType.Sphere, "KayKit_FirstKiln_Glow_Hero", hero, new Vector3(1.72f, 0.98f, -2.95f), Vector3.one * 0.18f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm kiln glow for the first refine step.", false);

            var dressedStone = CreatePrimitive(PrimitiveType.Cube, "KayKit_BuildMaterial_DressedStone_Hero", hero, new Vector3(1.22f, 1.03f, -2.55f), new Vector3(0.34f, 0.18f, 0.26f), Materials["rough_stone"], UnderstoryEditabilityClass.StoryGuide, "Refined dressed stone visible after the kiln step.", false);
            dressedStone.SetActive(false);
            var seedClay = CreatePrimitive(PrimitiveType.Sphere, "KayKit_BuildMaterial_Seedclay_Hero", hero, new Vector3(1.7f, 1.08f, -2.48f), Vector3.one * 0.15f, Materials["seed_soil_viable"], UnderstoryEditabilityClass.StoryGuide, "Refined viable seedclay visible after the kiln step.", false);
            seedClay.SetActive(false);
            var firedBrick = CreatePrimitive(PrimitiveType.Cube, "KayKit_BuildMaterial_FiredBrick_Hero", hero, new Vector3(2.16f, 1.03f, -2.55f), new Vector3(0.34f, 0.18f, 0.26f), Materials["fired_brick"], UnderstoryEditabilityClass.StoryGuide, "Refined fired brick visible after the kiln step.", false);
            firedBrick.SetActive(false);

            var buildPad = CreatePrimitive(PrimitiveType.Cube, "KayKit_SurfaceBuildPad_Hero", hero, new Vector3(0.6f, 0.19f, 3.05f), new Vector3(1.32f, 0.05f, 0.94f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Visible free-build pad in the miniature workyard.", true);
            buildPad.transform.localRotation = Quaternion.Euler(0f, -14f, 0f);
            var support = CreatePrimitive(PrimitiveType.Cube, "KayKit_PlayerSupportBlock_Hero", hero, new Vector3(0.58f, 0.58f, 3.04f), new Vector3(0.52f, 0.52f, 0.52f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Visible first placed support block in the hero miniature.", true);
            support.SetActive(false);
            var wantList = CreatePrimitive(PrimitiveType.Cube, "KayKit_DraftWantList_Hero", hero, new Vector3(1.32f, 0.62f, 3.28f), new Vector3(0.48f, 0.34f, 0.48f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Visible want-list ghost block that makes the next haul desirable.", true);
            wantList.SetActive(false);

            var shelterPatch = CreatePrimitive(PrimitiveType.Cube, "KayKit_RepairedShelterPatch_Hero", hero, new Vector3(-5.42f, 0.58f, 2.9f), new Vector3(0.82f, 0.62f, 0.22f), Materials["fired_brick"], UnderstoryEditabilityClass.PlayerBuilt, "Visible repaired shelter windbreak patch.", true);
            shelterPatch.transform.localRotation = Quaternion.Euler(0f, 18f, 0f);
            shelterPatch.SetActive(false);
            var terracePatch = CreatePrimitive(PrimitiveType.Cube, "KayKit_RepairedTerracePatch_Hero", hero, new Vector3(2.18f, 0.32f, 3.2f), new Vector3(1.08f, 0.22f, 0.48f), Materials["rough_stone"], UnderstoryEditabilityClass.PlayerBuilt, "Visible repaired terrace edge patch.", true);
            terracePatch.transform.localRotation = Quaternion.Euler(0f, 122f, 0f);
            terracePatch.SetActive(false);
            var gardenPatch = CreatePrimitive(PrimitiveType.Cube, "KayKit_ViableGarden_Repaired_Hero", hero, new Vector3(-4.02f, 0.18f, -2.24f), new Vector3(1.0f, 0.055f, 0.68f), Materials["seed_soil_viable"], UnderstoryEditabilityClass.PlayerBuilt, "Visible green garden repair overlay.", true);
            gardenPatch.SetActive(false);

            for (var i = 0; i < 5; i++)
            {
                var sprout = CreatePrimitive(PrimitiveType.Capsule, $"KayKit_GardenSprout_Hero_{i:00}", hero, new Vector3(-4.36f + i * 0.17f, 0.33f, -2.2f + (i % 2) * 0.16f), new Vector3(0.035f, 0.12f, 0.035f), Materials["seed_soil_viable"], UnderstoryEditabilityClass.PlayerBuilt, "Tiny visible sprout after garden repair.", false);
                sprout.SetActive(false);
            }

            CreatePrimitive(PrimitiveType.Cube, "KayKit_ArchiveShelf_Hero", hero, new Vector3(3.45f, 0.52f, 1.25f), new Vector3(0.95f, 0.16f, 0.46f), Materials["timber_brace"], UnderstoryEditabilityClass.ProtectedLandmark, "Visible Archive shelf in the miniature.", true);
            var archiveSeed = CreatePrimitive(PrimitiveType.Sphere, "KayKit_ArchiveSingular_FirstSeam_Hero", hero, new Vector3(3.45f, 0.78f, 1.25f), Vector3.one * 0.14f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Visible first Archive fragment.", true);
            archiveSeed.SetActive(false);
            var coreBand = CreatePrimitive(PrimitiveType.Cylinder, "KayKit_CoreSampleBand_FirstDescent_Hero", hero, new Vector3(1.88f, 0.74f, 1.42f), new Vector3(0.26f, 0.055f, 0.26f), Materials["clay_cut"], UnderstoryEditabilityClass.StoryGuide, "Visible first band added to the shafthead/Core Sample object.", true);
            coreBand.SetActive(false);
        }

        private static void CreateHeroCrew(Transform hero)
        {
            var crew = CreateGroup("KayKitHero_CrewVisibleMiniatures", hero);
            crew.localPosition = Vector3.zero;

            CreateHeroFigure(crew, "KayKitHero_StewardFigure", new Vector3(2.45f, 0.52f, -1.15f), 0.56f, Materials["steward_cloth"], Materials["roof_tile"], true);
            CreateHeroFigure(crew, "KayKitHero_WorkerFigure_A", new Vector3(3.18f, 0.46f, -2.1f), 0.44f, Materials["worker_cloth"], Materials["fired_brick"], false);
            CreateHeroFigure(crew, "KayKitHero_WorkerFigure_B", new Vector3(-2.55f, 0.46f, -1.25f), 0.42f, Materials["worker_cloth"], Materials["roof_tile"], false);

            CreateRotatedPrimitive(PrimitiveType.Cube, "KayKitHero_WorkerPick_Handle", crew, new Vector3(3.38f, 0.76f, -2.13f), new Vector3(0.035f, 0.42f, 0.035f), Quaternion.Euler(0f, 0f, -21f), Materials["timber_brace"], UnderstoryEditabilityClass.StoryGuide, "Miniature worker extraction tool handle.", false);
            CreateRotatedPrimitive(PrimitiveType.Cube, "KayKitHero_WorkerPick_Head", crew, new Vector3(3.46f, 0.98f, -2.14f), new Vector3(0.22f, 0.045f, 0.055f), Quaternion.Euler(0f, 0f, -21f), Materials["worn_metal"], UnderstoryEditabilityClass.StoryGuide, "Miniature worker extraction tool head.", false);
        }

        private static void CreateHeroFigure(Transform parent, string name, Vector3 position, float scale, Material cloth, Material cap, bool steward)
        {
            var figure = CreateGroup(name, parent);
            figure.localPosition = position;
            figure.localScale = Vector3.one * scale;
            AddMarker(figure.gameObject, UnderstoryEditabilityClass.StoryGuide, steward ? "Visible hero Steward miniature used by the runtime pathing." : "Visible worker miniature for the summit workyard.", steward);

            CreatePrimitive(PrimitiveType.Capsule, name + "_Body", figure, new Vector3(0f, 0.36f, 0f), new Vector3(0.18f, 0.32f, 0.18f), cloth, UnderstoryEditabilityClass.StoryGuide, "Rounded cloth body for a real-material miniature crew read.", false);
            CreatePrimitive(PrimitiveType.Sphere, name + "_Head", figure, new Vector3(0f, 0.86f, -0.02f), new Vector3(0.18f, 0.17f, 0.18f), Materials["warm_plaster"], UnderstoryEditabilityClass.StoryGuide, "Warm face/head mass for a readable crew silhouette.", false);
            CreatePrimitive(PrimitiveType.Cube, name + "_BrickCap", figure, new Vector3(0f, 1.02f, -0.02f), new Vector3(0.36f, 0.1f, 0.28f), cap, UnderstoryEditabilityClass.StoryGuide, "Brick/tile helmet cap matching the miniature material language.", false);
            CreatePrimitive(PrimitiveType.Cube, name + "_Scarf", figure, new Vector3(0f, 0.68f, -0.16f), new Vector3(0.34f, 0.07f, 0.08f), Materials["worker_cloth"], UnderstoryEditabilityClass.StoryGuide, "Tiny fabric scarf for tactile material variation.", false);
            CreatePrimitive(PrimitiveType.Sphere, name + "_Headlamp", figure, new Vector3(0f, 1.03f, -0.19f), Vector3.one * 0.065f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm headlamp point matching the reference character language.", false);
        }

        private static void CreateHeroScatter(Transform hero)
        {
            var scatter = CreateGroup("KayKitHero_SurfaceMaterialScatter", hero);
            scatter.localPosition = Vector3.zero;

            CreateGrassTufts(scatter, "KayKitHero_Grass_HouseEdge", new Vector3(-0.9f, 0.24f, 1.42f), 18, 2.6f, 0.72f);
            CreateGrassTufts(scatter, "KayKitHero_Grass_HatchEdge", new Vector3(2.14f, 0.24f, -0.25f), 14, 1.8f, 1.15f);
            CreateGrassTufts(scatter, "KayKitHero_Grass_GardenEdge", new Vector3(-2.58f, 0.22f, -1.32f), 12, 1.6f, 0.9f);

            var seed = StableSeed("kaykit_hero_scatter");
            for (var i = 0; i < 26; i++)
            {
                var x = -3.7f + Hash01(seed, i, 1) * 7.25f;
                var z = -2.35f + Hash01(seed, i, 2) * 4.9f;
                if (Mathf.Abs(x + 0.8f) < 0.8f && Mathf.Abs(z - 0.2f) < 0.9f)
                    continue;
                var scale = 0.055f + Hash01(seed, i, 3) * 0.08f;
                var material = Hash01(seed, i, 4) > 0.52f ? Materials["rough_stone"] : Materials["clay_cut"];
                CreateRotatedPrimitive(PrimitiveType.Cube, $"KayKitHero_Pebble_{i:00}", scatter, new Vector3(x, 0.22f, z), new Vector3(scale * 1.4f, scale * 0.55f, scale), Quaternion.Euler(0f, Hash01(seed, i, 5) * 180f, Hash01(seed, i, 6) * 9f), material, UnderstoryEditabilityClass.ExtractionVolume, "Small loose material pebble for tactile miniature scale.", false);
            }
        }

        private static void BuildBoreRoom(Transform root)
        {
            var bore = CreateGroup("02_BoreRoom_Blockout", root);
            bore.position = new Vector3(0f, -9.5f, 20f);

            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_BoreRoomContainment", bore, new Vector3(0f, -0.4f, 0f), new Vector3(13f, 0.5f, 10f), Materials["protected_zone"], UnderstoryEditabilityClass.ProtectedShell, "Authored chamber containment shell. Protected.", true);
            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_MainBoreShaft", bore, new Vector3(0f, -4.2f, 0f), new Vector3(3.2f, 7.5f, 3.2f), Materials["bore_dark"], UnderstoryEditabilityClass.ProtectedShell, "Main Bore shaft drops beyond view. Protected shell.", true);
            CreateRing("B_ProtectedLandmark_ProtectedShaftRim", bore, new Vector3(0f, 0.35f, 0f), 4.1f, 0.34f, Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Protected shaft rim. Not casually editable.");
            CreateRing("B_ProtectedLandmark_BrokenGuideRings", bore, new Vector3(0f, 1.2f, 0f), 5.4f, 0.22f, Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "Broken guide rings / lift rail read.");
            var brokenGuideRing = FindChild(bore, "B_ProtectedLandmark_BrokenGuideRings");
            if (brokenGuideRing != null)
                brokenGuideRing.gameObject.SetActive(false);

            var boreCacheGuide = CreatePrimitive(PrimitiveType.Cube, "E_ExtractionVolume_BoreMaterialCache", bore, new Vector3(-7.3f, 0.65f, 1.8f), new Vector3(3.1f, 1.15f, 2.3f), Materials["clay_cut"], UnderstoryEditabilityClass.ExtractionVolume, "First interior material cache / extraction volume.", true);
            boreCacheGuide.SetActive(false);
            var boreDebrisGuide = CreatePrimitive(PrimitiveType.Cube, "C_DestroyableRuin_EditableBoreDebris_A", bore, new Vector3(-6.1f, 1.35f, -2.1f), new Vector3(2.2f, 1.0f, 1.3f), Materials["rough_stone"], UnderstoryEditabilityClass.DestroyableRuin, "Editable debris field in Bore Room.", true);
            boreDebrisGuide.SetActive(false);
            var traceSeam = CreatePrimitive(PrimitiveType.Cube, "TraceSeam_Glow", bore, new Vector3(-7.3f, 1.35f, 3.05f), new Vector3(2.4f, 0.08f, 0.08f), Materials["lantern_glow"], UnderstoryEditabilityClass.ExtractionVolume, "Glowing seam target for the careful trace gesture.", true);
            traceSeam.SetActive(false);
            var blastDust = CreatePrimitive(PrimitiveType.Sphere, "BlastImpact_Dust", bore, new Vector3(-6.1f, 1.75f, -2.1f), new Vector3(1.3f, 0.65f, 1.3f), Materials["clay_cut"], UnderstoryEditabilityClass.StoryGuide, "Blast feedback cloud. Visible warning, not reward.", false);
            blastDust.SetActive(false);
            var collapseBurial = CreatePrimitive(PrimitiveType.Cube, "CollapseBurial_BoreDebris", bore, new Vector3(-3.8f, 1.05f, 0.4f), new Vector3(1.7f, 0.85f, 1.6f), Materials["rough_stone"], UnderstoryEditabilityClass.DestroyableRuin, "Blast burial detour: collapse buries, never deletes.", true);
            collapseBurial.SetActive(false);
            var boreShoringGuide = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuiltGhostDraft_BoreShoring", bore, new Vector3(-4.2f, 1.6f, -1.9f), new Vector3(0.35f, 2.5f, 0.35f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Ghost shoring preview for later extraction support.", true);
            boreShoringGuide.SetActive(false);
            var committedShoring = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_BoreShoring_Committed", bore, new Vector3(-4.2f, 1.6f, -1.9f), new Vector3(0.35f, 2.5f, 0.35f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Committed shoring that gates safe extraction.", true);
            committedShoring.SetActive(false);

            var worksHint = CreatePrimitive(PrimitiveType.Cube, "Works_MistEngine_ClearerHint_Inactive", bore, new Vector3(7.1f, 1.5f, 1.8f), new Vector3(2.2f, 2.4f, 1.2f), Materials["filterstone"], UnderstoryEditabilityClass.ProtectedLandmark, "Inactive Works / Mist Engine / Clearer hint. Practical machine, not magic.", true);
            worksHint.SetActive(false);
            var linesHint = CreatePrimitive(PrimitiveType.Cube, "TheLines_InactiveWallConduit_A", bore, new Vector3(4.1f, 2.0f, -4.9f), new Vector3(6.4f, 0.18f, 0.28f), Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "The Lines: inactive conduit hint.", true);
            linesHint.SetActive(false);
            var vaultHint = CreatePrimitive(PrimitiveType.Cube, "BlackVault_SealedHint", bore, new Vector3(8.6f, 1.35f, -2.9f), new Vector3(1.4f, 1.5f, 1.0f), Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Black Vault sealed hint. Seed only.", true);
            vaultHint.SetActive(false);
            var warningMark = CreatePrimitive(PrimitiveType.Cube, "HollowerWarningMark_OverOlderStone", bore, new Vector3(2.7f, 1.8f, -5.08f), new Vector3(1.0f, 0.75f, 0.08f), Materials["hollower_mark"], UnderstoryEditabilityClass.ProtectedLandmark, "Hollower warning mark painted over older stone.", true);
            warningMark.SetActive(false);

            var boreEditableOverlay = CreatePrimitive(PrimitiveType.Cube, "BoreRoom_BroadEditableZone_C_D_E", bore, new Vector3(-3.5f, 0.52f, 1.9f), new Vector3(8.5f, 0.07f, 5f), Materials["editable_zone"], UnderstoryEditabilityClass.StoryGuide, "Interior editable/debris/extraction area overlay.", true);
            boreEditableOverlay.SetActive(false);
            BuildBoreRoomDressings(bore);
            BuildKayKitBoreHero(bore);
            AddLabel(bore, "Label_BoreRoom", "Bore Room: built, old, deep, practical. Protected rim + editable debris/cache.", new Vector3(0f, 5.6f, -6.6f), 0.34f);
        }

        private static void BuildBoreRoomDressings(Transform bore)
        {
            var dress = CreateGroup("BoreRoom_RealMaterialDressings", bore);

            for (var i = 0; i < 28; i++)
            {
                var angle = i * Mathf.PI * 2f / 28f;
                var radius = 5.75f + ((i % 3) - 1) * 0.08f;
                var pos = new Vector3(Mathf.Cos(angle) * radius, 0.62f, Mathf.Sin(angle) * radius);
                var scale = new Vector3(0.72f, 0.18f, 0.42f);
                CreateRotatedPrimitive(PrimitiveType.Cube, $"BoreFloor_AncientPaver_{i:00}", dress, pos, scale, Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f), Materials[i % 4 == 0 ? "filterstone" : "rough_stone"], UnderstoryEditabilityClass.ProtectedLandmark, "Ancient paver ring around the protected shaft rim.", false);
            }

            for (var side = -1; side <= 1; side += 2)
            {
                for (var tier = 0; tier < 5; tier++)
                {
                    for (var i = 0; i < 7; i++)
                    {
                        var x = side * (3.7f + i * 0.64f);
                        var y = 0.8f + tier * 0.46f;
                        var z = -5.55f;
                        var offset = tier % 2 == 0 ? 0f : 0.27f;
                        CreatePrimitive(PrimitiveType.Cube, $"BoreBackWall_Block_{side}_{tier}_{i:00}", dress, new Vector3(x + side * offset, y, z), new Vector3(0.58f, 0.38f, 0.28f), Materials["rough_stone"], UnderstoryEditabilityClass.ProtectedShell, "Old built masonry: protected chamber shell.", false);
                    }
                }
            }

            for (var i = 0; i < 7; i++)
            {
                var x = -3f + i * 1f;
                CreatePrimitive(PrimitiveType.Cube, $"TheLines_CeramicSocket_{i:00}", dress, new Vector3(x, 2.05f, -5.18f), new Vector3(0.26f, 0.42f, 0.18f), Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "Ceramic socket in The Lines.", false);
                if (i % 2 == 0)
                    CreatePrimitive(PrimitiveType.Sphere, $"BoreLanternString_WarmPoint_{i:00}", dress, new Vector3(x, 2.42f, -4.88f), Vector3.one * 0.18f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm crew lantern along the Bore Room wall.", false);
            }

            CreatePrimitive(PrimitiveType.Cube, "BlackVault_SealedFrame_Top", dress, new Vector3(8.6f, 2.28f, -2.9f), new Vector3(1.78f, 0.22f, 1.16f), Materials["worn_metal"], UnderstoryEditabilityClass.ProtectedLandmark, "Worn metal frame around sealed Black Vault hint.", false);
            CreatePrimitive(PrimitiveType.Cube, "BlackVault_SealedFrame_Left", dress, new Vector3(7.72f, 1.35f, -2.9f), new Vector3(0.22f, 1.62f, 1.16f), Materials["worn_metal"], UnderstoryEditabilityClass.ProtectedLandmark, "Worn metal frame around sealed Black Vault hint.", false);
            CreatePrimitive(PrimitiveType.Cube, "BlackVault_SealedFrame_Right", dress, new Vector3(9.48f, 1.35f, -2.9f), new Vector3(0.22f, 1.62f, 1.16f), Materials["worn_metal"], UnderstoryEditabilityClass.ProtectedLandmark, "Worn metal frame around sealed Black Vault hint.", false);

            for (var i = 0; i < 12; i++)
            {
                var seed = StableSeed("bore_dust");
                var x = -8.5f + Hash01(seed, i, 1) * 17f;
                var y = 1.2f + Hash01(seed, i, 2) * 2.9f;
                var z = -3.4f + Hash01(seed, i, 3) * 7.2f;
                CreatePrimitive(PrimitiveType.Sphere, $"BoreDustMote_Glow_{i:00}", dress, new Vector3(x, y, z), Vector3.one * (0.055f + Hash01(seed, i, 4) * 0.075f), Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Tiny warm dust glint for chamber depth and scale.", false);
            }

            dress.gameObject.SetActive(false);
        }

        private static void BuildKayKitBoreHero(Transform bore)
        {
            var hero = CreateGroup("KayKitHero_BoreRoom_AncientWorks", bore);
            hero.localPosition = new Vector3(-1.1f, 0.18f, 0.65f);
            AddMarker(hero.gameObject, UnderstoryEditabilityClass.StoryGuide, "Imported CC0 KayKit pieces dressing the Bore Room into an old practical chamber.", true);

            CreateImportedModel($"{KayKitTileFolder}/square_rock_detail.fbx", "KayKit_BoreFloor_RockTile_A", hero, new Vector3(-1.45f, 0f, 0.2f), Vector3.one * 1.05f, Quaternion.Euler(0f, 0f, 0f), UnderstoryEditabilityClass.ProtectedShell, "Imported rock tile grounding the Bore Room floor.", false);
            CreateImportedModel($"{KayKitTileFolder}/square_rock_roadA_detail.fbx", "KayKit_BoreFloor_RoadTile_B", hero, new Vector3(0.75f, 0f, 0.25f), Vector3.one * 1.0f, Quaternion.Euler(0f, 90f, 0f), UnderstoryEditabilityClass.ProtectedShell, "Imported old road tile implying built access around the shaft.", false);
            CreateImportedModel($"{KayKitObjectFolder}/wall_straight.fbx", "KayKit_BoreWall_StoneRun_A", hero, new Vector3(-4.4f, 0.28f, -3.7f), Vector3.one * 0.82f, Quaternion.Euler(0f, 0f, 0f), UnderstoryEditabilityClass.ProtectedShell, "Imported stone wall run in the Bore Room.", false);
            CreateImportedModel($"{KayKitObjectFolder}/wall_straight.fbx", "KayKit_BoreWall_StoneRun_B", hero, new Vector3(-2.0f, 0.28f, -3.7f), Vector3.one * 0.82f, Quaternion.Euler(0f, 0f, 0f), UnderstoryEditabilityClass.ProtectedShell, "Imported stone wall run in the Bore Room.", false);
            CreateImportedModel($"{KayKitObjectFolder}/wall_corner.fbx", "KayKit_BoreWall_StoneCorner", hero, new Vector3(4.35f, 0.28f, -3.55f), Vector3.one * 0.75f, Quaternion.Euler(0f, 90f, 0f), UnderstoryEditabilityClass.ProtectedShell, "Imported stone corner supporting the old chamber read.", false);
            CreateImportedModel($"{KayKitObjectFolder}/mine.fbx", "KayKit_BoreMaterialFace_MineAsset", hero, new Vector3(-5.85f, 0.22f, 1.95f), Vector3.one * 0.88f, Quaternion.Euler(0f, 132f, 0f), UnderstoryEditabilityClass.ExtractionVolume, "Imported mine asset over the first material face.", true);
            CreateImportedModel($"{KayKitObjectFolder}/detail_rocks.fbx", "KayKit_BoreLooseRocks_A", hero, new Vector3(-3.4f, 0.14f, 2.35f), Vector3.one * 0.75f, Quaternion.Euler(0f, 24f, 0f), UnderstoryEditabilityClass.DestroyableRuin, "Imported loose rocks for editable Bore debris.", false);
            CreateImportedModel($"{KayKitObjectFolder}/bridge_roofed.fbx", "KayKit_BoreShoring_RoofedTimber", hero, new Vector3(-4.1f, 0.22f, -1.45f), Vector3.one * 0.56f, Quaternion.Euler(0f, -36f, 0f), UnderstoryEditabilityClass.PlayerBuilt, "Imported timber piece reinforcing shoring read.", false);

            var committedShoring = CreatePrimitive(PrimitiveType.Cube, "KayKit_BoreShoring_Committed_Hero", hero, new Vector3(-3.86f, 0.94f, -1.18f), new Vector3(0.18f, 1.42f, 0.18f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Visible committed Bore shoring after the player sets support.", true);
            committedShoring.transform.localRotation = Quaternion.Euler(0f, -36f, 7f);
            committedShoring.SetActive(false);
            var collapse = CreatePrimitive(PrimitiveType.Sphere, "KayKit_BoreCollapseBurial_Hero", hero, new Vector3(-2.4f, 0.66f, 1.9f), new Vector3(0.74f, 0.36f, 0.56f), Materials["clay_cut"], UnderstoryEditabilityClass.DestroyableRuin, "Visible blast burial pile that must be cleared after rough extraction.", true);
            collapse.SetActive(false);
            var seamGlow = CreatePrimitive(PrimitiveType.Cube, "KayKit_BoreTraceSeam_Hero", hero, new Vector3(-5.1f, 0.98f, 2.2f), new Vector3(1.25f, 0.05f, 0.08f), Materials["lantern_glow"], UnderstoryEditabilityClass.ExtractionVolume, "Visible hero seam for the careful trace gesture.", true);
            seamGlow.transform.localRotation = Quaternion.Euler(0f, 18f, 0f);
            seamGlow.SetActive(false);

            CreatePrimitive(PrimitiveType.Sphere, "KayKit_BoreLantern_Hero_A", hero, new Vector3(-3.6f, 1.55f, -2.8f), Vector3.one * 0.16f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm lantern cue on imported Bore Room art.", false);
            CreatePrimitive(PrimitiveType.Sphere, "KayKit_BoreLantern_Hero_B", hero, new Vector3(3.4f, 1.35f, -2.65f), Vector3.one * 0.14f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Warm lantern cue on imported Bore Room art.", false);
        }

        private static void BuildLegend(Transform root)
        {
            var legend = CreateGroup("00_EditabilityLegend_And_Guardrails", root);
            legend.position = new Vector3(-17f, 2.4f, 7f);

            AddLabel(legend, "Label_Guardrail_CanonLanguage", "v7 canon: The Works / Clearers / Mist Engines / The Lines / Black Vaults", Vector3.zero, 0.26f);
            AddLabel(legend, "Label_EditabilityClasses", "A Protected shell | B Protected landmark | C Destroyable ruin | D Player-built | E Extraction volume | F Repair anchor", new Vector3(0f, -0.8f, 0f), 0.22f);
            AddLabel(legend, "Label_PhaseScope", "Phase 0 first playable: no store, combat, multiplayer, water sim, or full procedural mountain", new Vector3(0f, -1.5f, 0f), 0.22f);
        }

        private static void BuildRuntimeFlow(Transform root)
        {
            root.gameObject.AddComponent<Scene01RuntimeController>();

            AddInteractable(root, "KayKit_Rocks_HatchScar", "surface_cut", "Shallow material cut", "Workers gather scarce material here.");
            AddInteractable(root, "KayKit_HatchMineralCrust_Hero", "hatch_crust", "Hatch crust", "Clear the crust to expose the hatch.");
            AddInteractable(root, "KayKit_HiddenHatch_MineMouth", "hatch", "Hidden hatch", "Open the hatch into the Bore Room.");
            AddInteractable(root, "KayKit_BoreShoring_RoofedTimber", "bore_shoring", "Bore shoring", "Place shoring before extraction.");
            AddInteractable(root, "KayKit_BoreMaterialFace_MineAsset", "trace_extract", "Trace extraction cache", "Carefully trace the material face.");
            AddInteractable(root, "KayKit_BoreLooseRocks_A", "blast_extract", "Blast extraction debris", "Rough blast option for the material face.");
            AddInteractable(root, "KayKit_BoreCollapseBurial_Hero", "reexcavate_burial", "Buried return path", "Clear the blast burial; collapse buries, never deletes.");
            AddInteractable(root, "KayKit_ReturnTable_Hero", "haul_table", "Haul table", "Return material physically to the summit.");
            AddInteractable(root, "KayKit_FirstKiln_Hero", "kiln", "First kiln", "Refine raw haul into buildable material.");
            AddInteractable(root, "KayKit_CoreSample_WellLikeShafthead", "core_sample", "Core Sample", "Add the first stratum band.");
            AddInteractable(root, "KayKit_ArchiveShelf_Hero", "archive_shelf", "Archive shelf", "Place the first fragment as an object.");
            AddInteractable(root, "KayKit_Windbreak_WallStraight", "repair_shelter", "Shelter windbreak", "Consumes brick and timber.");
            AddInteractable(root, "KayKit_Terrace_WallCorner", "repair_terrace", "Broken terrace edge", "Consumes rough stone.");
            AddInteractable(root, "KayKit_FirstGarden_FarmPlot", "repair_garden", "Tiny garden bed", "Consumes viable seedclay.");
            AddInteractable(root, "KayKit_SurfaceBuildPad_Hero", "surface_build_zone", "Surface build zone", "Place the first support block.");
            AddInteractable(root, "KayKit_PlayerSupportBlock_Hero", "player_build_block", "Player support block", "Remove or replace the proof block.");

            var nodes = CreateGroup("Scene01_RuntimeNodes", root);
            CreateNode("Scene01Node_RepairCluster", nodes, new Vector3(-8.85f, 1.2f, -1.3f));
            CreateNode("Scene01Node_ShallowCut", nodes, new Vector3(-3.55f, 1.05f, -0.36f));
            CreateNode("Scene01Node_Hatch", nodes, new Vector3(-3.38f, 1.1f, -0.95f));
            CreateNode("Scene01Node_BoreInspection", nodes, new Vector3(-4.2f, -8.2f, 18.1f));
            CreateNode("Scene01Node_HaulTable", nodes, new Vector3(-5.45f, 1.25f, -2.68f));
            CreateNode("Scene01Node_Kiln", nodes, new Vector3(-4.05f, 1.45f, -2.74f));
            CreateNode("Scene01Node_BuildZone", nodes, new Vector3(-5.2f, 1.15f, 3.15f));
            CreateNode("Scene01Node_CoreSample", nodes, new Vector3(-3.92f, 1.1f, 1.62f));
            CreateNode("Scene01Node_ArchiveShelf", nodes, new Vector3(-2.3f, 1.1f, 1.5f));
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            var group = new GameObject(name);
            group.transform.SetParent(parent);
            group.transform.localPosition = Vector3.zero;
            group.transform.localRotation = Quaternion.identity;
            group.transform.localScale = Vector3.one;
            AddMarker(group, UnderstoryEditabilityClass.StoryGuide, $"Scene group: {name}.", false);
            return group.transform;
        }

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Material material, UnderstoryEditabilityClass editabilityClass, string role, bool required)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            AddMarker(go, editabilityClass, role, required);
            return go;
        }

        private static GameObject CreateImportedModel(string assetPath, string name, Transform parent, Vector3 position, Vector3 scale, Quaternion rotation, UnderstoryEditabilityClass editabilityClass, string role, bool required)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null && File.Exists(assetPath))
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            }

            if (prefab == null)
            {
                return CreatePrimitive(
                    PrimitiveType.Cube,
                    name + "_MissingAssetFallback",
                    parent,
                    position,
                    scale,
                    Materials["hollower_mark"],
                    editabilityClass,
                    role + " Missing imported asset: " + assetPath,
                    required);
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (instance == null)
            {
                instance = UnityEngine.Object.Instantiate(prefab, parent);
            }

            instance.name = name;
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            instance.transform.localScale = scale;
            AddMarker(instance, editabilityClass, role, required);
            BeautifyImportedRenderers(instance, assetPath, name);
            EnsureAggregateCollider(instance);
            return instance;
        }

        private static void BeautifyImportedRenderers(GameObject instance, string assetPath, string instanceName)
        {
            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.receiveShadows = true;

                var sharedMaterials = renderer.sharedMaterials;
                for (var i = 0; i < sharedMaterials.Length; i++)
                    sharedMaterials[i] = ConvertImportedMaterial(sharedMaterials[i], assetPath, instanceName);
                renderer.sharedMaterials = sharedMaterials;
            }
        }

        private static Material ConvertImportedMaterial(Material source, string assetPath, string instanceName)
        {
            if (source == null)
                return GuessFallbackMaterial(assetPath, instanceName);

            var mapped = MapKayKitMaterial(source.name, assetPath, instanceName);
            if (mapped != null)
                return mapped;

            var color = ExtractMaterialColor(source);
            var key = SanitizeAssetName($"{source.name}_{ColorUtility.ToHtmlStringRGBA(color)}");
            if (ImportedMaterialCache.TryGetValue(key, out var cached) && cached != null)
                return cached;

            var path = $"{ImportedMaterialFolder}/KayKit_{key}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.name = $"KayKit_{key}";
            material.color = color;
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", 0.18f);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", 0f);
            EditorUtility.SetDirty(material);
            ImportedMaterialCache[key] = material;
            return material;
        }

        private static Material MapKayKitMaterial(string sourceName, string assetPath, string instanceName)
        {
            var haystack = $"{sourceName} {assetPath} {instanceName}".ToLowerInvariant();
            if (haystack.Contains("water"))
                return Materials["water_cold"];
            if (haystack.Contains("green"))
                return GetImportedFlatMaterial("moss_green", new Color(0.25f, 0.34f, 0.17f));
            if (haystack.Contains("yellow"))
                return GetImportedFlatMaterial("muted_clay_path", new Color(0.43f, 0.27f, 0.17f));
            if (haystack.Contains("beige") || haystack.Contains("white"))
                return GetImportedFlatMaterial("warm_plaster", new Color(0.64f, 0.55f, 0.42f));
            if (haystack.Contains("wood") || haystack.Contains("brown"))
                return haystack.Contains("house")
                    ? GetImportedFlatMaterial("fired_roof_tile", new Color(0.48f, 0.17f, 0.1f))
                    : GetImportedFlatMaterial("weathered_timber", new Color(0.34f, 0.2f, 0.12f));
            if (haystack.Contains("metal"))
                return GetImportedFlatMaterial("worn_metal", new Color(0.46f, 0.43f, 0.38f), 0.42f, 0.35f);
            if (haystack.Contains("black"))
                return GetImportedFlatMaterial("blackglass", new Color(0.035f, 0.045f, 0.05f), 0.62f, 0.08f);
            if (haystack.Contains("stone"))
                return GetImportedFlatMaterial("rough_stone", new Color(0.34f, 0.32f, 0.28f));
            return null;
        }

        private static Material GetImportedFlatMaterial(string id, Color color, float smoothness = 0.14f, float metallic = 0f)
        {
            var key = "flat_" + SanitizeAssetName(id);
            if (ImportedMaterialCache.TryGetValue(key, out var cached) && cached != null)
                return cached;

            var path = $"{ImportedMaterialFolder}/KayKit_{key}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.name = $"KayKit_{key}";
            material.color = color;
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Metallic"))
                material.SetFloat("_Metallic", metallic);
            EditorUtility.SetDirty(material);
            ImportedMaterialCache[key] = material;
            return material;
        }

        private static Color ExtractMaterialColor(Material material)
        {
            if (material.HasProperty("_BaseColor"))
                return material.GetColor("_BaseColor");
            if (material.HasProperty("_Color"))
                return material.GetColor("_Color");
            return new Color(0.54f, 0.47f, 0.38f);
        }

        private static Material GuessFallbackMaterial(string assetPath, string instanceName)
        {
            var haystack = $"{assetPath} {instanceName}".ToLowerInvariant();
            if (haystack.Contains("house") || haystack.Contains("wall"))
                return Materials["warm_plaster"];
            if (haystack.Contains("tree") || haystack.Contains("forest") || haystack.Contains("farm"))
                return Materials["moss_grass"];
            if (haystack.Contains("mine") || haystack.Contains("rock"))
                return Materials["rough_stone"];
            if (haystack.Contains("bridge"))
                return Materials["timber_brace"];
            return Materials["rough_stone"];
        }

        private static string SanitizeAssetName(string value)
        {
            var chars = value.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (!char.IsLetterOrDigit(chars[i]) && chars[i] != '_' && chars[i] != '-')
                    chars[i] = '_';
            }
            var sanitized = new string(chars).Trim('_');
            return sanitized.Length > 80 ? sanitized.Substring(0, 80) : sanitized;
        }

        private static void EnsureAggregateCollider(GameObject instance)
        {
            if (instance.GetComponentsInChildren<Collider>(true).Length > 0)
                return;

            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return;

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            var localBounds = new Bounds(instance.transform.InverseTransformPoint(bounds.center), Vector3.zero);
            foreach (var corner in GetBoundsCorners(bounds))
                localBounds.Encapsulate(instance.transform.InverseTransformPoint(corner));

            var box = instance.AddComponent<BoxCollider>();
            box.center = localBounds.center;
            box.size = new Vector3(Mathf.Max(0.12f, localBounds.size.x), Mathf.Max(0.12f, localBounds.size.y), Mathf.Max(0.12f, localBounds.size.z));
        }

        private static IEnumerable<Vector3> GetBoundsCorners(Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;
            yield return new Vector3(min.x, min.y, min.z);
            yield return new Vector3(min.x, min.y, max.z);
            yield return new Vector3(min.x, max.y, min.z);
            yield return new Vector3(min.x, max.y, max.z);
            yield return new Vector3(max.x, min.y, min.z);
            yield return new Vector3(max.x, min.y, max.z);
            yield return new Vector3(max.x, max.y, min.z);
            yield return new Vector3(max.x, max.y, max.z);
        }

        private static GameObject CreateRotatedPrimitive(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Quaternion rotation, Material material, UnderstoryEditabilityClass editabilityClass, string role, bool required)
        {
            var go = CreatePrimitive(type, name, parent, position, scale, material, editabilityClass, role, required);
            go.transform.localRotation = rotation;
            return go;
        }

        private static void CreateCapsule(string name, Transform parent, Vector3 position, float radius, float height, Material material, UnderstoryEditabilityClass editabilityClass, string role)
        {
            var capsule = CreatePrimitive(PrimitiveType.Capsule, name, parent, position, new Vector3(radius, height * 0.5f, radius), material, editabilityClass, role, true);
            var lantern = CreatePrimitive(PrimitiveType.Sphere, name + "_Lantern", parent, position + new Vector3(0.38f, -0.05f, 0.18f), Vector3.one * 0.18f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Small lantern/readability marker.", false);
            lantern.transform.SetParent(capsule.transform, true);
        }

        private static void CreateShrub(Transform parent, string prefix, Vector3 position, float size)
        {
            CreatePrimitive(PrimitiveType.Cylinder, prefix + "_Trunk", parent, position + new Vector3(0f, size * 0.14f, 0f), new Vector3(size * 0.1f, size * 0.28f, size * 0.1f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Tiny hardy summit shrub trunk.", false);
            for (var i = 0; i < 5; i++)
            {
                var angle = i * Mathf.PI * 2f / 5f;
                var offset = new Vector3(Mathf.Cos(angle) * size * 0.18f, size * (0.42f + (i % 2) * 0.08f), Mathf.Sin(angle) * size * 0.16f);
                var leafScale = Vector3.one * size * (0.28f + (i % 3) * 0.03f);
                CreatePrimitive(PrimitiveType.Sphere, $"{prefix}_LeafMass_{i:00}", parent, position + offset, leafScale, Materials["moss_grass"], UnderstoryEditabilityClass.PlayerBuilt, "Rounded hardy summit shrub leaf mass.", false);
            }
        }

        private static void CreateGrassTufts(Transform parent, string prefix, Vector3 center, int count, float width, float depth)
        {
            var seed = StableSeed(prefix);
            for (var i = 0; i < count; i++)
            {
                var x = center.x + (Hash01(seed, i, 5) - 0.5f) * width;
                var z = center.z + (Hash01(seed, i, 9) - 0.5f) * depth;
                var height = 0.18f + Hash01(seed, i, 13) * 0.25f;
                var yaw = Hash01(seed, i, 17) * 180f;
                CreateRotatedPrimitive(PrimitiveType.Cube, $"{prefix}_Tuft_{i:00}", parent, new Vector3(x, center.y + height * 0.5f, z), new Vector3(0.045f, height, 0.065f), Quaternion.Euler(Hash01(seed, i, 19) * 10f, yaw, Hash01(seed, i, 23) * 8f), Materials["moss_grass"], UnderstoryEditabilityClass.PlayerBuilt, "Tiny grass blade tuft for real-material miniature scale.", false);
            }
        }

        private static void CreateRing(string name, Transform parent, Vector3 center, float radius, float segmentThickness, Material material, UnderstoryEditabilityClass editabilityClass, string role)
        {
            var ring = CreateGroup(name, parent);
            ring.localPosition = center;
            var segments = 16;
            for (var i = 0; i < segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                var x = Mathf.Cos(angle) * radius;
                var z = Mathf.Sin(angle) * radius;
                var segment = CreatePrimitive(PrimitiveType.Cube, $"{name}_Segment_{i:00}", ring, new Vector3(x, 0f, z), new Vector3(1.25f, segmentThickness, 0.34f), material, editabilityClass, role, true);
                segment.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
            }
            AddMarker(ring.gameObject, editabilityClass, role, true);
        }

        private static void AddLabel(Transform parent, string name, string text, Vector3 localPosition, float characterSize)
        {
            var label = new GameObject(name);
            label.transform.SetParent(parent);
            label.transform.localPosition = localPosition;
            label.transform.localRotation = Quaternion.Euler(62f, 0f, 0f);
            label.transform.localScale = Vector3.one;

            var mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.characterSize = characterSize;
            mesh.fontSize = 64;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.color = new Color(0.93f, 0.88f, 0.74f);
            AddMarker(label, UnderstoryEditabilityClass.StoryGuide, text, false);
            label.SetActive(false);
        }

        private static void CreateLight(string name, Transform parent, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.Soft;
        }

        private static void AddMarker(GameObject go, UnderstoryEditabilityClass editabilityClass, string role, bool required)
        {
            var marker = go.GetComponent<UnderstoryEditabilityMarker>() ?? go.AddComponent<UnderstoryEditabilityMarker>();
            marker.editabilityClass = editabilityClass;
            marker.gameplayRole = role;
            marker.sourceOfTruthNote = "SOURCE_OF_TRUTH.md + START_HERE_UNDERSTORY_HANDOFF_v7.md";
            marker.phaseZeroRequired = required;
        }

        private static void AddInteractable(Transform root, string objectName, string interactionId, string displayName, string hint)
        {
            var target = FindChild(root, objectName);
            if (target == null)
                throw new InvalidOperationException($"Cannot add Scene01 interactable. Missing object `{objectName}`.");

            var interactable = target.GetComponent<Scene01Interactable>() ?? target.gameObject.AddComponent<Scene01Interactable>();
            interactable.interactionId = interactionId;
            interactable.displayName = displayName;
            interactable.objectiveHint = hint;
            interactable.requiresTraceGesture = interactionId == "trace_extract";
            interactable.actionMarker = CreateActionMarker(root, target, interactionId);
        }

        private static Transform CreateActionMarker(Transform root, Transform target, string interactionId)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = $"ActionMarker_{interactionId}";
            var renderer = target.GetComponentInChildren<Renderer>();
            var bounds = renderer != null ? renderer.bounds : new Bounds(target.position, Vector3.one);
            marker.transform.position = bounds.center + Vector3.up * Mathf.Max(0.85f, bounds.extents.y + 0.45f);
            marker.transform.SetParent(target, true);
            marker.transform.localRotation = Quaternion.identity;
            marker.transform.localScale = Vector3.one * 0.52f;
            marker.GetComponent<Renderer>().sharedMaterial = Materials["lantern_glow"];
            AddMarker(marker, UnderstoryEditabilityClass.StoryGuide, $"Tap marker for interaction `{interactionId}`.", false);
            marker.SetActive(false);
            return marker.transform;
        }

        private static void CreateNode(string name, Transform parent, Vector3 position)
        {
            var node = new GameObject(name);
            node.transform.SetParent(parent);
            node.transform.localPosition = position;
            node.transform.localRotation = Quaternion.identity;
            node.transform.localScale = Vector3.one;
            AddMarker(node, UnderstoryEditabilityClass.StoryGuide, $"Soft tap-to-move runtime node: {name}.", false);
        }

        private static Transform FindChild(Transform root, string objectName)
        {
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == objectName)
                    return child;
            }
            return null;
        }

        private static void LookAt(Transform transform, Vector3 target)
        {
            var direction = target - transform.position;
            transform.rotation = direction.sqrMagnitude > 0.001f ? Quaternion.LookRotation(direction.normalized, Vector3.up) : Quaternion.identity;
        }
    }
}
