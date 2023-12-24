using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cosmos.Gameplay.UI
{ 
    /// <summary>
    /// This controls the tooltip popup -- the little text blurb that appears when you hover your mouse over an arbitary icon.
    /// </summary>
    public class UITooltipPopup : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField, Tooltip("This transform is shown/hidden to show/hide the popup box")]
        private GameObject _windowRootGO;

        [SerializeField]
        private TextMeshProUGUI _textField;

        [SerializeField]
        private Vector3 _cursorOffset;

        private void Awake()
        {
            Assert.IsNotNull(_canvas);
        }

        public void ShowTooltip(string text, Vector3 screenXY)
        {
            screenXY += _cursorOffset;
            _windowRootGO.transform.position = GetCanvasCoords(screenXY);
            _textField.text = text;
            _windowRootGO.SetActive(true);
        }

        /// <summary>
        /// Hides the current tooltip
        /// </summary>
        public void HideTooltip()
        {
            _windowRootGO.SetActive(false);
        }

        /// <summary>
        /// Maps screen coordinates (e.g. Input.mousePosition) to coordinates on our Canvas.
        /// </summary>
        /// <param name="screenCoords"></param>
        /// <returns></returns>
        private Vector3 GetCanvasCoords(Vector3 screenCoords)
        {
            Vector2 canvasCoords;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                screenCoords,
                _canvas.worldCamera,
                out canvasCoords);
            return _canvas.transform.TransformPoint(canvasCoords);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.scene.rootCount > 1) // Hacky way for checking if this is a scene object or a prefab instance and not a prefab definition
            {
                if (!_canvas)
                {
                    // typically there's only one canvas in the scene, so pick that
                    _canvas = FindObjectOfType<Canvas>();
                }
            }
        }
#endif
    }
}

