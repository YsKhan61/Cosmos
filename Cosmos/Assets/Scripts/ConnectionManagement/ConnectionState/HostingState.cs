using UnityEngine;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to a listening host. Handles incoming client connections. 
    /// When shutting down or being timed out, transitions to the Offline state.
    /// </summary>
    internal class HostingState : OnlineState
    {
        const int k_MaxConnectedPayload = 1024;

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}

