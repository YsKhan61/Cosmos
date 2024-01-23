using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// NetworkBehaviour component to send/receive GUIDs from server to clients.
    /// </summary>
    public class NetworkAvatarGuidState : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkVariable<NetworkGuid> n_AvatarNetworkGuid = new NetworkVariable<NetworkGuid>();

        [SerializeField]
        AvatarRegistrySO m_AvatarRegistry;

        AvatarSO m_Avatar;

        public AvatarSO RegisteredAvatar
        {
            get
            {
                if (m_Avatar == null)
                {
                    RegisterAvatar(n_AvatarNetworkGuid.Value.ToGuid());
                }

                return m_Avatar;
            }
        }

        public void SetRandomAvatar()
        {
            n_AvatarNetworkGuid.Value = m_AvatarRegistry.GetRandomAvatar().Guid.ToNetworkGuid();
        }

        void RegisterAvatar(Guid guid)
        {
            if (guid.Equals(Guid.Empty))
            {
                // not a valid Guid
                return;
            }

            // based on the Guid received, Avatar is fetched from AvatarRegistry
            if (!m_AvatarRegistry.TryGetAvatar(guid, out AvatarSO avatar))
            {
                Debug.LogError("Avatar not found!");
                return;
            }

            if (m_Avatar != null)
            {
                // already set, this is an idempotent call, we don't want to Instantiate twice
                return;
            }

            m_Avatar = avatar;

            if (TryGetComponent<ServerCharacter>(out ServerCharacter serverCharacter))
            {
                serverCharacter.CharacterClass = avatar.CharacterClass;
            }
        }
    }

}
