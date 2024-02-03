using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.GameplayObjects.Character;
using Cosmos.Gameplay.GameState;
using Cosmos.Gameplay.Utilities;
using Cosmos.Infrastructure;
using System;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Cosmos.Gameplay
{

    /// <summary>
    /// Only runs on the SERVER.
    /// This script takes data from ConnectedClients of NetworkManager and updates the RadarNetworkData List
    /// </summary>
    public class ServerRadarSystem : NetworkBehaviour// MonoBehaviour
    {
        [Serializable]
        public struct RadarNetworkData : INetworkSerializable, IEquatable<RadarNetworkData>
        {
            public ulong ClientId;                  // ClientId of the player over the network.
            public Vector3 AvatarPosition;          // Position of avatar in world space that will be used to calculate the position on the radar.
            public Color ImageColor;                // Color of the avatar on the radar.

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref AvatarPosition);
                serializer.SerializeValue(ref ImageColor);
            }

            public bool Equals(RadarNetworkData other)
            {
                return ClientId == other.ClientId;
            }
        }

        [SerializeField]
        OwnerRadarSystem m_OwnerRadarSystem;

        public NetworkList<RadarNetworkData> n_RadarNetworkDatas {get; private set;}

        /// <summary>
        /// Cache the NetworkAvatarGuidState of all the connected clients.
        /// </summary>
        Dictionary<ulong, NetworkAvatarGuidState> avatars;

        /*[SerializeField]
        ServerRadarDataSO m_ServerRadarData;*/

        /*[Inject]
        UpdateRunner m_updateRunner;*/

        private void Awake()
        {
            n_RadarNetworkDatas = new NetworkList<RadarNetworkData>(
                values: default,
                readPerm: NetworkVariableReadPermission.Owner,
                writePerm: NetworkVariableWritePermission.Server
            );

            avatars = new Dictionary<ulong, NetworkAvatarGuidState>();
        }

        public override void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            InitializeDatasRelatedToRadars();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        private void LateUpdate()
        {
            UpdateAvatarPositionInRadarData();
        }

        private void UpdateAvatarPositionInRadarData()
        {
            if (n_RadarNetworkDatas == null)
            {
                Debug.Log("ServerRadarSystem: UpdateAvatarPositionInRadarData: n_RadarNetworkDatas is null");
                return;
            }

            if (avatars == null)
            {
                Debug.Log("ServerRadarSystem: UpdateAvatarPositionInRadarData: avatars is null");
                return;
            }

            for (int i = 0, length = n_RadarNetworkDatas.Count; i < length; i++)
            {
                RadarNetworkData data = n_RadarNetworkDatas[i];
                data.AvatarPosition = avatars[data.ClientId].transform.position;
                n_RadarNetworkDatas[i] = data;
            }
        }

        /*private void OnDestroy()
        {
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
        }*/

        /*private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientIdsCompleted, List<ulong> clientIdsTimedOut)
        {
            if (loadSceneMode == LoadSceneMode.Single)
            {
                InitializeDatasRelatedToRadars();

                // Tell UpdateRunner to update visual in every owner through ClientRpc.
            }
        }*/

        private void OnClientConnected(ulong clientId)
        {
            AddClientToRadarDataList(clientId);

            m_OwnerRadarSystem.ReInitialize_ClientRpc();
        }

        private void OnClientDisconnect(ulong clientId)
        {
            RemoveClientFromRadarDataList(clientId);

            m_OwnerRadarSystem.ReInitialize_ClientRpc();
        }

        private void InitializeDatasRelatedToRadars()
        {
            /*if (n_RadarNetworkDatas == null)
            {
                Debug.Log("ServerRadarSystem: InitializeDatasRelatedToRadars: n_RadarNetworkDatas is null");
                return;
            }*/

            foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
            {
                AddClientToRadarDataList(kvp.Key);
            }

            m_OwnerRadarSystem.ReInitialize_ClientRpc();
        }

        private void AddClientToRadarDataList(ulong clientId)
        {
            NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            if (playerObject.gameObject.TryGetComponent(out NetworkAvatarGuidState avatar))
            {
                avatars.Add(clientId, avatar);

                n_RadarNetworkDatas.Add(new RadarNetworkData
                {
                    ClientId = clientId,
                    AvatarPosition = Vector3.zero,
                    ImageColor = avatar.RegisteredAvatar.raderVisualColor
                });
            }
        }

        private void RemoveClientFromRadarDataList(ulong clientId)
        {
            for (int i = 0, length = n_RadarNetworkDatas.Count; i < length; i++)
            {
                if (n_RadarNetworkDatas[i].ClientId == clientId)
                {
                    n_RadarNetworkDatas.RemoveAt(i);
                    avatars.Remove(clientId);
                    break;
                }
            }
        }
    }
}
