using Unity.Netcode;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects
{
    /// <summary>
    /// Various Space Objects like Asteroids, Meteors, Planets etc can randomly move in the space.
    /// </remarks>
    public class ServerSpaceObjectMovement : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            
        }
    }
}

