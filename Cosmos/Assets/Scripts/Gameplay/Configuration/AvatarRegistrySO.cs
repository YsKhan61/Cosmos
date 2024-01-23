using System;
using UnityEngine;

namespace Cosmos.Gameplay.Configuration
{
    /// <summary>
    /// This ScriptableObject will be the container for all possible Avatars inside Cosmos.
    /// <see cref="Avatar"/>
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarRegistryData", menuName = "ScriptableObjects/ConfigurationData/AvatarRegistrySO")]
    public sealed class AvatarRegistrySO : ScriptableObject
    {
        [SerializeField]
        AvatarSO[] m_Avatars;

        public bool TryGetAvatar(Guid guid, out AvatarSO avatarValue)
        {
            avatarValue = Array.Find(m_Avatars, avatar => avatar.Guid == guid);

            return avatarValue != null;
        }

        public AvatarSO GetRandomAvatar()
        {
            if (m_Avatars == null || m_Avatars.Length == 0)
            {
                return null;
            }

            return m_Avatars[UnityEngine.Random.Range(0, m_Avatars.Length)];
        }
    }
}