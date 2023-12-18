using UnityEngine;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to a connected client.
    /// When being disconnected, transitions to the ClientReconnecting state if no reason is given,
    /// or to the Offline state.
    /// </summary>
    internal class ClientConnectedState : OnlineState
    {
        public override void Enter()
        {
                
        }
        
        public override void Exit() { }
    }

}

