
using UnityEngine;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to a host starting up. Starts the host when entering the state.
    /// If successful, transitions to the Hosting state. If not, transitions back to the Offline state.
    /// </summary>
    internal class StartingHostState : OnlineState
    {
        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }
    }
}

