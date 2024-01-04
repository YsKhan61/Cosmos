using Cosmos.Utilities;
using System;
using System.Collections.Generic;
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

    public struct ReconnectMessage
    {
        public int CurrentAttempt;
        public int MaxAttempts;

        public ReconnectMessage(int currentAttempt, int maxAttempts)
        {
            CurrentAttempt = currentAttempt;
            MaxAttempts = maxAttempts;
        }
    }

    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
        public FixedPlayerName PlayerName;
    }

    [Serializable]
    public class ConnectionPayload
    {
        public string playerId;
        public string playerName;
        public bool isDebug;
    }


    /// <summary>
    /// This state machine handles connection through the NetworkManager. It is responsible for listening to
    /// NetworkManager callbacks and other outside calls and redirecting them to the current ConnectionState.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        [Inject]
        NetworkManager _networkManager;
        public NetworkManager NetworkManager => _networkManager;

        [Inject]
        IObjectResolver _objectResolver;

        [SerializeField]
        int _reconnectAttempts = 2;
        public int ReconnectAttempts => _reconnectAttempts;

        internal readonly OfflineState _offlineState = new OfflineState();
        internal readonly ClientConnectingState _clientConnectingState = new ClientConnectingState();
        internal readonly ClientConnectedState _clientConnectedState = new ClientConnectedState();
        internal readonly ClientReconnectingState _clientReconnectingState = new ClientReconnectingState();
        internal readonly StartingHostState _startingHostState = new StartingHostState();
        internal readonly HostingState _hostingState = new HostingState();

        private ConnectionState _currentState;

        public int MaxConnectedPlayers = 4;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            List<ConnectionState> connectionStates = new() { _offlineState, _clientConnectedState, _clientReconnectingState, _startingHostState, _hostingState };
            foreach (ConnectionState connectionState in connectionStates)
            {
                _objectResolver.Inject(connectionState);
            }

            _currentState = _offlineState;

            NetworkManager.OnServerStarted += OnServerStarted;
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.ConnectionApprovalCallback += ConnectionApprovalCallback;
            NetworkManager.OnTransportFailure += OnTransportFailure;
            NetworkManager.OnServerStopped += OnServerStopped;
        }

        private void OnDestroy()
        {
            NetworkManager.OnServerStarted -= OnServerStarted;
            NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.ConnectionApprovalCallback -= ConnectionApprovalCallback;
            NetworkManager.OnTransportFailure -= OnTransportFailure;
            NetworkManager.OnServerStopped -= OnServerStopped;
        }

        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {_currentState.GetType().Name} to {nextState.GetType().Name}.");

            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = nextState;
            _currentState.Enter();
        }


        public void StartHostLobby(string playerName)
        {
            _currentState.StartHostLobby(playerName);
        }

        public void StartClientLobby(string playerName)
        {
            _currentState.StartClientLobby(playerName);
        }

        public void StartHostIp(string playerName, string ip, int portNumber)
        {
            _currentState.StartHostIP(playerName, ip, portNumber);
        }

        public void StartClientIP(string text, string ip, int portNumber)
        {
            _currentState.StartClientIP(text, ip, portNumber);
        }

        public void RequestShutdown()
        {
            _currentState.OnUserRequestedShutdown();
        }

        private void OnServerStarted()
        {
            _currentState.OnServerStarted();
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            _currentState.OnClientConnected(clientId);
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            _currentState.OnClientDisconnect(clientId);
        }

        private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            _currentState.ApprovalCheck(request, response);
        }

        private void OnTransportFailure()
        {
            _currentState.OnTransportFailure();
        }

        private void OnServerStopped(bool _) // we don't need this parameter as the ConnectionState already carries the relevant information
        {
            _currentState.OnServerStopped();
        }
    }
}


