using UnityEngine;


namespace Cosmos.ConnectionManagement
{

    /// <summary>
    /// Connection state corresponding to when a client is attempting to connect to a server.
    /// Starts the client when entering. If successful, transitions to the ClientConnected state.
    /// If not, transitions to the Offline state.
    /// </summary>
    internal class ClientConnectingState : OnlineState
    {
        public override void Enter()
        {
            
        }

        public override void Exit() { }
    }
}

