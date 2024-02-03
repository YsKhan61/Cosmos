using Cosmos.Gameplay.Utilities;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Only runs on the OWNER
    /// This script takes the NetworkList of RadarNetworkData List and updates the radar UI.
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
            // [SerializeField] internal Color imageColor;
            [SerializeField] internal bool isInitialized;           // Once the radar visual is initialized, it will be set to true.
        }

        [SerializeField] 
        private RadarVisual[] _radarVisual;                         // each avatar in the game will have unique radar visual.

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

        /*[SerializeField]
        ServerRadarDataSO m_ServerRadarData;*/

        [SerializeField]
        ServerRadarSystem m_serverRadarSystem;

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

        /*private void Start()
        {
            m_serverRadarSystem.n_RadarNetworkDatas.OnListChanged += OnRadarNetworkDatasChanged;

            InitializeRadarVisuals();
        }*/

        /*public override void OnNetworkDespawn()
        {
            m_serverRadarSystem.n_RadarNetworkDatas.OnListChanged -= OnRadarNetworkDatasChanged;
        }*/

        /*private void OnRadarNetworkDatasChanged(NetworkListEvent<ServerRadarSystem.RadarNetworkData> changeEvent)
        {
            InitializeRadarVisuals();
        }*/
        

        private void LateUpdate()
        {
            UpdateRadarUI();
        }

        /// <summary>
        /// When a client joins the game, or leaves the game, the radar visuals will be reinitialized.
        /// This is called from ServerRadarSystem
        /// </summary>

        [ClientRpc]
        public void ReInitialize_ClientRpc()
        {
            InitializeRadarVisuals();
        }

        /*[ClientRpc]
        public void UpdatePosition_ClientRpc(ulong clientId, Vector3 avatarPosition)
        {
            if (!IsOwner)
                return;

            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (_radarVisual[i].clientId == clientId)
                {
                    _radarVisual[i].avatarPosition = avatarPosition;
                    break;
                }
            }
        }*/

        private void InitializeRadarVisuals()
        {
            Reset();

            int index = 0;

            foreach (var data in m_serverRadarSystem.n_RadarNetworkDatas)
            {
                if (data.ClientId == NetworkManager.Singleton.LocalClientId)
                    continue;

                _radarVisual[index].clientId = data.ClientId;
                _radarVisual[index].avatarPosition = data.AvatarPosition;
                // _radarVisual[index].imageColor = data.ImageColor;
                _radarVisual[index].image.color = data.ImageColor;
                _radarVisual[index].isInitialized = true;
                _radarVisual[index].image.gameObject.SetActive(true);

                index++;
            }
        }

        /// <summary>
        /// Letting each owner call this method to update the radar UI.
        /// Server could call this but that would be bad for performance.
        /// Every frame, server had to call this method for every owner.
        /// </summary>
        private void UpdateRadarUI()
        {
            UpdateAvatarPositions();

            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (!_radarVisual[i].isInitialized)
                    continue;

                Vector3 vectorFromThisShipToOtherShip = GetAvatarPosition(_radarVisual[i].clientId) - _ourTransform.position;
                vectorFromThisShipToOtherShip = _radarCanvasTransform.InverseTransformDirection(vectorFromThisShipToOtherShip);
                Vector3 projectedVector = Vector3.ProjectOnPlane(vectorFromThisShipToOtherShip, _radarCanvasTransform.forward);
                _radarVisual[i].image.rectTransform.anchoredPosition = new Vector2(
                        Mathf.Clamp((projectedVector * 100).x, -_clampPosValue, _clampPosValue),
                        Mathf.Clamp((projectedVector * 100).y, -_clampPosValue, _clampPosValue));

                _radarVisual[i].image.sprite = vectorFromThisShipToOtherShip.z < 0 ? _backSprite : _frontSprite;
            }
        }

        private Vector3 GetAvatarPosition(ulong clientId)
        {
            for (int i = 0, length = m_serverRadarSystem.n_RadarNetworkDatas.Count; i < length; i++)
            {
                if (m_serverRadarSystem.n_RadarNetworkDatas[i].ClientId == clientId)
                {
                    return m_serverRadarSystem.n_RadarNetworkDatas[i].AvatarPosition;
                }
            }

            Debug.LogError("OwnerRadarSystem: GetAvatarPosition: ClientId not found in the list.");
            return Vector3.zero;
        }

        private void UpdateAvatarPositions()
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
        }

        private void Reset()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                _radarVisual[i].clientId = ulong.MaxValue;
                _radarVisual[i].avatarPosition = Vector3.zero;
                // _radarVisual[i].imageColor = Color.black;
                _radarVisual[i].image.color = Color.black;
                _radarVisual[i].isInitialized = false;
                _radarVisual[i].image.gameObject.SetActive(false);
            }
        }
    }
}
