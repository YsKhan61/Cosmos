using UnityEngine;

namespace Cosmos.ScriptableObjects.EventChannels
{
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
