using Cosmos.ConnectionManagement;
using Cosmos.Gameplay.GameplayObjects;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cosmos.Gameplay.GameplayObjects.Character;
using Cosmos.Utilities;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
using Cosmos.ApplicationLifecycle.Messages;
using Cosmos.Infrastructure;
using Cosmos.Gameplay.UI;
using System.Linq;

namespace Cosmos.Gameplay.GameState
{
    /// <summary>
    /// Server specialization of core Cosmos game logic.
    /// </summary>
    [RequireComponent(typeof(NetcodeHooks))]
    public class ServerCosmosState : GameStateBehaviour
    {
        [SerializeField]
        NetcodeHooks m_NetcodeHooks;

        [SerializeField]
        [Tooltip("Make sure this is included in the NetworkManager's list of prefabs!")]
        private NetworkObject m_PlayerPrefab;

        [SerializeField]
        [Tooltip("A collection of locations for spawning players")]
        private Transform[] m_PlayerSpawnPoints;

        private List<Transform> m_PlayerSpawnPointsList = null;

        public override GameState ActiveState { get { return GameState.Cosmos; } }

        /// <summary>
        /// Has the ServerBossRoomState already hit its initial spawn? (i.e. spawned players following load from character select).
        /// </summary>
        public bool InitialSpawnDone { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            // NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            // NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;

            // <SessionPlayerData>.Instance.OnSessionStarted();
        }

        void OnNetworkDespawn()
        {
            // NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            // NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;
        }

        protected override void OnDestroy()
        {
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_NetcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }

            base.OnDestroy();
        }

        void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (!InitialSpawnDone && loadSceneMode == LoadSceneMode.Single)
            {
                InitialSpawnDone = true;
                foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
                {
                    SpawnPlayer(kvp.Key, false);
                }
            }
        }

        void SpawnPlayer(ulong clientId, bool lateJoin)
        {
            Transform spawnPoint;

            if (m_PlayerSpawnPointsList == null || m_PlayerSpawnPointsList.Count == 0)
            {
                m_PlayerSpawnPointsList = new List<Transform>(m_PlayerSpawnPoints);
            }

            Debug.Assert(m_PlayerSpawnPointsList.Count > 0,
                $"PlayerSpawnPoints array should have at least 1 spawn points.");

            int index = Random.Range(0, m_PlayerSpawnPointsList.Count);
            spawnPoint = m_PlayerSpawnPointsList[index];
            m_PlayerSpawnPointsList.RemoveAt(index);

            NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);

            NetworkObject newPlayer = Instantiate(m_PlayerPrefab, spawnPoint.position, spawnPoint.rotation);        // Later this can be changed to implement physics wrapper.

            var persistentPlayerExists = playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer);
            Assert.IsTrue(persistentPlayerExists,
                $"Matching persistent PersistentPlayer for client {clientId} not found!");

            // pass character type from persistent player to avatar
            var networkAvatarGuidStateExists =
                newPlayer.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);                       // NetworkAvatarGuidState

            Assert.IsTrue(networkAvatarGuidStateExists,
                $"NetworkCharacterGuidState not found on player avatar!");

            // if reconnecting, set the player's position and rotation to its previous state
            if (lateJoin)
            {
                // Do stuffs for late join
            }

            networkAvatarGuidState.n_AvatarNetworkGuid.Value =
                persistentPlayer.NetworkAvatarGuidState.n_AvatarNetworkGuid.Value;

            // pass name from persistent player to avatar
            if (newPlayer.TryGetComponent(out NetworkNameState networkNameState))                                   // NetworkNameState
            {
                networkNameState.Name.Value = persistentPlayer.NetworkNameState.Name.Value;
            }

            /*if (newPlayer.TryGetComponent(out OwnerRadarSystem radarSystem))
            {
                radarSystem.Initialize(NetworkManager.Singleton.ConnectedClientsIds.ToArray());
            }*/


            // spawn players characters with destroyWithScene = true
            newPlayer.SpawnWithOwnership(clientId, true);
        }
    }
}
