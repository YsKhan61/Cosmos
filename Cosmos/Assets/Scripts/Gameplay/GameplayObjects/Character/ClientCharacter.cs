using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Client related things of a character.
    /// </summary>
    [RequireComponent(typeof(NetworkAvatarGuidState))]
    public class ClientCharacter : NetworkBehaviour
    {
        [SerializeField]
        Transform _graphicsTransform;
        public Transform GraphicsTransform => _graphicsTransform;

        NetworkAvatarGuidState m_networkAvatarGuidState;
        public NetworkAvatarGuidState NetworkAvatarGuidState => m_networkAvatarGuidState;

        void Awake()
        {
            m_networkAvatarGuidState = GetComponent<NetworkAvatarGuidState>();
            // m_ownerRadarSystem = GetComponent<OwnerRadarSystem>();
        }
    }
}
