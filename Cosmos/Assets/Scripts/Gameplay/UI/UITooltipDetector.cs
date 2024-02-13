using UnityEngine;
using UnityEngine.EventSystems;

namespace Cosmos.Gameplay.UI
{
    public class UITooltipDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField, Tooltip("The actual tooltip that should be triggered")]
        private UITooltipPopup _uiTooltipPopup;

        [SerializeField]
        [Multiline]
        [Tooltip("The text of the tooltip (this is the default text, it can also be changed in code)")]
        private string _tooltipText;

        [SerializeField, Tooltip("Should the tooltip appear instantly if the player clicks this UI element")]
        private bool _activateOnClick = true;

        [SerializeField, Tooltip("The length of time this mouse needs to hover over this element before the tooltip appears (in seconds)")]
        private float _tooltipDelay = 0.5f;

        private float _pointerEnterTime = 0;
        private bool _isShwingTooltip;

        private void OnDisable()
        {
            HideTooltip();
        }

        public void SetText(string text)
        {
            bool wasChanged = text != _tooltipText;
            _tooltipText = text;
            if (wasChanged && _isShwingTooltip)
            {
                // we changed the text while of our tooltip was being shown! We need to re-show the tooltip!
                HideTooltip();
                ShowTooltip();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerEnterTime = Time.time;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerEnterTime = 0;
            HideTooltip();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_activateOnClick)
            {
                ShowTooltip();
            }
        }

        private void Update()
        {
            if (_pointerEnterTime != 0 && (Time.time - _pointerEnterTime) > _tooltipDelay)
            {
                ShowTooltip();
            }
        }

        private void ShowTooltip()
        {
            if (!_isShwingTooltip)
            {
                _uiTooltipPopup.ShowTooltip(_tooltipText, Input.mousePosition);
                _isShwingTooltip = true;
            }
        }

        private void HideTooltip()
        {
            if (_isShwingTooltip)
            {
                _uiTooltipPopup.HideTooltip();
                _isShwingTooltip = false;
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (gameObject.scene.rootCount > 1) // Hacky way for checking if this is a scene object or a prefab instance and not a prefab definition
            {
                if (!_uiTooltipPopup)
                {
                    // typically there's only one canvas in the scene, so pick that
                    _uiTooltipPopup = FindObjectOfType<UITooltipPopup>();
                }
            }
        }

#endif
    }
}
