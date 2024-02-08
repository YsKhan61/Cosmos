using Cosmos.Infrastructure;
using Cosmos.Utilities;
using UnityEngine;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// This is attached to Cosmos - scene game object, and it's referenced to ServerChatSystem component
    /// This will listen to the event raised by UI Button, and will send the message to ServerChatSystem through another event channel.
    /// </summary>
    public class MessageBridgeVR : MonoBehaviour
    {
        [SerializeField]
        ServerChatSystem m_ServerChatSystem;

        [Header("Listens to")]

        [SerializeField, Tooltip("Event channel from UI to Server")]
        NetworkChatMessageEventChannelSO m_NetworkChatMessageEventChannel_UIToServer;

        [Header("Sends to")]

        [SerializeField, Tooltip("Event channel from Server to UI")]
        NetworkChatMessageEventChannelSO m_NetworkChatMessageEventChannel_ServerToUI;

        DisposableGroup m_Subscriptions;

        [Inject]
        void InjectDependencies(
            ISubscriber<NetworkChatMessage> networkClientChatSubscriber
        )
        {
            m_Subscriptions = new DisposableGroup();
            m_Subscriptions.Add(networkClientChatSubscriber.Subscribe(OnChatMessageReceivedFromServer));
        }

        private void OnChatMessageReceivedFromServer(NetworkChatMessage message)
        {
            m_NetworkChatMessageEventChannel_ServerToUI.RaiseEvent(message);
        }

        private void Start()
        {
            m_NetworkChatMessageEventChannel_UIToServer.OnEventRaised += OnChatMessageReceivedFromUI;
        }

        private void OnDestroy()
        {
            m_NetworkChatMessageEventChannel_UIToServer.OnEventRaised -= OnChatMessageReceivedFromUI;
        }

        public void OnChatMessageReceivedFromUI(NetworkChatMessage message)
        {
            m_ServerChatSystem.SendChatMessageServerRpc(message);
        }
    }
}
