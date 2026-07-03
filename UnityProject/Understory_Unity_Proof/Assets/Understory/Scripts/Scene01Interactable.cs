using UnityEngine;

namespace Understory
{
    public sealed class Scene01Interactable : MonoBehaviour
    {
        public string interactionId;
        public string displayName;
        [TextArea(1, 3)]
        public string objectiveHint;
    }
}
