using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Server related things of a character.
    /// </summary>
    [RequireComponent(typeof(NetworkAvatarGuidState))]
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField]
        CharacterClassSO m_CharacterClass;          // dont know use of it

        public CharacterClassSO CharacterClass
        {
            get
            {
                if (m_CharacterClass == null)
                {
                    m_CharacterClass = m_networkAvatarGuidState.RegisteredAvatar.CharacterClass;
                }

                return m_CharacterClass;
            }

            set => m_CharacterClass = value;
        }

        NetworkAvatarGuidState m_networkAvatarGuidState;
        public NetworkAvatarGuidState NetworkAvatarGuidState => m_networkAvatarGuidState;

        void Awake()
        {
            m_networkAvatarGuidState = GetComponent<NetworkAvatarGuidState>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }
        }
    }
}
