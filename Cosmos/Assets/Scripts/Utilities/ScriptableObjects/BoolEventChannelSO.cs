using UnityEngine;

namespace Cosmos.Utilities
{
    [CreateAssetMenu(fileName = "New Bool Event Channel", menuName = "ScriptableObjects/Event Channels/BoolEventChannelSO")]
    public class BoolEventChannelSO : ScriptableObject
    {
        public delegate void BoolEvent(bool value);
        public event BoolEvent OnEventRaised;

        private bool m_Value;
        public bool Value => m_Value;

        public void RaiseEvent(bool value)
        {
            m_Value = value;
            OnEventRaised?.Invoke(value);
        }
    }
}
