
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Client-side component that awaits a state change on an avatar's Guid, and fetches matching Avatar from the
    /// AvatarRegistry, if possible. Once fetched, the Graphics GameObject is spawned.
    /// </summary>
    public class ClientAvatarGuidHandler : NetworkBehaviour
    {
        [SerializeField, Tooltip("Avatar will be spawned under this parent")]
        Transform m_avatarParent;

        [SerializeField]
        NetworkAvatarGuidState m_NetworkAvatarGuidState;

        [SerializeField] private Camera m_camera;            // Temporary for testing

        public event Action<GameObject> AvatarGraphicsSpawned;

        public override void OnNetworkSpawn()
        {
            // Temporary for testing
            if (IsLocalPlayer)
            {
                m_camera.gameObject.SetActive(true);
            }
            else
            {
                m_camera.gameObject.SetActive(false);
            }

            if (IsClient)
            {
                InstantiateAvatar();
            }
        }

        void InstantiateAvatar()
        {
            if (m_avatarParent.childCount > 0)
            {
                // we may receive a NetworkVariable's OnValueChanged callback more than once as a client
                // this makes sure we don't spawn a duplicate graphics GameObject
                return;
            }

            // spawn avatar graphics GameObject
            Instantiate(m_NetworkAvatarGuidState.RegisteredAvatar.Graphics, m_avatarParent);

            AvatarGraphicsSpawned?.Invoke(m_avatarParent.gameObject);
        }
    }

}
