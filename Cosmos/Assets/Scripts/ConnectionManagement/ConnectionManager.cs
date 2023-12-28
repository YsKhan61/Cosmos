using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;


namespace Cosmos.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,                    // client successfully connected. This may also be a successful reconnect.
        ServerFull,                 // can't join, server is already at capacity.
        LoggedInAgain,              // logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect,    // user requested to disconnect from the server.
        GenericDisconnect,          // Server disconnected, but no specific reason was given.
        Reconnecting,               // client lost connection to the server, but is attempting to reconnect.
        IncompatibleBuildType,      // client and server are running different build types (debug vs release)
        HostEndedSession,           // host ended the session intentionally.
        StartHostFailed,            // host failed to start.
        StartClientFailed,          // client failed to start or connect to the server.
    }

    /// <summary>
    /// This state machine handles connection through the NetworkManager. It is responsible for listening to
    /// NetworkManager callbacks and other outside calls and redirecting them to the current ConnectionState.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        ConnectionState _currentState;

        [Inject]
        NetworkManager _networkManager;
        public NetworkManager NetworkManager => _networkManager;

        [SerializeField]
        int _reconnectAttempts = 2;

        public int MaxConnectedPlayers = 4;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void StartHostLobby(string playerName)
        {
            _currentState.StartHostLobby(playerName);
        }

        public void StartClientLobby(string playerName)
        {
            _currentState.StartClientLobby(playerName);
        }

        public void StartHostIp(string text, string ip, int portNumber)
        {
            _currentState.StartHostIP(text, ip, portNumber);
        }

        public void RequestShutdown()
        {
            _currentState.OnUserRequestedShutdown();
        }

        public void StartClientIP(string text, string ip, int portNumber)
        {
            throw new NotImplementedException();
        }
    }
}


