using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Only runs on the OWNER
    /// This script takes the NetworkList of RadarNetworkData List and updates the radar UI.
    /// </summary>
    public class OwnerRadarSystem1 : NetworkBehaviour
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

        /*[SerializeField] 
        private Transform _ourTransform;*/

        [SerializeField, Tooltip("Sprite to indicate spaceship in front of this.")] 
        private Sprite _frontSprite;

        [SerializeField, Tooltip("Sprite to indicate spaceship in the back of this.")] 
        private Sprite _backSprite;

        [SerializeField] 
        private float _clampPosValue = 40;

        [SerializeField]
        private float _scaleMultiplier = 0.01f;                     // Scale the radar distance respect to world distance.

        private Transform _graphicsTransform;           // our graphics transform

        /*[SerializeField]
        ServerRadarSystem m_serverRadarSystem;*/

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

        /// <summary>
        /// When a client joins the game, or leaves the game, the radar visuals will be reinitialized.
        /// This is called from ServerRadarSystem
        /// </summary>

        /*[ClientRpc]
        public void InitializeRadarVisuals_ClientRpc()
        {
            if (!IsOwner)
                return;

            InitializeRadarVisuals();
        }*/

        private void InitializeRadarVisuals(ulong _)
        {
            Reset();
            // Debug.Log($"OwnerRadarSystem: InitializeRadarVisuals_ClientRpc {OwnerClientId}, RadarDatasCount: {m_serverRadarSystem.n_RadarNetworkDatas.Count}");

            List<ClientCharacter> characterList = ClientCharactersCachedInClientMachine.GetAllClientCharacters();

            for (int i = 0, length = characterList.Count; i < length; i++)
            {
                var data = characterList[i];

                if (data.OwnerClientId == OwnerClientId)
                    continue;

                _radarVisual[i].clientId = data.OwnerClientId;
                _radarVisual[i].avatarPosition = data.transform.position;
                _radarVisual[i].imageColor = data.NetworkAvatarGuidState.RegisteredAvatar.radarVisualColor;
                _radarVisual[i].image.color = _radarVisual[i].imageColor;
                _radarVisual[i].isInitialized = true;
                _radarVisual[i].image.gameObject.SetActive(true);

                // Debug.Log($"NetworkVariable Data: {data.OwnerClientId} {data.transform.position} {data.ImageColor}");
            }
        }

        /// <summary>
        /// Letting each owner call this method to update the radar UI.
        /// Server could call this but that would be bad for performance.
        /// Every frame, server had to call this method for every owner.
        /// </summary>
        private void UpdateRadarUI()
        {
            // UpdateAvatarPositions();

            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (!_radarVisual[i].isInitialized)
                    continue;

                Vector3 vectorFromThisShipToOtherShip = GetAvatarPosition(_radarVisual[i].clientId) - _graphicsTransform.position;
                vectorFromThisShipToOtherShip = _radarCanvasTransform.InverseTransformDirection(vectorFromThisShipToOtherShip);
                Debug.DrawLine(_radarCanvasTransform.position, vectorFromThisShipToOtherShip, Color.white);
                Vector3 projectedVector = Vector3.ProjectOnPlane(vectorFromThisShipToOtherShip, _radarCanvasTransform.forward);
                projectedVector *= _scaleMultiplier;
                _radarVisual[i].image.rectTransform.anchoredPosition = new Vector2(
                        Mathf.Clamp(projectedVector.x, -_clampPosValue, _clampPosValue),
                        Mathf.Clamp(projectedVector.y, -_clampPosValue, _clampPosValue));

                _radarVisual[i].image.sprite = vectorFromThisShipToOtherShip.z > 0 ? _backSprite : _frontSprite;

                Debug.DrawLine(_graphicsTransform.position, GetAvatarPosition(_radarVisual[i].clientId), _radarVisual[i].imageColor);
            }
        }

        private Vector3 GetAvatarPosition(ulong clientId)
        {
            ClientCharacter character = ClientCharactersCachedInClientMachine.GetClientCharacter(clientId);

            if (character != null)
            {
                return character.GraphicsTransform.position;
            }

            Debug.LogError("OwnerRadarSystem: GetAvatarPosition: ClientId not found in the list.");
            return Vector3.zero;

            /*for (int i = 0, length = m_serverRadarSystem.n_RadarNetworkDatas.Count; i < length; i++)
            {
                if (m_serverRadarSystem.n_RadarNetworkDatas[i].ClientId == clientId)
                {
                    return m_serverRadarSystem.n_RadarNetworkDatas[i].AvatarPosition;
                }
            }

            Debug.LogError("OwnerRadarSystem: GetAvatarPosition: ClientId not found in the list.");
            return Vector3.zero;*/
        }

        /*private void UpdateAvatarPositions()
        {
            for (int i = 0, length = m_serverRadarSystem.n_RadarNetworkDatas.Count; i < length; i++)
            {
                if (m_serverRadarSystem.n_RadarNetworkDatas[i].ClientId == NetworkManager.Singleton.LocalClientId)
                    continue;

                for (int j = 0, length2 = _radarVisual.Length; j < length2; j++)
                {
                    if (_radarVisual[j].clientId == m_serverRadarSystem.n_RadarNetworkDatas[i].ClientId)
                    {
                        _radarVisual[j].avatarPosition = m_serverRadarSystem.n_RadarNetworkDatas[i].AvatarPosition;
                        break;
                    }
                }
            }
        }*/

        private void Reset()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                _radarVisual[i].clientId = ulong.MaxValue;
                _radarVisual[i].avatarPosition = Vector3.zero;
                _radarVisual[i].imageColor = Color.black;
                _radarVisual[i].isInitialized = false;
                _radarVisual[i].image.gameObject.SetActive(false);
            }
        }
    }
}
