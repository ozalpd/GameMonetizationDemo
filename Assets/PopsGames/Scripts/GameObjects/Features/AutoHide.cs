using UnityEngine;

namespace Pops.GameObjects.Features
{
    /// <summary>
    /// Automatically hides game object by disabling its mesh renderer and collider.
    /// Intended to use transforms those are needed to be seen in design time and should not be visible in game.
    /// </summary>
    public class AutoHide : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private new Collider collider;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.enabled = false;

            collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;
        }

    }
}