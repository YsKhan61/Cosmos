using UnityEngine;

namespace Cosmos.Utilities
{
    [CreateAssetMenu(fileName = "New Void Event Channel", menuName = "ScriptableObjects/Event Channels/VoidEventChannelSO")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public delegate void VoidEvent();
        public event VoidEvent OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }
}
