using UnityEngine;
using UnityEngine.EventSystems;

namespace Cosmos.Utilities
{
    public class TouchButtonUIHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        BoolEventChannelSO m_BoolEventChannel;

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            m_BoolEventChannel.RaiseEvent(true);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            m_BoolEventChannel.RaiseEvent(false);
        }
    }
}
