using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Only runs on the OWNER
    /// This script takes the transforms of team members from ClientCharactersCachedInClientMachine and updates the radar UI.
    /// </summary>
    public class OwnerRadarSystem : NetworkBehaviour
    {
        [Serializable]
        internal class RadarVisual
        {
            [SerializeField] internal Image image;

            // ClientId, AvatarPosition, ImageColor, IsInitialized are made serializable for Debug visual only.
            [SerializeField] internal ulong clientId;
            [SerializeField] internal Vector3 avatarPosition;
            [SerializeField] internal Color imageColor;
            [SerializeField] internal bool isInitialized;           // Once the radar visual is initialized, it will be set to true.
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

        [SerializeField, Tooltip("Scale the radar distance respect to world distance.")]
        private float _scaleMultiplier = 1f;

        // caching the graphics transform of the owner, to use it every late update
        private Transform _graphicsTransform;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                _radarCanvasTransform.gameObject.SetActive(false);
                enabled = false;
                return;
            }

            _radarCanvasTransform.gameObject.SetActive(true);

            NetworkManager.Singleton.OnClientConnectedCallback += InitializeRadarVisuals;
            NetworkManager.Singleton.OnClientDisconnectCallback += InitializeRadarVisuals;
        }

        private void Start()
        {
            _graphicsTransform = ClientCharactersCachedInClientMachine.GetClientCharacter(OwnerClientId).GraphicsTransform;

            InitializeRadarVisuals(OwnerClientId);
        }

        private void LateUpdate()
        {
            UpdateRadarUI();
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= InitializeRadarVisuals;
            NetworkManager.Singleton.OnClientDisconnectCallback -= InitializeRadarVisuals;

            Reset();
        }

        private void InitializeRadarVisuals(ulong _)
        {
            Reset();

            List<ClientCharacter> characterList = ClientCharactersCachedInClientMachine.GetAllClientCharacters();

            for (int i = 0, length = characterList.Count; i < length; i++)
            {
                var data = characterList[i];

                if (data == null)
                {
                    ResetRadarUI(i);
                    continue;
                }

                if (data.OwnerClientId == OwnerClientId)
                    continue;

                _radarVisual[i].clientId = data.OwnerClientId;
                _radarVisual[i].avatarPosition = data.GraphicsTransform.position;
                _radarVisual[i].imageColor = data.NetworkAvatarGuidState.RegisteredAvatar.radarVisualColor;
                _radarVisual[i].image.color = _radarVisual[i].imageColor;
                _radarVisual[i].isInitialized = true;
                _radarVisual[i].image.gameObject.SetActive(true);

            }
        }

        private void UpdateRadarUI()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (!_radarVisual[i].isInitialized)
                    continue;

                if (!TryGetAvatarPosition(i, out Vector3 avatarPosition))
                {
                    // Sometimes when a client disconnects, I could not yet figure out why the client character is not removed from the characterlist.
                    ResetRadarUI(i);
                    continue;
                }

                Vector3 vectorFromThisShipToOtherShip = avatarPosition - _graphicsTransform.position;
                vectorFromThisShipToOtherShip = _radarCanvasTransform.InverseTransformDirection(vectorFromThisShipToOtherShip);
                // Debug.DrawLine(_radarCanvasTransform.position, vectorFromThisShipToOtherShip, Color.white);
                Vector3 projectedVector = Vector3.ProjectOnPlane(vectorFromThisShipToOtherShip, _radarCanvasTransform.forward);
                projectedVector *= _scaleMultiplier;
                _radarVisual[i].image.rectTransform.anchoredPosition = new Vector2(
                        Mathf.Clamp(projectedVector.x, -_clampPosValue, _clampPosValue),
                        Mathf.Clamp(projectedVector.y, -_clampPosValue, _clampPosValue));

                _radarVisual[i].image.sprite = vectorFromThisShipToOtherShip.z > 0 ? _backSprite : _frontSprite;

                // Debug.DrawLine(_graphicsTransform.position, GetAvatarPosition(_radarVisual[i].clientId), _radarVisual[i].imageColor);
            }
        }

        private bool TryGetAvatarPosition(int i, out Vector3 avatarPosition)
        {
            ClientCharacter character = ClientCharactersCachedInClientMachine.GetClientCharacter(_radarVisual[i].clientId);

            if (character != null)
            {
                avatarPosition = character.GraphicsTransform.position;
                return true;
            }

            Debug.LogError("OwnerRadarSystem: GetAvatarPosition: ClientId not found in the list.");
            avatarPosition = Vector3.zero;
            return false;
        }

        private void Reset()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                ResetRadarUI(i);
            }
        }

        private void ResetRadarUI(int i)
        {
            _radarVisual[i].clientId = ulong.MaxValue;
            _radarVisual[i].avatarPosition = Vector3.zero;
            _radarVisual[i].imageColor = Color.black;
            _radarVisual[i].isInitialized = false;
            _radarVisual[i].image.gameObject.SetActive(false);
        }
    }
}
