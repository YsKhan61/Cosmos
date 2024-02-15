using Cosmos.ConnectionManagement;
using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Attached to the player-avatar's prefab, this maintains a list of active ClientCharacter objects in the OwnerClient's machine.
    /// </summary>
    /// <remarks>
    /// This is an optimization. In server code you can already get a list of players' ServerCharacters by
    /// iterating over the connected clients and calling GetComponent() on their NetworkObject. But we need
    /// to iterate over all players quite often, in radar updates,
    /// and all those GetComponent() calls add up! So this optimization lets us iterate without calling
    /// GetComponent(). 
    /// 
    /// This will be refactored with a ScriptableObject-based approach on player collection.
    /// </remarks>
    [RequireComponent(typeof(ClientCharacter))]
    public class ClientCharactersCachedInClientMachine : NetworkBehaviour
    {
        static List<ClientCharacter> s_ActivePlayers = new();

        [SerializeField]
        ClientCharacter m_CachedServerCharacter;

        public override void OnNetworkSpawn()
        {
            s_ActivePlayers.Add(m_CachedServerCharacter);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            s_ActivePlayers.Remove(m_CachedServerCharacter);
        }

        /// <summary>
        /// Returns a list of all active players' ClientCharacters. Treat the list as read-only!
        /// The list will be empty on the client.
        /// </summary>
        public static List<ClientCharacter> GetAllClientCharacters()
        {
            return s_ActivePlayers;
        }

        /// <summary>
        /// Returns the ServerCharacter owned by a specific client. Always returns null on the client.
        /// </summary>
        /// <param name="ownerClientId"></param>
        /// <returns>The ServerCharacter owned by the client, or null if no ServerCharacter is found</returns>
        public static ClientCharacter GetClientCharacter(ulong ownerClientId)
        {
            foreach (var clientCharacter in s_ActivePlayers)
            {
                if (clientCharacter.OwnerClientId == ownerClientId)
                {
                    return clientCharacter;
                }
            }
            return null;
        }
    }
}
