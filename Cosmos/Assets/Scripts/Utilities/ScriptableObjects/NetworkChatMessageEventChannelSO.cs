using UnityEngine;

namespace Cosmos.Utilities
{
    [CreateAssetMenu(fileName = "New NetworkChatMessage Event Channel", menuName = "ScriptableObjects/Event Channels/NetworkChatMessageEventChannelSO")]
    public class NetworkChatMessageEventChannelSO : ScriptableObject
    {
        public delegate void NetworkChatMessageEvent(NetworkChatMessage message);
        public event NetworkChatMessageEvent OnEventRaised;

        public void RaiseEvent(NetworkChatMessage message)
        {
            OnEventRaised?.Invoke(message);
        }
    }
}
