using Cosmos.UnityServices.Lobbies;
using Cosmos.Utilities;
using Unity.Multiplayer.Samples.Utilities;
using UnityEngine.SceneManagement;
using VContainer;


namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to when the NetworkManager is shut down. From this state we can transition to the 
    /// ClientConnecting state -> if starting as a client, 
    /// StartingHost state -> if starting as a host.
    /// </summary>
    internal class OfflineState : ConnectionState
    {
        private const string MAIN_MENU_SCENE_NAME = "MainMenu";

        [Inject]
        private LobbyServiceFacade _lobbyServiceFacade;

        [Inject]
        private ProfileManager _profileManager;

        [Inject]
        private LocalLobby _localLobby;

        public override void Enter()
        {
            _lobbyServiceFacade.EndTracking();
            _connectionManager.NetworkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != MAIN_MENU_SCENE_NAME)
            {
                SceneLoaderWrapper.Instance.LoadScene(MAIN_MENU_SCENE_NAME, useNetworkSceneManager: false);
            }
        }

        public override void Exit() {}

        public override void StartHostLobby(string playerName)
        {
            ConnectionMethodRelay connectionMethodRelay =
                new(_lobbyServiceFacade, _localLobby, _connectionManager, _profileManager, playerName);

            _connectionManager.ChangeState(_connectionManager._startingHostState.Configure(connectionMethodRelay));
        }

        public override void StartClientLobby(string playerName)
        {
            ConnectionMethodRelay connectionMethodRelay = 
                new(_lobbyServiceFacade, _localLobby, _connectionManager, _profileManager, playerName);

            _connectionManager._clientReconnectingState.Configure(connectionMethodRelay);
            _connectionManager.ChangeState(_connectionManager._clientConnectingState.Configure(connectionMethodRelay));
        }

        public override void StartHostIP(string playerName, string ipAddress, int port)
        {
            ConnectionMethodIP connectionMethodIP =
                new(ipAddress, (ushort)port, _connectionManager, _profileManager, playerName);

            _connectionManager.ChangeState(_connectionManager._startingHostState.Configure(connectionMethodIP));
        }

        public override void StartClientIP(string playerName, string ipAddress, int port)
        {
            ConnectionMethodIP connectionMethodIP =
                new(ipAddress, (ushort)port, _connectionManager, _profileManager, playerName);

            _connectionManager._clientReconnectingState.Configure(connectionMethodIP);
            _connectionManager.ChangeState(_connectionManager._clientConnectingState.Configure(connectionMethodIP));
        }
    }
}

