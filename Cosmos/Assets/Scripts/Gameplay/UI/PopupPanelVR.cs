using UnityEngine;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Activate / Deactivate the surface collider of IPointable RayInteractable UI elements in VR (Meta)
    /// </summary>
    public class PopupPanelVR : PopupPanel
    {

        public override void Hide()
        {
            base.Hide();

            if (FetchSurfaceCollider(out ToggleSurfaceCollider toggleSurfaceCollider))
                toggleSurfaceCollider.Activate(false);

        }

        protected override void Show()
        {
            base.Show();

            if (FetchSurfaceCollider(out ToggleSurfaceCollider toggleSurfaceCollider))
                toggleSurfaceCollider.Activate(true);
        }

        private bool FetchSurfaceCollider(out ToggleSurfaceCollider toggleSurfaceCollider)
        {
            return transform.parent.TryGetComponent(out toggleSurfaceCollider);
        }
    }

}
