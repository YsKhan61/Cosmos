using UnityEngine;
using UnityEngine.SceneManagement;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to when the NetworkManager is shut down. From this state we can transition to the 
    /// ClientConnecting state -> if starting as a client, 
    /// StartingHost state -> if starting as a host.
    /// </summary>
    internal class OfflineState : ConnectionState
    {
        const string k_MainMenuSceneName = "MainMenu";

        public override void Enter()
        {
            _connectionManager.NetworkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != k_MainMenuSceneName)
            {
                // Load the main menu scene if we are not in it already
            }
        }

        public override void Exit() { }
    }
}

