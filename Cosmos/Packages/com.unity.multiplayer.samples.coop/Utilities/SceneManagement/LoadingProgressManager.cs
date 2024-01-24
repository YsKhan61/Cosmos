using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities
{
    /// <summary>
    /// Contains data on scene loading progress for the local instance and remote instances.
    /// </summary>
    public class LoadingProgressManager : NetworkBehaviour
    {
        [SerializeField]
        GameObject m_ProgressTrackerPrefab;

        /// <summary>
        /// Dictionary containing references to the NetworkedLoadingProgessTrackers that contain the loading progress of
        /// each client. Keys are ClientIds.
        /// </summary>
        public Dictionary<ulong, NetworkedLoadingProgressTracker> ProgressTrackersDictionary { get; } = new Dictionary<ulong, NetworkedLoadingProgressTracker>();

        /// <summary>
        /// This is the AsyncOperation of the current load operation. This property should be set each time a new
        /// loading operation begins.
        /// </summary>
        public AsyncOperation LocalLoadOperation
        {
            set
            {
                m_IsLoading = true;
                LocalProgress = 0;
                m_LocalLoadOperation = value;
            }
        }

        AsyncOperation m_LocalLoadOperation;

        float m_LocalProgress;

        bool m_IsLoading;

        /// <summary>
        /// This event is invoked each time the dictionary of progress trackers is updated (if one is removed or added, for example.)
        /// </summary>
        public event Action onTrackersUpdated;

        /// <summary>
        /// The current loading progress for the local client. Handled by a local field if not in a networked session,
        /// or by a progress tracker from the dictionary.
        /// </summary>
        public float LocalProgress
        {
            get => IsSpawned && ProgressTrackersDictionary.ContainsKey(NetworkManager.LocalClientId) ?
                ProgressTrackersDictionary[NetworkManager.LocalClientId].n_Progress.Value : m_LocalProgress;
            private set
            {
                if (IsSpawned && ProgressTrackersDictionary.ContainsKey(NetworkManager.LocalClientId) && ProgressTrackersDictionary[NetworkManager.LocalClientId].IsSpawned)
                {
                    ProgressTrackersDictionary[NetworkManager.LocalClientId].n_Progress.Value = value;
                }
                else
                {
                    m_LocalProgress = value;
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += AddTracker;
                NetworkManager.OnClientDisconnectCallback += RemoveTracker;
                // AddTracker(NetworkManager.LocalClientId);                    - LoadingTracker was spawning twice
            }
        }
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= AddTracker;
                NetworkManager.OnClientDisconnectCallback -= RemoveTracker;
            }
            ProgressTrackersDictionary.Clear();
            onTrackersUpdated?.Invoke();
        }

        void Update()
        {
            if (m_LocalLoadOperation != null && m_IsLoading)
            {
                if (m_LocalLoadOperation.isDone)
                {
                    m_IsLoading = false;
                    LocalProgress = 1;
                }
                else
                {
                    LocalProgress = m_LocalLoadOperation.progress;
                }
            }
        }

        [ClientRpc]
        void UpdateTrackersClientRpc()
        {
            if (!IsHost)
            {
                ProgressTrackersDictionary.Clear();
                foreach (NetworkedLoadingProgressTracker tracker in FindObjectsOfType<NetworkedLoadingProgressTracker>())
                {
                    // If a tracker is despawned but not destroyed yet, don't add it
                    if (tracker.IsSpawned)
                    {
                        ProgressTrackersDictionary[tracker.OwnerClientId] = tracker;
                        if (tracker.OwnerClientId == NetworkManager.LocalClientId)
                        {
                            LocalProgress = Mathf.Max(m_LocalProgress, LocalProgress);
                        }
                    }
                }
            }
            onTrackersUpdated?.Invoke();
        }

        void AddTracker(ulong clientId)
        {
            if (IsServer)
            {
                GameObject trackerGO = Instantiate(m_ProgressTrackerPrefab);
                NetworkObject networkObject = trackerGO.GetComponent<NetworkObject>();
                networkObject.SpawnWithOwnership(clientId);
                ProgressTrackersDictionary[clientId] = trackerGO.GetComponent<NetworkedLoadingProgressTracker>();
                UpdateTrackersClientRpc();
            }
        }

        void RemoveTracker(ulong clientId)
        {
            if (IsServer)
            {
                if (ProgressTrackersDictionary.ContainsKey(clientId))
                {
                    NetworkedLoadingProgressTracker tracker = ProgressTrackersDictionary[clientId];
                    ProgressTrackersDictionary.Remove(clientId);
                    tracker.NetworkObject.Despawn();
                    UpdateTrackersClientRpc();
                }
            }
        }
    }
}
