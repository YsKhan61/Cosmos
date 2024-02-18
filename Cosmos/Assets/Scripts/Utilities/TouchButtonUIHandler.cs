using UnityEngine;
using UnityEngine.EventSystems;

namespace Cosmos.Utilities
{
    public class TouchButtonUIHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /*[SerializeField] protected RectTransform background = null;
        [SerializeField] private RectTransform handle = null;
        private RectTransform baseRect = null;*/

        /*private Canvas canvas;
        private Camera cam;*/

        private bool isPressed = false;
        public bool IsPressed => isPressed;

        /*protected virtual void Start()
        {
            baseRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                Debug.LogError("The Joystick is not placed inside a canvas");

            Vector2 center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
        }*/

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Button Pressed");
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Button Released");
        }
    }
}
