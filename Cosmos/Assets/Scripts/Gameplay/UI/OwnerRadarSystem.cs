using Cosmos.Gameplay.GameplayObjects.Character;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// This script takes the position of the other ship and the position of this ship 
    /// and calculates the position of the other ship on the radar.
    /// </summary>
    public class OwnerRadarSystem : NetworkBehaviour
    {
        [Serializable]
        internal class RadarVisual
        {
            [SerializeField] internal Image image;
            internal Transform avatarTransform;
            internal Color radarVisualColor;
            internal bool isInitialized = false;
        }

        [SerializeField] 
        private RadarVisual[] _radarVisual;

        [SerializeField]
        private Transform _radarCanvasTransform;

        [SerializeField] 
        private Transform _ourTransform;

        [SerializeField] 
        private Sprite _frontSprite;

        [SerializeField] 
        private Sprite _backSprite;

        [SerializeField] 
        private float _clampPosValue = 40;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                _radarCanvasTransform.gameObject.SetActive(false);
                enabled = false;
                return;
            }

            _radarCanvasTransform.gameObject.SetActive(true);
        }

        public void Initialize(IReadOnlyDictionary<ulong, NetworkClient> connectedClients)
        {
            int clientIndex = 0;

            foreach (var kvp in connectedClients)
            {
                NetworkObject playerObject = kvp.Value.PlayerObject;

                if (playerObject.IsLocalPlayer)
                    continue;

                if (playerObject.gameObject.TryGetComponent(out NetworkAvatarGuidState avatar))
                {
                    IntializeRadarVisual(clientIndex, playerObject.transform, avatar.RegisteredAvatar.raderVisualColor);
                    clientIndex++;
                }
            }
        }

        private void Update()
        {
            UpdateRadarUI();
        }

        private void IntializeRadarVisual(int index, Transform avatarTransform, Color radarVisualColor)
        {
            if (index < 0 || index >= _radarVisual.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            _radarVisual[index].avatarTransform = avatarTransform;
            _radarVisual[index].radarVisualColor = radarVisualColor;
            _radarVisual[index].image.color = radarVisualColor;
            _radarVisual[index].image.gameObject.SetActive(true);
            _radarVisual[index].isInitialized = true;
        }

        public void UpdateRadarUI()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (!_radarVisual[i].isInitialized)
                    continue;

                Vector3 vectorFromThisShipToOtherShip = _radarVisual[i].avatarTransform.position - _ourTransform.position;
                vectorFromThisShipToOtherShip = _radarCanvasTransform.InverseTransformDirection(vectorFromThisShipToOtherShip);
                Vector3 projectedVector = Vector3.ProjectOnPlane(vectorFromThisShipToOtherShip, _radarCanvasTransform.forward);
                _radarVisual[i].image.rectTransform.anchoredPosition = new Vector2(
                        Mathf.Clamp((projectedVector * 100).x, -_clampPosValue, _clampPosValue),
                        Mathf.Clamp((projectedVector * 100).y, -_clampPosValue, _clampPosValue));

                _radarVisual[i].image.sprite = vectorFromThisShipToOtherShip.z < 0 ? _backSprite : _frontSprite;
            }
        }
    }
}
