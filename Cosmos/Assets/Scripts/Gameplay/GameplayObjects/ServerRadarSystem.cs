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
    /// This script takes the position of the other ship and the position of this ship 
    /// and calculates the position of the other ship on the radar.
    /// </summary>
    public class ServerRadarSystem : NetworkBehaviour// MonoBehaviour
    {
        public NetworkList<RadarNetworkData> n_RadarNetworkDatas {get; private set;}

        /*[SerializeField]
        ServerRadarDataSO m_ServerRadarData;*/

        /*[SerializeField]
        NetcodeHooks m_NetcodeHooks;*/

        /*[Inject]
        UpdateRunner m_updateRunner;*/

        private void Awake()
        {
            /*m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;*/

            n_RadarNetworkDatas = new NetworkList<RadarNetworkData>(
                values: default,
                readPerm: NetworkVariableReadPermission.Owner,
                writePerm: NetworkVariableWritePermission.Server
            );
        }

        public override void OnNetworkSpawn() // void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            InitializeDatasRelatedToRadars();

            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }

        public override void OnNetworkDespawn() // void OnNetworkDespawn()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
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

        private void OnClientDisconnect(ulong clientId)
        {
            RemoveClientFromRadarDataList(clientId);
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
                NetworkObject playerObject = kvp.Value.PlayerObject;

                if (playerObject.gameObject.TryGetComponent(out NetworkAvatarGuidState avatar))
                {
                    n_RadarNetworkDatas.Add(new RadarNetworkData
                    {
                        ClientId = kvp.Key,
                        AvatarPosition = Vector3.zero,
                        ImageColor = avatar.RegisteredAvatar.raderVisualColor
                    });
                }
            }
        }

        private void RemoveClientFromRadarDataList(ulong clientId)
        {
            for (int i = 0, length = n_RadarNetworkDatas.Count; i < length; i++)
            {
                if (n_RadarNetworkDatas[i].ClientId == clientId)
                {
                    n_RadarNetworkDatas.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
