using Cosmos.Utilities;
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
        public NetworkNameState NetworkNameState => _networkNameState;

        public override void OnNetworkSpawn()
        {
            gameObject.name = "PersistentPlayer_" + OwnerClientId;

            // Note that this is done here on NetworkSpawn in case this NetworkBehaviour's properties are accessed
            // when this element is added to the runtime collection. If this was done in OnEnable() there is a chance
            // that OwnerClientID could be its default value (0).
            _persistentPlayerRuntimeCollection.Add(this);
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
        }
    }
}

