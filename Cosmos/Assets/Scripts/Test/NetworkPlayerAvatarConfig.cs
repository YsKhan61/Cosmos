using Cosmos.Gameplay.GameplayObjects.Character;
using Cosmos.PlatformConfiguration;
using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Test
{
    /// <summary>
    /// A temporary script to activate the respective camera for the player
    /// according to the platform they are playing on and if they are the owner of the avatar.
    /// Also activates or deactivates the spaceship control scripts based on if it is a owner or not. (Network Object)
    /// </summary>
    public class NetworkPlayerAvatarConfig : NetworkBehaviour
    {
        [SerializeField] private GameObject m_flatScreenCamera;
        [SerializeField] private GameObject m_vrCamera;

        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private ThrottleMovement m_throttleMovement;
        [SerializeField] private ControlMovement m_controlMovement;

        [SerializeField] private PlatformConfigSO m_platformConfig;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                m_rigidbody.isKinematic = true;
                m_throttleMovement.enabled = false;
                m_controlMovement.enabled = false;
                return;
            }

            m_rigidbody.isKinematic = false;

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
