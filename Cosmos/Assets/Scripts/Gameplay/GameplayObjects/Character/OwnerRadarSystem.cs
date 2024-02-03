using Cosmos.Gameplay.Utilities;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /*[Serializable]
    public struct RadarNetworkData : INetworkSerializable
    {
        public Vector3 AvatarPosition;
        public Color ImageColor;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AvatarPosition);
            serializer.SerializeValue(ref ImageColor);
        }
    }*/

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

            // ClientId, AvatarPosition, ImageColor, IsInitialized are made serializable for Debug visual only.
            [SerializeField] internal ulong clientId;
            [SerializeField] internal Vector3 avatarPosition;
            [SerializeField] internal Color imageColor;
            [SerializeField] internal bool isInitialized;           // Once the radar visual is initialized, it will be set to true.
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

        private void Start()
        {
            // m_serverRadarSystem.n_RadarNetworkDatas.OnListChanged += OnRadarNetworkDatasChanged;

            InitializeRadarVisuals();
        }

        public override void OnNetworkDespawn()
        {
            // m_serverRadarSystem.n_RadarNetworkDatas.OnListChanged -= OnRadarNetworkDatasChanged;
        }

        private void OnRadarNetworkDatasChanged(NetworkListEvent<RadarNetworkData> changeEvent)
        {
            InitializeRadarVisuals();
        }

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
                _radarVisual[index].imageColor = data.ImageColor;
                _radarVisual[index].isInitialized = true;
                _radarVisual[index].image.gameObject.SetActive(true);

                index++;
            }
        }

        private void LateUpdate()
        {
            UpdateRadarUI();
        }

        
        public void UpdateRadarUI()
        {
            for (int i = 0, length = _radarVisual.Length; i < length; i++)
            {
                if (!_radarVisual[i].isInitialized)
                    continue;

                Vector3 vectorFromThisShipToOtherShip = _radarVisual[i].avatarPosition - _ourTransform.position;
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
