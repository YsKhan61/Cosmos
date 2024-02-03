using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.GameplayObjects.Character.UI
{
    [ExecuteAlways]
    /// <summary>
    /// Testing purpose only. - Delete it later.
    /// </summary>
    public class RadarUI : MonoBehaviour
    {
        [Serializable]
        internal class RadarVisual
        {
            [SerializeField] internal Image image;

            // ClientId, AvatarPosition, ImageColor, IsInitialized are made serializable for Debug visual only.
            [SerializeField] internal Transform targetTransform;
            [SerializeField] internal Color imageColor;
            [SerializeField] internal bool isInitialized = true;           // Once the radar visual is initialized, it will be set to true.
        }

        [SerializeField]
        private RadarVisual[] _radarVisual;                         // each avatar in the game will have unique radar visual.

        [SerializeField]
        private Transform _radarCanvasTransform;

        [SerializeField, Tooltip("Sprite to indicate spaceship in front of this.")]
        private Sprite _frontSprite;

        [SerializeField, Tooltip("Sprite to indicate spaceship in the back of this.")]
        private Sprite _backSprite;

        [SerializeField]
        private float _clampPosValue = 40;

        [SerializeField]
        private float _scaleMultiplier = 0.01f;                     // Scale the radar distance respect to world distance.

        [SerializeField]
        private Transform _graphicsTransform;           // our graphics transform

        // Start is called before the first frame update
        void Start()
        {
            _radarCanvasTransform.gameObject.SetActive(true);
            InitializeUI();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRadarUI();
        }

        private void InitializeUI()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                _radarVisual[i].image.color = _radarVisual[i].imageColor;
                _radarVisual[i].image.gameObject.SetActive(true);
                _radarVisual[i].isInitialized = true;
            }
        }

        private void UpdateRadarUI()
        {
            // UpdateAvatarPositions();

            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (!_radarVisual[i].isInitialized)
                    continue;

                Vector3 vectorFromThisShipToOtherShip = _radarVisual[i].targetTransform.position - _graphicsTransform.position;
                vectorFromThisShipToOtherShip = _radarCanvasTransform.InverseTransformDirection(vectorFromThisShipToOtherShip);
                Debug.DrawLine(_radarCanvasTransform.position, vectorFromThisShipToOtherShip, Color.white);
                Vector3 projectedVector = Vector3.ProjectOnPlane(vectorFromThisShipToOtherShip, _radarCanvasTransform.forward);
                projectedVector *= _scaleMultiplier;
                _radarVisual[i].image.rectTransform.anchoredPosition = new Vector2(
                        // Mathf.Lerp(-_clampPosValue, _clampPosValue, (projectedVector).x / _clampPosValue),
                        Mathf.Clamp((projectedVector).x, -_clampPosValue, _clampPosValue),
                        Mathf.Clamp((projectedVector).y, -_clampPosValue, _clampPosValue));

                _radarVisual[i].image.sprite = vectorFromThisShipToOtherShip.z < 0 ? _backSprite : _frontSprite;

                Debug.DrawLine(_graphicsTransform.position, _radarVisual[i].targetTransform.position, _radarVisual[i].imageColor);
                
            }
        }
    }
}