using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cosmos.ConnectionManagement
{

    /// <summary>
    /// Connection state corresponding to a client attempting to reconnect to a server.
    /// It will try to reconnect a number of times defined by the ConnectionManager's reconnectAttempts property.
    /// If it succeeds, it will transition to the ClientConnected state. If not, it will transition to the Offline state.
    /// If given a disconnect reason first, depending on the reason given, may not try to reconnect again and transition
    /// directly to the Offline state.
    /// </summary>
    internal class ClientReconnectingState : OnlineState
    {
        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}

