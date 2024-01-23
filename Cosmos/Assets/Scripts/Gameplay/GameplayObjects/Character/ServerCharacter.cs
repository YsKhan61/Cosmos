using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Contains all NetworkVariables, RPCs and server-side logic of a character.
    /// This class was separated in two to keep client and server context self contained. 
    /// This way you don't have to continuously ask yourself if code is running client or server side.
    /// </summary>
    [RequireComponent(typeof(NetworkAvatarGuidState))]
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField]
        CharacterClassSO m_CharacterClass;

        public CharacterClassSO CharacterClass
        {
            get
            {
                if (m_CharacterClass == null)
                {
                    m_CharacterClass = m_State.RegisteredAvatar.CharacterClass;
                }

                return m_CharacterClass;
            }

            set => m_CharacterClass = value;
        }

        NetworkAvatarGuidState m_State;
    }

}
