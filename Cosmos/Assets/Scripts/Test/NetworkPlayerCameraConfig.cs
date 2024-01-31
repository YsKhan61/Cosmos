using Cosmos.PlatformConfiguration;
using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Test
{
    /// <summary>
    /// A temporary script to activate the respective camera for the player
    /// according to the platform they are playing on and if they are the owner of the avatar.
    /// </summary>
    public class NetworkPlayerCameraConfig : NetworkBehaviour
    {
        [SerializeField] private GameObject m_flatScreenCamera;
        [SerializeField] private GameObject m_vrCamera;

        [SerializeField] private PlatformConfigSO m_platformConfig;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }

            if (m_platformConfig.Platform == PlatformType.FlatScreen)
            {
                m_flatScreenCamera.SetActive(true);
                m_vrCamera.SetActive(false);
            }
            else if (m_platformConfig.Platform == PlatformType.VR)
            {
                m_vrCamera.SetActive(true);
                m_flatScreenCamera.SetActive(false);
            }
        }
    }

}
