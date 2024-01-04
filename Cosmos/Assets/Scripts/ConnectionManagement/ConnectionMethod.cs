using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using Cosmos.Utilities;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Cosmos.ConnectionManagement
{
    /// <summary>
    /// ConnectionMethod contains all setup needed to setup NGO to be ready to start a connection, either host or client side.
    /// Please override this abstract class to add a new transport or way of connecting.
    /// </summary>
    public abstract class ConnectionMethodBase
    {
        protected const string DTLS_CONNECTION_TYPE = "dtls";

        protected ConnectionManager _connectionManager;
        readonly ProfileManager _profileManager;
        protected readonly string _playerName;
        
        public ConnectionMethodBase(ConnectionManager connectionManager, ProfileManager profileManager, string playerName)
        {
            _connectionManager = connectionManager;
            _profileManager = profileManager;
            _playerName = playerName;
        }

        /// <summary>
        /// Setup the host connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract Task SetupHostConnectionAsync();

        /// <summary>
        /// Setup the client connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract Task SetupClientConnectionAsync();

        /// <summary>
        /// Setup the client prior to reconnecting
        /// </summary>
        /// <returns></returns>
        public abstract Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync();

        protected void SetConnectionPayload(string playerId, string playerName)
        {
            string payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                playerId = playerId,
                playerName = playerName,
                isDebug = Debug.isDebugBuild
            });
        }

        /// Using authentication, this makes sure your session is assosiated with your account and not your device.
        /// This means you could reconnect from a different device for example.
        /// A playerId is also a bit more permanent than player prefs.
        /// In a browser for example, player prefs can be cleared as easily as cookies.
        /// The forked flow here is for debug purposes and to make UGS optional in Cosmos.
        /// This way we can study the sample without setting up a UGS account.
        /// It's recommended to investigate your own initialization and IsSigned flows to see if you need those checks
        /// on your own and react accordingly.
        /// We offer here the option for offline access for debug purposes, but in your own game you might want to
        /// show an error popup and ask your player to connect to the internet.
        protected string GetPlayerId()
        {
            if (Unity.Services.Core.UnityServices.State != ServicesInitializationState.Initialized)
            {
                return ClientPrefs.GetGUID() + _profileManager.Profile;
            }

            return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId
                : ClientPrefs.GetGUID() + _profileManager.Profile;
        }
    }

    /// <summary>
    /// UTP's Relay connection setup using the Lobby integration
    /// </summary>
    internal class ConnectionMethodRelay : ConnectionMethodBase
    {
        private LobbyServiceFacade _lobbyServiceFacade;
        private LocalLobby _localLobby;

        public ConnectionMethodRelay(LobbyServiceFacade lobbyServiceFacade, LocalLobby localLobby, ConnectionManager connectionManager, ProfileManager profileManager, string playerName) 
            : base(connectionManager, profileManager, playerName)
        {
            _connectionManager = connectionManager;
            _lobbyServiceFacade = lobbyServiceFacade;
            _localLobby = localLobby;
        }

        public override async Task SetupHostConnectionAsync()
        {
            Debug.Log("Setting up Unity Relay Host Connection");

            SetConnectionPayload(GetPlayerId(), _playerName);

            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(_connectionManager.MaxConnectedPlayers, region: null);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

            Debug.Log($"server: connection data: {hostAllocation.ConnectionData[0]} {hostAllocation.ConnectionData[1]}, " +
                $"allocation ID: {hostAllocation.AllocationId}, region: {hostAllocation.Region}");

            _localLobby.RelayJoinCode = joinCode;

            // next line enables lobby and relay services integration
            await _lobbyServiceFacade.UpdateLobbyDataAndUnlockAsync();
            await _lobbyServiceFacade.UpdatePlayerDataAsync(hostAllocation.AllocationIdBytes.ToString(), joinCode);

            // Setup UTP with relay connection info
            UnityTransport utp = (UnityTransport)_connectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(hostAllocation, DTLS_CONNECTION_TYPE));  // This is with DTLS enabled for a secure connection

            Debug.Log($"Created relay allocation with join code {_localLobby.RelayJoinCode}");
        }

        public override async Task SetupClientConnectionAsync()
        {
            Debug.Log("Setting up Unity Relay Client");

            SetConnectionPayload(GetPlayerId(), _playerName);

            if (_lobbyServiceFacade.CurrentUnityLobby == null)
            {
                throw new System.Exception("Trying to start relay while lobby isn't set");
            }

            Debug.Log($"Setting Unity Relay Client with join coede {_localLobby.RelayJoinCode}");

            // Create client joining allocation from join code
            JoinAllocation joinedAllocation = await RelayService.Instance.JoinAllocationAsync(_localLobby.RelayJoinCode);
            Debug.Log($"client: {joinedAllocation.ConnectionData[0]} {joinedAllocation.ConnectionData[1]}, " +
                $"host: {joinedAllocation.HostConnectionData[0]} {joinedAllocation.HostConnectionData[1]}, " +
                $"client: {joinedAllocation.AllocationId}");

            await _lobbyServiceFacade.UpdatePlayerDataAsync(joinedAllocation.AllocationId.ToString(), _localLobby.RelayJoinCode);

            // Configure UTP with allocation
            UnityTransport utp = (UnityTransport)_connectionManager.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetRelayServerData(new RelayServerData(joinedAllocation, DTLS_CONNECTION_TYPE));  // This is with DTLS enabled for a secure connection
        }

        public override async Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
            if (_lobbyServiceFacade.CurrentUnityLobby == null)
            {
                Debug.Log("Lobby does not exist anymore, stopping reconnection attempts.");
                return (false, false);
            }

            // When using Lobby with Relay, if a user is disconnected from the Relay server, the server will notify the
            // Lobby service and mark the user as disconnected, but will not remove them from the lobby.
            // Then they have some time to attempt to reconnect (defined by the "Disconnect removal time" paramter on the dashboard),
            // after which they will be removed from the lobby completely.
            // See https://docs.unity.com/lobby/reconnect-to-lobby.html
            Lobby lobby = await _lobbyServiceFacade.ReconnectToLobbyAsync();
            bool success = lobby != null;
            Debug.Log(success ? "Successfully reconnected to Lobby." : "Failed to reconnect to Lobby.");
            return (success, true); // return a success if reconnecting to lobby returns a lobby.
        }
    }
}
