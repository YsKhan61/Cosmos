
using Cosmos.ConnectionManagement;
using Cosmos.Infrastructure;
using Cosmos.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;
using VContainer;


namespace Cosmos.Gameplay
{
    /// <summary>
    /// Send and receive chat messages on the server.
    /// </summary>
    public class ServerChatSystem : NetworkBehaviour
    {

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }
        }

        [Inject]
        IPublisher<NetworkChatMessage> m_networkClientChatPublisher;

        [ServerRpc(RequireOwnership = false)]
        public void SendChatMessageServerRpc(NetworkChatMessage message)
        {
            m_networkClientChatPublisher.Publish(message);
        }
    }
}

