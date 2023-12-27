using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    [RequireComponent(typeof(Image))]
    public class UITinter : MonoBehaviour
    {
        [SerializeField] private Color[] _tintColors;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetToColor(int index)
        {
            if (index < 0 || index >= _tintColors.Length)
            {
                Debug.LogError($"Index {index} is out of range for tint colors.");
                return;
            }

            _image.color = _tintColors[index];
        }
    }
}
