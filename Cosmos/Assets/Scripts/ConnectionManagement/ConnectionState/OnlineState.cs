using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// Base class representing an online connection state.
    /// </summary>
    internal abstract class OnlineState : ConnectionState
    {
        public override void OnUserRequestedShutdown()
        {
            
        }

        public override void OnTransportFailure()
        {
            
        }
    }
}