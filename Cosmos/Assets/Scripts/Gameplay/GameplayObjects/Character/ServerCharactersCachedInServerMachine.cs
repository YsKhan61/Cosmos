using Cosmos.ConnectionManagement;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Attached to the player-avatar's prefab, this maintains a list of active ServerCharacter objects in the server machine.
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
    [RequireComponent(typeof(ServerCharacter))]
    public class ServerCharactersCachedInServerMachine : NetworkBehaviour
    {
        static List<ServerCharacter> s_ActivePlayers = new List<ServerCharacter>();

        [SerializeField]
        ServerCharacter m_CachedServerCharacter;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                s_ActivePlayers.Add(m_CachedServerCharacter);
            }
            else
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            s_ActivePlayers.Remove(m_CachedServerCharacter);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                Transform movementTransform = m_CachedServerCharacter.transform;
                SessionPlayerData? sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    SessionPlayerData playerData = sessionPlayerData.Value;
                    playerData.PlayerPosition = movementTransform.position;
                    playerData.PlayerRotation = movementTransform.rotation;
                    playerData.HasCharacterSpawned = true;
                    SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                }
            }
        }

        /// <summary>
        /// Returns a list of all active players' ServerCharacters. Treat the list as read-only!
        /// The list will be empty on the client.
        /// </summary>
        public static List<ServerCharacter> GetAllServerCharacters()
        {
            return s_ActivePlayers;
        }

        /// <summary>
        /// Returns the ServerCharacter owned by a specific client. Always returns null on the client.
        /// </summary>
        /// <param name="ownerClientId"></param>
        /// <returns>The ServerCharacter owned by the client, or null if no ServerCharacter is found</returns>
        public static ServerCharacter GetServerCharacter(ulong ownerClientId)
        {
            foreach (var serverCharacter in s_ActivePlayers)
            {
                if (serverCharacter.OwnerClientId == ownerClientId)
                {
                    return serverCharacter;
                }
            }
            return null;
        }
    }

}
