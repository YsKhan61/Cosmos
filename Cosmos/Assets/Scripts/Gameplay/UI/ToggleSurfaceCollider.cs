using UnityEngine;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Activate / Deactivate the surface collider (collider component) of IPointable RayInteractable UI elements in VR (Meta)
    /// </summary>
    public class ToggleSurfaceCollider : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider _surfaceCollider;

        private void Start()
        {
            _surfaceCollider.enabled = false;
        }

        public void Activate(bool activate)
        {
            _surfaceCollider.enabled = activate;
        }
    }

}
