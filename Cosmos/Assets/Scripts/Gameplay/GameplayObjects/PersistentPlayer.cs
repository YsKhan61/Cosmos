using Cosmos.ConnectionManagement;
using Cosmos.Gameplay.GameplayObjects.Character;
using Cosmos.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects
{
    /// <summary>
    /// NetworkBehaviour that representes a player connection and is the "Default player prefab"
    /// inside NGO's NetworkManager.
    /// This NetworkBehaviour will contain several other NetworkBehaviours that should persist
    /// throughout the duration of this connection, meaning it will persist between scenes.
    /// </summary>
    /// <remarks>
    /// It is not necessary to explicitely mark this as Netcode will handle 
    /// migrating this player object between scene loads.
    /// </remarks>
    [RequireComponent(typeof(NetworkObject))]
    public class PersistentPlayer : NetworkBehaviour
    {
        [SerializeField]
        private PersistentPlayersRuntimeCollectionSO _persistentPlayerRuntimeCollection;

        [SerializeField]
        private NetworkNameState _networkNameState;

        [SerializeField]
        NetworkAvatarGuidState m_NetworkAvatarGuidState;

        public NetworkNameState NetworkNameState => _networkNameState;

        public NetworkAvatarGuidState NetworkAvatarGuidState => m_NetworkAvatarGuidState;

        public override void OnNetworkSpawn()
        {
            gameObject.name = "PersistentPlayer_" + OwnerClientId;

            // Note that this is done here on NetworkSpawn in case this NetworkBehaviour's properties are accessed
            // when this element is added to the runtime collection. If this was done in OnEnable() there is a chance
            // that OwnerClientID could be its default value (0).
            _persistentPlayerRuntimeCollection.Add(this);

            if (IsServer)
            {
                var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    var playerData = sessionPlayerData.Value;
                    _networkNameState.Name.Value = playerData.PlayerName;
                    if (playerData.HasCharacterSpawned)
                    {
                        m_NetworkAvatarGuidState.n_AvatarNetworkGuid.Value = playerData.AvatarNetworkGuid;
                    }
                    else
                    {
                        m_NetworkAvatarGuidState.SetRandomAvatar();
                        playerData.AvatarNetworkGuid = m_NetworkAvatarGuidState.n_AvatarNetworkGuid.Value;
                        SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemovePersistentPlayer();
        }

        public override void OnNetworkDespawn()
        {
            RemovePersistentPlayer();
        }

        private void RemovePersistentPlayer()
        {
            _persistentPlayerRuntimeCollection.Remove(this);
            if (IsServer)
            {
                var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    var playerData = sessionPlayerData.Value;
                    playerData.PlayerName = _networkNameState.Name.Value;
                    playerData.AvatarNetworkGuid = m_NetworkAvatarGuidState.n_AvatarNetworkGuid.Value;
                    SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                }
            }
        }
    }
}

