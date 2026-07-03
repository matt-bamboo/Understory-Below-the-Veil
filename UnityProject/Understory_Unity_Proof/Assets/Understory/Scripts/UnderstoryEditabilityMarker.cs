using UnityEngine;

namespace Understory
{
    public enum UnderstoryEditabilityClass
    {
        ProtectedShell,
        ProtectedLandmark,
        DestroyableRuin,
        PlayerBuilt,
        ExtractionVolume,
        RepairAnchor,
        StoryGuide
    }

    public sealed class UnderstoryEditabilityMarker : MonoBehaviour
    {
        public UnderstoryEditabilityClass editabilityClass;
        public string gameplayRole;
        public string sourceOfTruthNote;
        public bool phaseZeroRequired;
    }
}
