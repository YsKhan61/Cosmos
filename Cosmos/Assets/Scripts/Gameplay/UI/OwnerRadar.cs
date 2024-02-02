using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Only the owner of the radar should be able to see it.
    /// Attach this to player avatar. and it will controls the RadarUI
    /// </summary>
    public class OwnerRadar : NetworkBehaviour
    {
        [SerializeField] private GameObject m_radarGO;
        [SerializeField] private OwnerRadarSystem m_radarUI;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                m_radarGO.SetActive(false);
                enabled = false;
                return;
            }

            m_radarGO.SetActive(true);
        }

        /*public void UpdateRadar(Vector3 otherPosition)
        {
            m_radarUI.UpdateRadarUI(transform.position, otherPosition);
        }*/
    }
}
