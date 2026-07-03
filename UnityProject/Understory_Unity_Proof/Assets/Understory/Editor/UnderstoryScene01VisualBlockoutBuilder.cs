using System;
using System.Collections.Generic;
using System.IO;
using Understory;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Understory.Editor
{
    public static class UnderstoryScene01VisualBlockoutBuilder
    {
        private const string Root = "Assets/Understory";
        private const string ScenePath = Root + "/Scenes/Scene01_SummitHatch_BoreRoom.unity";
        private const string MaterialFolder = Root + "/Materials/Scene01_Blockout";

        private static readonly Dictionary<string, Material> Materials = new();

        public static void BuildScene01VisualProofPass1()
        {
            EnsureFolders();
            EnsureMaterials();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Scene01_SummitHatch_BoreRoom";

            var root = new GameObject("Scene01_VisualProofPass1");
            AddMarker(root, UnderstoryEditabilityClass.StoryGuide, "Scene 01 visual proof root. Scope: visual/blockout only.", true);

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
            Directory.CreateDirectory(Root + "/Scripts");
            Directory.CreateDirectory(Root + "/Scenes");
        }

        private static void EnsureMaterials()
        {
            Materials.Clear();

            CreateMaterial("summit_snow", new Color(0.86f, 0.9f, 0.88f), 0.78f);
            CreateMaterial("summit_stone", new Color(0.47f, 0.46f, 0.41f), 0.88f);
            CreateMaterial("rough_stone", new Color(0.34f, 0.34f, 0.31f), 0.82f);
            CreateMaterial("clay_cut", new Color(0.58f, 0.35f, 0.25f), 0.9f);
            CreateMaterial("timber_brace", new Color(0.43f, 0.28f, 0.16f), 0.72f);
            CreateMaterial("fired_brick", new Color(0.63f, 0.24f, 0.16f), 0.7f);
            CreateMaterial("seed_soil_dead", new Color(0.2f, 0.16f, 0.12f), 0.95f);
            CreateMaterial("seed_soil_viable", new Color(0.26f, 0.34f, 0.18f), 0.9f);
            CreateMaterial("blackglass", new Color(0.03f, 0.05f, 0.06f), 0.32f);
            CreateMaterial("filterstone", new Color(0.42f, 0.55f, 0.5f), 0.86f);
            CreateMaterial("line_ceramic", new Color(0.75f, 0.7f, 0.6f), 0.6f);
            CreateMaterial("hollower_mark", new Color(0.55f, 0.08f, 0.05f), 0.8f);
            CreateMaterial("steward_cloth", new Color(0.18f, 0.32f, 0.34f), 0.82f);
            CreateMaterial("worker_cloth", new Color(0.5f, 0.45f, 0.36f), 0.84f);
            CreateMaterial("lantern_glow", new Color(1f, 0.68f, 0.28f), 0.35f, 1.8f);
            CreateMaterial("bore_dark", new Color(0.025f, 0.024f, 0.022f), 0.95f);
            CreateMaterial("ghost_draft", new Color(0.35f, 0.72f, 0.86f, 0.36f), 0.5f, 0f, true);
            CreateMaterial("editable_zone", new Color(0.3f, 0.6f, 0.45f, 0.22f), 0.8f, 0f, true);
            CreateMaterial("veil", new Color(0.55f, 0.72f, 0.74f, 0.28f), 0.95f, 0f, true);
            CreateMaterial("protected_zone", new Color(0.2f, 0.24f, 0.3f, 0.36f), 0.82f, 0f, true);
        }

        private static void CreateMaterial(string id, Color color, float roughness, float emission = 0f, bool transparent = false)
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
                mat.SetFloat("_Metallic", 0f);

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
            RenderSettings.fogColor = new Color(0.46f, 0.6f, 0.62f);
            RenderSettings.fogDensity = 0.012f;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.72f, 0.68f, 0.58f);
            RenderSettings.ambientEquatorColor = new Color(0.38f, 0.46f, 0.45f);
            RenderSettings.ambientGroundColor = new Color(0.12f, 0.11f, 0.1f);
            RenderSettings.ambientIntensity = 1.05f;

            var sun = new GameObject("Sun_WarmLowAngle");
            sun.transform.SetParent(root);
            sun.transform.SetPositionAndRotation(new Vector3(-8f, 18f, -10f), Quaternion.Euler(42f, -35f, 0f));
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.86f, 0.62f);
            light.intensity = 2.25f;
            light.shadows = LightShadows.Soft;

            CreateLight("BoreRoom_LanternKey", root, new Vector3(0f, -7f, 17f), new Color(1f, 0.58f, 0.28f), 4.5f, 16f);
            CreateLight("Surface_WorkLanterns", root, new Vector3(-7f, 3.2f, -1f), new Color(1f, 0.68f, 0.34f), 2.4f, 12f);

            var camera = new GameObject("Main Camera");
            camera.tag = "MainCamera";
            camera.transform.SetParent(root);
            camera.transform.position = new Vector3(24f, 18f, -30f);
            LookAt(camera.transform, new Vector3(0f, 0f, 6f));
            var cam = camera.AddComponent<Camera>();
            cam.fieldOfView = 44f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 500f;
            cam.allowHDR = true;
            camera.AddComponent<AudioListener>();

            var boreCamera = new GameObject("CaptureCamera_BoreRoomReveal");
            boreCamera.transform.SetParent(root);
            boreCamera.transform.position = new Vector3(13f, -2f, 33f);
            LookAt(boreCamera.transform, new Vector3(0f, -8f, 17f));
            var boreCam = boreCamera.AddComponent<Camera>();
            boreCam.enabled = false;
            boreCam.fieldOfView = 42f;

            var surfaceCamera = new GameObject("CaptureCamera_SurfaceRepairCluster");
            surfaceCamera.transform.SetParent(root);
            surfaceCamera.transform.position = new Vector3(-17f, 10f, -16f);
            LookAt(surfaceCamera.transform, new Vector3(-3f, 1f, -1f));
            var surfaceCam = surfaceCamera.AddComponent<Camera>();
            surfaceCam.enabled = false;
            surfaceCam.fieldOfView = 38f;
        }

        private static void BuildSurface(Transform root)
        {
            var surface = CreateGroup("01_Surface_SummitWorkyard", root);

            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_MountainSummitMass", surface, new Vector3(0f, -1.25f, 0f), Vector3.Scale(new Vector3(28f, 2.5f, 22f), Vector3.one), Materials["summit_stone"], UnderstoryEditabilityClass.ProtectedShell, "Authored mountain shell. Not editable.", true);
            CreatePrimitive(PrimitiveType.Cylinder, "A_ProtectedShell_SummitSnowCap", surface, new Vector3(0f, 0.05f, 0f), new Vector3(21f, 0.4f, 16f), Materials["summit_snow"], UnderstoryEditabilityClass.ProtectedShell, "Authored summit cap and silhouette.", true);
            CreatePrimitive(PrimitiveType.Cube, "Veil_Atmosphere_BelowSummit", surface, new Vector3(0f, -8.2f, 12f), new Vector3(48f, 3.5f, 18f), Materials["veil"], UnderstoryEditabilityClass.StoryGuide, "Visible Veil below the workyard. Progression wall only in this pass.", true);
            CreatePrimitive(PrimitiveType.Cube, "Surface_BroadEditableZone_C_D_F", surface, new Vector3(-2f, 0.38f, -2f), new Vector3(18f, 0.08f, 11f), Materials["editable_zone"], UnderstoryEditabilityClass.StoryGuide, "Broad editable/buildable zone overlay for ruined and repairable surface pieces.", true);

            BuildRepairCluster(surface);
            BuildShallowCutAndHatch(surface);
            BuildCharacters(surface);
            BuildReturnAndRefineStubs(surface);

            AddLabel(surface, "Label_SurfaceScope", "Surface proof: damaged repair cluster + shallow cut + hidden hatch discovery", new Vector3(-2f, 5.2f, -10f), 0.42f);
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
            var cut = CreateGroup("HatchDiscovery_ShallowMaterialCut", surface);
            CreatePrimitive(PrimitiveType.Cube, "E_ExtractionVolume_ShallowSummitCut", cut, new Vector3(5.4f, 0.2f, -1.3f), new Vector3(6.5f, 0.42f, 3.6f), Materials["clay_cut"], UnderstoryEditabilityClass.ExtractionVolume, "Workers gather scarce clay/stone here and uncover the hatch.", true);
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

            AddLabel(cut, "Label_HatchDiscovery", "Workers uncover hatch while gathering scarce material", new Vector3(5.1f, 3.7f, 2.4f), 0.3f);
        }

        private static void BuildCharacters(Transform surface)
        {
            var characters = CreateGroup("VisibleCrewPlaceholders", surface);

            CreateCapsule("StewardPlaceholder_SummitSteward", characters, new Vector3(-1.5f, 1.25f, -1.4f), 0.55f, 1.9f, Materials["steward_cloth"], UnderstoryEditabilityClass.StoryGuide, "Visible Summit Steward placeholder. Soft tap-to-move node target.");
            CreatePrimitive(PrimitiveType.Cube, "StewardPlaceholder_LineKey_FieldTool", characters, new Vector3(-0.85f, 1.8f, -1.4f), new Vector3(0.12f, 0.9f, 0.12f), Materials["line_ceramic"], UnderstoryEditabilityClass.StoryGuide, "Field tool placeholder. Name intentionally not final.", true);

            CreateCapsule("WorkerPlaceholder_ShallowCut_A", characters, new Vector3(6.8f, 1.05f, -3.0f), 0.42f, 1.55f, Materials["worker_cloth"], UnderstoryEditabilityClass.StoryGuide, "Worker placeholder at shallow material cut.");
            CreateCapsule("WorkerPlaceholder_ShallowCut_B", characters, new Vector3(3.3f, 1.05f, -3.25f), 0.42f, 1.55f, Materials["worker_cloth"], UnderstoryEditabilityClass.StoryGuide, "Worker placeholder calling the Steward to the hatch.");
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
            AddLabel(loop, "Label_ReturnRefineStub", "Loop stubs only: haul table + kiln, no economy yet", new Vector3(3.2f, 2.8f, -7.6f), 0.24f);
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
            CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuiltGhostDraft_BoreShoring", bore, new Vector3(-4.2f, 1.6f, -1.9f), new Vector3(0.35f, 2.5f, 0.35f), Materials["ghost_draft"], UnderstoryEditabilityClass.PlayerBuilt, "Ghost shoring preview for later extraction support.", true);
            var committedShoring = CreatePrimitive(PrimitiveType.Cube, "D_PlayerBuilt_BoreShoring_Committed", bore, new Vector3(-4.2f, 1.6f, -1.9f), new Vector3(0.35f, 2.5f, 0.35f), Materials["timber_brace"], UnderstoryEditabilityClass.PlayerBuilt, "Committed shoring that gates safe extraction.", true);
            committedShoring.SetActive(false);

            CreatePrimitive(PrimitiveType.Cube, "Works_MistEngine_ClearerHint_Inactive", bore, new Vector3(7.1f, 1.5f, 1.8f), new Vector3(2.2f, 2.4f, 1.2f), Materials["filterstone"], UnderstoryEditabilityClass.ProtectedLandmark, "Inactive Works / Mist Engine / Clearer hint. Practical machine, not magic.", true);
            CreatePrimitive(PrimitiveType.Cube, "TheLines_InactiveWallConduit_A", bore, new Vector3(4.1f, 2.0f, -4.9f), new Vector3(6.4f, 0.18f, 0.28f), Materials["line_ceramic"], UnderstoryEditabilityClass.ProtectedLandmark, "The Lines: inactive conduit hint.", true);
            CreatePrimitive(PrimitiveType.Cube, "BlackVault_SealedHint", bore, new Vector3(8.6f, 1.35f, -2.9f), new Vector3(1.4f, 1.5f, 1.0f), Materials["blackglass"], UnderstoryEditabilityClass.ProtectedLandmark, "Black Vault sealed hint. Seed only.", true);
            CreatePrimitive(PrimitiveType.Cube, "HollowerWarningMark_OverOlderStone", bore, new Vector3(2.7f, 1.8f, -5.08f), new Vector3(1.0f, 0.75f, 0.08f), Materials["hollower_mark"], UnderstoryEditabilityClass.ProtectedLandmark, "Hollower warning mark painted over older stone.", true);

            CreatePrimitive(PrimitiveType.Cube, "BoreRoom_BroadEditableZone_C_D_E", bore, new Vector3(-3.5f, 0.52f, 1.9f), new Vector3(8.5f, 0.07f, 5f), Materials["editable_zone"], UnderstoryEditabilityClass.StoryGuide, "Interior editable/debris/extraction area overlay.", true);
            AddLabel(bore, "Label_BoreRoom", "Bore Room: built, old, deep, practical. Protected rim + editable debris/cache.", new Vector3(0f, 5.6f, -6.6f), 0.34f);
        }

        private static void BuildLegend(Transform root)
        {
            var legend = CreateGroup("00_EditabilityLegend_And_Guardrails", root);
            legend.position = new Vector3(-17f, 2.4f, 7f);

            AddLabel(legend, "Label_Guardrail_NoOldLanguage", "Guardrail: use The Works / Clearers / Mist Engines / The Lines / Black Vaults", Vector3.zero, 0.26f);
            AddLabel(legend, "Label_EditabilityClasses", "A Protected shell | B Protected landmark | C Destroyable ruin | D Player-built | E Extraction volume | F Repair anchor", new Vector3(0f, -0.8f, 0f), 0.22f);
            AddLabel(legend, "Label_PhaseScope", "Phase 0 visual proof only: no store, combat, multiplayer, water sim, or full procedural mountain", new Vector3(0f, -1.5f, 0f), 0.22f);
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
            AddInteractable(root, "ReturnRitualStub_HaulTable", "haul_table", "Haul table", "Return material physically to the summit.");
            AddInteractable(root, "RefinerStub_FirstKiln", "kiln", "First kiln", "Refine raw haul into buildable material.");
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

        private static void CreateCapsule(string name, Transform parent, Vector3 position, float radius, float height, Material material, UnderstoryEditabilityClass editabilityClass, string role)
        {
            var capsule = CreatePrimitive(PrimitiveType.Capsule, name, parent, position, new Vector3(radius, height * 0.5f, radius), material, editabilityClass, role, true);
            var lantern = CreatePrimitive(PrimitiveType.Sphere, name + "_Lantern", parent, position + new Vector3(0.38f, -0.05f, 0.18f), Vector3.one * 0.18f, Materials["lantern_glow"], UnderstoryEditabilityClass.StoryGuide, "Small lantern/readability marker.", false);
            lantern.transform.SetParent(capsule.transform, true);
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
