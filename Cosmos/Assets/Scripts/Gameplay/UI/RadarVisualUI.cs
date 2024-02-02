using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Contains specific configurations for the visual representation of a friendly ship on the radar.
    /// </summary>
    public class RadarVisualUI : MonoBehaviour
    {
        [SerializeField] private Image _radarVisual;
        [SerializeField] private Sprite _frontSprite;
        [SerializeField] private Sprite _backSprite;

        [SerializeField] private float _clampPosValue = 40;

        public void UpdateRadarUI(Vector3 thisPosition, Vector3 otherPosition)
        {
            Vector3 vectorFromThisShipToOtherShip = otherPosition - thisPosition;
            vectorFromThisShipToOtherShip = transform.InverseTransformDirection(vectorFromThisShipToOtherShip);
            Vector3 projectedVector = Vector3.ProjectOnPlane(vectorFromThisShipToOtherShip, transform.forward);
            _radarVisual.rectTransform.anchoredPosition = new Vector2(
                    Mathf.Clamp((projectedVector * 100).x, -_clampPosValue, _clampPosValue),
                    Mathf.Clamp((projectedVector * 100).y, -_clampPosValue, _clampPosValue));

            _radarVisual.sprite = vectorFromThisShipToOtherShip.z < 0 ? _backSprite : _frontSprite;
        }
    }
}
