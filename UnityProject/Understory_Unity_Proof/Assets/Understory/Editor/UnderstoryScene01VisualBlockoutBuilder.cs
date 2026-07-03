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
        private const string TextureFolder = MaterialFolder + "/ProceduralTextures";

        private static readonly Dictionary<string, Material> Materials = new();
        private static readonly Dictionary<string, Texture2D> MaterialTextures = new();

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
            Directory.CreateDirectory(TextureFolder);
            Directory.CreateDirectory(Root + "/Scripts");
            Directory.CreateDirectory(Root + "/Scenes");
            Directory.CreateDirectory(Root + "/Settings");
        }

        private static void EnsureMaterials()
        {
            Materials.Clear();
            MaterialTextures.Clear();

            CreateMaterial("summit_snow", new Color(0.7f, 0.73f, 0.67f), 0.92f, pattern: SurfacePattern.Snow);
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
            light.intensity = 3.15f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.74f;

            CreateLight("BoreRoom_LanternKey", root, new Vector3(0f, -7f, 17f), new Color(1f, 0.55f, 0.25f), 5.5f, 18f);
            CreateLight("Surface_WorkLanterns", root, new Vector3(-7f, 3.2f, -1f), new Color(1f, 0.63f, 0.28f), 2.7f, 13f);
            CreateLight("GoldenRim_FromVeil", root, new Vector3(14f, 5.6f, -14f), new Color(1f, 0.47f, 0.2f), 1.6f, 28f);

            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            camera.transform.SetParent(root);
            camera.transform.position = new Vector3(13.8f, 8.2f, -15.4f);
            LookAt(camera.transform, new Vector3(-3.6f, 1.05f, 0.3f));
            var cam = camera.AddComponent<Camera>();
            cam.fieldOfView = 34f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 500f;
            cam.allowHDR = true;
            EnablePostProcessing(cam);
            camera.AddComponent<AudioListener>();

            var boreCamera = new GameObject("CaptureCamera_BoreRoomReveal");
            boreCamera.transform.SetParent(root);
            boreCamera.transform.position = new Vector3(7.8f, -6.4f, 31.2f);
            LookAt(boreCamera.transform, new Vector3(-1.6f, -10.1f, 18.8f));
            var boreCam = boreCamera.AddComponent<Camera>();
            boreCam.enabled = false;
            boreCam.fieldOfView = 34f;
            boreCam.allowHDR = true;
            boreCam.clearFlags = CameraClearFlags.SolidColor;
            boreCam.backgroundColor = new Color(0.04f, 0.036f, 0.032f);
            EnablePostProcessing(boreCam);

            var surfaceCamera = new GameObject("CaptureCamera_SurfaceRepairCluster");
            surfaceCamera.transform.SetParent(root);
            surfaceCamera.transform.position = new Vector3(-9.4f, 5.7f, -8.8f);
            LookAt(surfaceCamera.transform, new Vector3(-4.9f, 1.25f, -0.2f));
            var surfaceCam = surfaceCamera.AddComponent<Camera>();
            surfaceCam.enabled = false;
            surfaceCam.fieldOfView = 31f;
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
            color.postExposure.Override(0.08f);
            color.contrast.Override(18f);
            color.saturation.Override(-8f);
            color.colorFilter.Override(new Color(1f, 0.93f, 0.84f));

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
            CreatePrimitive(PrimitiveType.Cube, "Veil_Atmosphere_BelowSummit", surface, new Vector3(0f, -8.2f, 12f), new Vector3(48f, 3.5f, 18f), Materials["veil"], UnderstoryEditabilityClass.StoryGuide, "Visible Veil below the workyard. Progression wall only in this pass.", true);
            CreatePrimitive(PrimitiveType.Cube, "Surface_BroadEditableZone_C_D_F", surface, new Vector3(-2f, 0.38f, -2f), new Vector3(18f, 0.08f, 11f), Materials["editable_zone"], UnderstoryEditabilityClass.StoryGuide, "Broad editable/buildable zone overlay for ruined and repairable surface pieces.", true);

            BuildSummitRefugeMiniature(surface);
            BuildRepairCluster(surface);
            BuildShallowCutAndHatch(surface);
            BuildCharacters(surface);
            BuildReturnAndRefineStubs(surface);
            BuildShaftheadProgress(surface);
            BuildSurfaceMaterialScatter(surface);

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
        }

        private static void BuildBoreRoom(Transform root)
        {
            var bore = CreateGroup("02_BoreRoom_Blockout", root);
            bore.position = new Vector3(0f, -9.5f, 20f);

            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_BoreRoomContainment", bore, new Vector3(0f, -0.4f, 0f), new Vector3(13f, 0.5f, 10f), Materials["protected_zone"], UnderstoryEditabilityClass.ProtectedShell, "Authored chamber containment shell. Protected.", true);
            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_MainBoreShaft", bore, new Vector3(0f, -4.2f, 0f), new Vector3(3.2f, 7.5f, 3.2f), Materials["bore_dark"], UnderstoryEditabilityClass.ProtectedShell, "Main Bore shaft drops beyond view. Protected shell.", true);
            CreateRing("B_ProtectedLandmark_ProtectedShaftRim", bore, new Vector3(0f, 0.35f, 0f), 4.1f, 0.34f, Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Protected shaft rim. Not casually editable.");
            CreateRing("B_ProtectedLandmark_BrokenGuideRings", bore, new Vector3(0f, 1.2f, 0f), 5.4f, 0.22f, Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "Broken guide rings / lift rail read.");

            CreatePrimitive(PrimitiveType.Cube, "E_ExtractionVolume_BoreMaterialCache", bore, new Vector3(-7.3f, 0.65f, 1.8f), new Vector3(3.1f, 1.15f, 2.3f), Materials["clay_cut"], UnderstoryEditabilityClass.ExtractionVolume, "First interior material cache / extraction volume.", true);
            CreatePrimitive(PrimitiveType.Cube, "C_DestroyableRuin_EditableBoreDebris_A", bore, new Vector3(-6.1f, 1.35f, -2.1f), new Vector3(2.2f, 1.0f, 1.3f), Materials["rough_stone"], UnderstoryEditabilityClass.DestroyableRuin, "Editable debris field in Bore Room.", true);
            var traceSeam = CreatePrimitive(PrimitiveType.Cube, "TraceSeam_Glow", bore, new Vector3(-7.3f, 1.35f, 3.05f), new Vector3(2.4f, 0.08f, 0.08f), Materials["lantern_glow"], UnderstoryEditabilityClass.ExtractionVolume, "Glowing seam target for the careful trace gesture.", true);
            traceSeam.SetActive(false);
            var blastDust = CreatePrimitive(PrimitiveType.Sphere, "BlastImpact_Dust", bore, new Vector3(-6.1f, 1.75f, -2.1f), new Vector3(1.3f, 0.65f, 1.3f), Materials["clay_cut"], UnderstoryEditabilityClass.StoryGuide, "Blast feedback cloud. Visible warning, not reward.", false);
            blastDust.SetActive(false);
            var collapseBurial = CreatePrimitive(PrimitiveType.Cube, "CollapseBurial_BoreDebris", bore, new Vector3(-3.8f, 1.05f, 0.4f), new Vector3(1.7f, 0.85f, 1.6f), Materials["rough_stone"], UnderstoryEditabilityClass.DestroyableRuin, "Blast burial detour: collapse buries, never deletes.", true);
            collapseBurial.SetActive(false);
            CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuiltGhostDraft_BoreShoring", bore, new Vector3(-4.2f, 1.6f, -1.9f), new Vector3(0.35f, 2.5f, 0.35f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Ghost shoring preview for later extraction support.", true);
            var committedShoring = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_BoreShoring_Committed", bore, new Vector3(-4.2f, 1.6f, -1.9f), new Vector3(0.35f, 2.5f, 0.35f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Committed shoring that gates safe extraction.", true);
            committedShoring.SetActive(false);

            CreatePrimitive(PrimitiveType.Cube, "Works_MistEngine_ClearerHint_Inactive", bore, new Vector3(7.1f, 1.5f, 1.8f), new Vector3(2.2f, 2.4f, 1.2f), Materials["filterstone"], UnderstoryEditabilityClass.ProtectedLandmark, "Inactive Works / Mist Engine / Clearer hint. Practical machine, not magic.", true);
            CreatePrimitive(PrimitiveType.Cube, "TheLines_InactiveWallConduit_A", bore, new Vector3(4.1f, 2.0f, -4.9f), new Vector3(6.4f, 0.18f, 0.28f), Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "The Lines: inactive conduit hint.", true);
            CreatePrimitive(PrimitiveType.Cube, "BlackVault_SealedHint", bore, new Vector3(8.6f, 1.35f, -2.9f), new Vector3(1.4f, 1.5f, 1.0f), Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Black Vault sealed hint. Seed only.", true);
            CreatePrimitive(PrimitiveType.Cube, "HollowerWarningMark_OverOlderStone", bore, new Vector3(2.7f, 1.8f, -5.08f), new Vector3(1.0f, 0.75f, 0.08f), Materials["hollower_mark"], UnderstoryEditabilityClass.ProtectedLandmark, "Hollower warning mark painted over older stone.", true);

            CreatePrimitive(PrimitiveType.Cube, "BoreRoom_BroadEditableZone_C_D_E", bore, new Vector3(-3.5f, 0.52f, 1.9f), new Vector3(8.5f, 0.07f, 5f), Materials["editable_zone"], UnderstoryEditabilityClass.StoryGuide, "Interior editable/debris/extraction area overlay.", true);
            BuildBoreRoomDressings(bore);
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

            AddInteractable(root, "E_ExtractionVolume_ShallowSummitCut", "surface_cut", "Shallow material cut", "Workers gather scarce material here.");
            AddInteractable(root, "C_DestroyableRuin_HatchMineralCrust", "hatch_crust", "Hatch crust", "Clear the crust to expose the hatch.");
            AddInteractable(root, "B_ProtectedLandmark_HiddenHatch_Current", "hatch", "Hidden hatch", "Open the hatch into the Bore Room.");
            AddInteractable(root, "D_PlayerBuiltGhostDraft_BoreShoring", "bore_shoring", "Bore shoring", "Place shoring before extraction.");
            AddInteractable(root, "E_ExtractionVolume_BoreMaterialCache", "trace_extract", "Trace extraction cache", "Carefully trace the material face.");
            AddInteractable(root, "C_DestroyableRuin_EditableBoreDebris_A", "blast_extract", "Blast extraction debris", "Rough blast option for the material face.");
            AddInteractable(root, "CollapseBurial_BoreDebris", "reexcavate_burial", "Buried return path", "Clear the blast burial; collapse buries, never deletes.");
            AddInteractable(root, "ReturnRitualStub_HaulTable", "haul_table", "Haul table", "Return material physically to the summit.");
            AddInteractable(root, "RefinerStub_FirstKiln", "kiln", "First kiln", "Refine raw haul into buildable material.");
            AddInteractable(root, "CoreSample_GlassColumn", "core_sample", "Core Sample", "Add the first stratum band.");
            AddInteractable(root, "ArchiveShelf_Phase0", "archive_shelf", "Archive shelf", "Place the first fragment as an object.");
            AddInteractable(root, "F_RepairAnchor_ShelterWindbreak", "repair_shelter", "Shelter windbreak", "Consumes brick and timber.");
            AddInteractable(root, "F_RepairAnchor_BrokenTerraceEdge", "repair_terrace", "Broken terrace edge", "Consumes rough stone.");
            AddInteractable(root, "F_RepairAnchor_TinySoilBed", "repair_garden", "Tiny garden bed", "Consumes viable seedclay.");
            AddInteractable(root, "Surface_BroadEditableZone_C_D_F", "surface_build_zone", "Surface build zone", "Place the first support block.");
            AddInteractable(root, "D_PlayerBuilt_Block_01", "player_build_block", "Player support block", "Remove or replace the proof block.");

            var nodes = CreateGroup("Scene01_RuntimeNodes", root);
            CreateNode("Scene01Node_RepairCluster", nodes, new Vector3(-5.9f, 1.2f, -4.2f));
            CreateNode("Scene01Node_ShallowCut", nodes, new Vector3(5.5f, 1.05f, -3.25f));
            CreateNode("Scene01Node_Hatch", nodes, new Vector3(3.8f, 1.1f, -1.95f));
            CreateNode("Scene01Node_BoreInspection", nodes, new Vector3(-4.2f, -8.2f, 18.1f));
            CreateNode("Scene01Node_HaulTable", nodes, new Vector3(1.25f, 1.32f, -7.8f));
            CreateNode("Scene01Node_Kiln", nodes, new Vector3(4.9f, 1.62f, -6.7f));
            CreateNode("Scene01Node_BuildZone", nodes, new Vector3(-1.1f, 1.2f, -3.7f));
            CreateNode("Scene01Node_CoreSample", nodes, new Vector3(8.6f, 1.1f, -6.9f));
            CreateNode("Scene01Node_ArchiveShelf", nodes, new Vector3(10.25f, 1.1f, -6.6f));
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
