using Cosmos.Gameplay.GameplayObjects.Character;
using System;
using Unity.Netcode;
using UnityEngine;

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

        /*[SerializeField]
        OwnerRadarSystem m_OwnerRadarSystem;*/

        public NetworkList<RadarNetworkData> n_RadarNetworkDatas {get; private set;}

        /// <summary>
        /// Cache the NetworkAvatarGuidState of all the connected clients.
        /// </summary>
        // Dictionary<ulong, Transform> avatarTransforms;

        private void Awake()
        {
            n_RadarNetworkDatas = new NetworkList<RadarNetworkData>(
                values: default,
                readPerm: NetworkVariableReadPermission.Owner,
                writePerm: NetworkVariableWritePermission.Server
            );

            // avatarTransforms = new Dictionary<ulong, Transform>();
        }

        public override void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void Start()
        {
            Debug.Log("ServerRadarSystem: Start: InitializeDatasRelatedToRadars() called. " + gameObject.name);
            InitializeDatasRelatedToRadars();
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

            /*if (avatarTransforms == null)
            {
                Debug.Log("ServerRadarSystem: UpdateAvatarPositionInRadarData: avatars is null");
                return;
            }*/

            for (int i = 0, length = n_RadarNetworkDatas.Count; i < length; i++)
            {
                RadarNetworkData data = n_RadarNetworkDatas[i];
                // data.AvatarPosition = avatarTransforms[data.ClientId].transform.position;
                data.AvatarPosition = ServerCharactersCachedInServerMachine.GetServerCharacter(data.ClientId).transform.position;
                n_RadarNetworkDatas[i] = data;
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            AddClientToRadarDataList(clientId);

            NotifyOwnersAboutRadarDataChange();
        }

        private void OnClientDisconnect(ulong clientId)
        {
            RemoveClientFromRadarDataList(clientId);

            NotifyOwnersAboutRadarDataChange();
        }

        private void InitializeDatasRelatedToRadars()
        {
            /*foreach (ServerCharacter serverCharacter in ServerCharactersCachingInServer.GetAllServerCharacters())
            {
                AddClientToRadarDataList(OwnerClientId);
            }*/

            /*foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
            {
                AddClientToRadarDataList(kvp.Key);
            }*/

            AddClientToRadarDataList(OwnerClientId);

            NotifyOwnersAboutRadarDataChange();
        }

        private void AddClientToRadarDataList(ulong clientId)
        {
            /*NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            if (playerObject.gameObject.TryGetComponent(out NetworkAvatarGuidState avatar))
            {
                avatars.Add(clientId, avatar);

                n_RadarNetworkDatas.Add(new RadarNetworkData
                {
                    ClientId = clientId,
                    AvatarPosition = Vector3.zero,
                    ImageColor = avatar.RegisteredAvatar.raderVisualColor
                });
            }*/

            ServerCharacter serverCharacter = ServerCharactersCachedInServerMachine.GetServerCharacter(clientId);

            if (serverCharacter == null)
            {
                Debug.LogError("ServerRadarSystem: AddClientToRadarDataList: ServerCharacter not found!");
                return;
            }

            // avatarTransforms.Add(serverCharacter.OwnerClientId, serverCharacter.transform); 

            n_RadarNetworkDatas.Add(new RadarNetworkData
            {
                ClientId = clientId,
                AvatarPosition = serverCharacter.transform.position,
                ImageColor = serverCharacter.NetworkAvatarGuidState.RegisteredAvatar.radarVisualColor
            });

            Debug.Log($"ServerRadarSystem: ClientId: {clientId}, Transform GameObject {serverCharacter.transform.name} added!");
        }

        private void RemoveClientFromRadarDataList(ulong clientId)
        {
            for (int i = 0, length = n_RadarNetworkDatas.Count; i < length; i++)
            {
                if (n_RadarNetworkDatas[i].ClientId == clientId)
                {
                    n_RadarNetworkDatas.RemoveAt(i);
                    // avatarTransforms.Remove(clientId);
                    break;
                }
            }
        }

        /// <summary>
        /// Have to notfy all owners if any change happens in the radar data.
        /// </summary>
        /// <param name="clientId"></param>
        private void NotifyOwnersAboutRadarDataChange()
        {
            /*foreach (ServerCharacter serverCharacter in ServerCharactersCachingInServer.GetAllServerCharacters())
            {
                serverCharacter.OwnerRadarSystem.InitializeRadarVisuals_ClientRpc();
            }*/
        }
    }
}
