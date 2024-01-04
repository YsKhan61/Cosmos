using Cosmos.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Cosmos.UnityServices.Lobbies
{
    /// <summary>
    /// An abstraction layer between the direct calls into the Lobby API and the outcomes you actually want
    /// </summary>
    public class LobbyServiceFacade : IDisposable, IStartable
    {
        private const float HEART_BEAT_PERIOD = 8; // the heartbeat must be rate-limited to 5 calls per 30 seconds, We'll aim for longer in case periods don't allign.

        [Inject] private LifetimeScope _parentLifetimeScope;
        [Inject] private UpdateRunner _updateRunner;
        [Inject] private LocalLobby _localLobby;
        [Inject] private LocalLobbyUser _localLobbyUser;
        [Inject] private IPublisher<UnityServiceErrorMessage> _unityServiceErrorMessagePublisher;
        [Inject] private IPublisher<LobbyListFetchedMessage> _lobbyListFetchedMessagePublisher;

        private float _heartBeatTime = 0;

        private LifetimeScope _serviceLifetimeScope;
        private LobbyAPIInterface _lobbyApiInterface;

        private RateLimitCooldown _rateLimitQuery;
        private RateLimitCooldown _rateLimitJoin;
        private RateLimitCooldown _rateLimitQuickJoin;
        private RateLimitCooldown _rateLimitHost;

        public Lobby CurrentUnityLobby { get; private set; }

        private ILobbyEvents _lobbyEvents;

        private bool _isTracking = false;

        private LobbyEventConnectionState _lobbyEventConnectionState = LobbyEventConnectionState.Unknown;

        public void Start()
        {
            _serviceLifetimeScope = _parentLifetimeScope.CreateChild(
                builder =>
                {
                    builder.Register<LobbyAPIInterface>(Lifetime.Singleton);
                });

            _lobbyApiInterface = _serviceLifetimeScope.Container.Resolve<LobbyAPIInterface>();

            _rateLimitQuery = new RateLimitCooldown(1f);
            _rateLimitJoin = new RateLimitCooldown(3f);
            _rateLimitQuickJoin = new RateLimitCooldown(10f);
            _rateLimitHost = new RateLimitCooldown(3f);
        }

        public void Dispose()
        {
            EndTracking();
            if (_serviceLifetimeScope != null)
            {
                _serviceLifetimeScope.Dispose();
                _serviceLifetimeScope = null;
            }
        }

        /// <summary>
        /// Attempt to create a new lobby and then join it.
        /// </summary>
        /// <param name="lobbyName"></param>
        /// <param name="maxPlayers"></param>
        /// <param name="isPrivate"></param>
        /// <returns></returns>
        public async Task<(bool Success, Lobby Lobby)> TryCreateLobbyAsync(string lobbyName, int maxPlayers, bool isPrivate)
        {
            if (!_rateLimitHost.CanCall)
            {
                Debug.LogWarning("Create Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                Lobby lobby = await _lobbyApiInterface.CreateLobby(AuthenticationService.Instance.PlayerId, lobbyName, maxPlayers, isPrivate,
                    _localLobbyUser.GetDataForUnityServices(), null);

                return (true, lobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _rateLimitHost.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Attempt to join an existing lobby. Will try to join via code, if code is null - will try to join via ID.
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <param name="lobbyCode"></param>
        /// <returns></returns>
        public async Task<(bool Success, Lobby Lobby)> TryJoinLobbyAsync(string lobbyId, string lobbyCode)
        {
            if (!_rateLimitJoin.CanCall ||
                (lobbyId == null && lobbyCode == null))
            {
                Debug.LogWarning("Join Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                if (!string.IsNullOrEmpty(lobbyCode))
                {
                    Lobby lobby = await _lobbyApiInterface.JoinLobbyByCode(AuthenticationService.Instance.PlayerId, lobbyCode, _localLobbyUser.GetDataForUnityServices());
                    return (true, lobby);
                }
                else
                {
                    Lobby lobby = await _lobbyApiInterface.JoinLobbyById(AuthenticationService.Instance.PlayerId, lobbyId, _localLobbyUser.GetDataForUnityServices());
                    return (true, lobby);
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _rateLimitJoin.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }

            return (false, null);
        }

        public async Task<(bool Success, Lobby Lobby)> TryQuickJoinLobbyAsync()
        {
            if (!_rateLimitQuickJoin.CanCall)
            {
                Debug.LogWarning("Quick Join Lobby hit the rate limit.");
                return (false, null);
            }

            try
            {
                Lobby lobby = await _lobbyApiInterface.QuickJoinLobby(AuthenticationService.Instance.PlayerId, _localLobbyUser.GetDataForUnityServices());
                return (true, lobby);
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _rateLimitQuickJoin.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }
            return (false, null);
        }

        public void SetRemoteLobby(Lobby lobby)
        {
            CurrentUnityLobby = lobby;
            _localLobby.ApplyRemoteData(lobby);
        }

        /// <summary>
        /// Used for getting the list of all active lobbies, without needing full into for each.
        /// </summary>
        /// <returns></returns>
        public async Task RetrieveAndPublishLobbyListAsync()
        {
            if (!_rateLimitQuery.CanCall)
            {
                Debug.LogError("Retrieve lobby list hit the rate limit. Will try again soon...");
                return;
            }

            try
            {
                QueryResponse response = await _lobbyApiInterface.QueryAllLobbies();
                _lobbyListFetchedMessagePublisher.Publish(new LobbyListFetchedMessage(LocalLobby.CreateLocalLobbies(response)));
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _rateLimitQuery.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }
        }

        /// <summary>
        /// Initiates tracking of joined lobby's events. The host also starts sending heartbeat pings here.
        /// </summary>
        public void BeginTracking()
        {
            if (!_isTracking)
            {
                _isTracking = true;
                SubscribeToJoinedLobbyAsync();

                // Only the host sends heartbeat pings to the service to keep the lobby alive
                if (_localLobbyUser.IsHost)
                {
                    _heartBeatTime = 0;
                    _updateRunner.Subscribe(DoLobbyHeartbeat, 1.5f);
                }    
            }    
        }
        

        /// <summary>
        /// Emds tracking of joined lobby's events and leaves or deletes the lobby. The host also stops sending heartbeat pings here.
        /// </summary>
        public void EndTracking()
        {
            if (_isTracking)
            {
                _isTracking = false;
                UnsubscribeToJoinedLobbyAsync();

                // Only the host sends heartbeats pings to the service to keep the lobby alive
                if (_localLobbyUser.IsHost)
                {
                    _updateRunner.Unsubscribe(DoLobbyHeartbeat);
                }
            }

            if (CurrentUnityLobby != null)
            {
                if (_localLobbyUser.IsHost)
                {
                    DeleteLobbyAsync();
                }
                else
                {
                    LeaveLobbyAsync();
                }
            }
        }

        /// <summary>
        /// Attempt to update the set of key-value pairs associated with a given lobby and unlocks it so clients can see it.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateLobbyDataAndUnlockAsync()
        {
            if (_rateLimitQuery.CanCall)
            {
                return;
            }

            Dictionary<string, DataObject> localData = _localLobby.GetDataForUnityServices();

            Dictionary<string, DataObject> currentData = CurrentUnityLobby.Data;
            if (currentData == null)
            {
                currentData = new Dictionary<string, DataObject>();
            }

            foreach (KeyValuePair<string, DataObject> newData in localData)
            {
                if (currentData.ContainsKey(newData.Key))
                {
                    currentData[newData.Key] = newData.Value;
                }
                else
                {
                    currentData.Add(newData.Key, newData.Value);
                }
            }

            try
            {
                Lobby result = await _lobbyApiInterface.UpdateLobby(CurrentUnityLobby.Id, currentData, shouldLock: false);
                if (result != null)
                {
                    CurrentUnityLobby = result;
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _rateLimitQuery.PutOnCooldown();
                }
                else
                {
                    PublishError(e);
                }
            }
        }

        /// <summary>
        /// Attempt to push a set of key-value pairs associated with the local player which will overwrite any existing data
        /// for these keys. Lobby can be provided info about Relay (or any other remote allocation) so it can add automatic disconnect handling.
        /// </summary>
        /// <returns></returns>
        public async Task UpdatePlayerDataAsync(string allocationId, string connectionInfo)
        {
            if (!_rateLimitQuery.CanCall)
            {
                return;
            }

            try
            {
                Lobby lobby = await _lobbyApiInterface.UpdatePlayer(CurrentUnityLobby.Id, AuthenticationService.Instance.PlayerId, _localLobbyUser.GetDataForUnityServices(), allocationId, connectionInfo);

                if (lobby != null)
                {
                    CurrentUnityLobby = lobby; // Store the most up-to-date lobby now since we have it, instead of waiting for the next heartbeat.
                }
            }
            catch (LobbyServiceException e)
            {
                if (e.Reason == LobbyExceptionReason.RateLimited)
                {
                    _rateLimitQuery.PutOnCooldown();
                }
                // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                else if (e.Reason != LobbyExceptionReason.LobbyNotFound && !_localLobbyUser.IsHost)
                {
                    PublishError(e);
                }
            }
        }

        public async Task<Lobby> ReconnectToLobbyAsync()
        {
            try
            {
                return await _lobbyApiInterface.ReconnectToLobby(_localLobby.LobbyID);
            }
            catch (LobbyServiceException e)
            {
                // If Lobby is not found and if we are not the host, it has already been deleted.
                // No need to publish the error here.
                if (e.Reason != LobbyExceptionReason.LobbyNotFound && !_localLobbyUser.IsHost)
                {
                    PublishError(e);
                }
            }

            return null;
        }

        public async void RemovePlayerFromLobbyAsync(string uasId)
        {
            if (_localLobbyUser.IsHost)
            {
                try
                {
                    await _lobbyApiInterface.RemovePlayerFromLobby(uasId, _localLobby.LobbyID);
                }
                catch (LobbyServiceException e)
                {
                    PublishError(e);
                }
            }
            else
            {
                Debug.LogError("Only the host can remove other players from the lobby.");
            }
        }

        /// <summary>
        /// Attempt to leave a lobby
        /// </summary>
        private async void LeaveLobbyAsync()
        {
            string uasId  = AuthenticationService.Instance.PlayerId;
            try
            {
                await _lobbyApiInterface.RemovePlayerFromLobby(uasId, _localLobby.LobbyID);
            }
            catch (LobbyServiceException e)
            {
                // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                if (e.Reason != LobbyExceptionReason.LobbyNotFound && !_localLobbyUser.IsHost)
                {
                    PublishError(e);
                }
            }
            finally
            {
                ResetLobby();
            }
        }

        private void ResetLobby()
        {
            CurrentUnityLobby = null;
            if (_localLobbyUser != null)
            {
                _localLobbyUser.ResetState();
            }
            if (_localLobby != null)
            {
                _localLobby.Reset(_localLobbyUser);
            }

            // no need to disconnect Netcode, it should already be handled by Netcode's Callback to disconnect
        }

        private async void DeleteLobbyAsync()
        {
            if (_localLobbyUser.IsHost)
            {
                try
                {
                    await _lobbyApiInterface.DeleteLobby(_localLobby.LobbyID);
                }
                catch (LobbyServiceException e)
                {
                    PublishError(e);
                }
                finally
                {
                    ResetLobby();
                }
            }
            else
            {
                Debug.LogError("Only the host can delete a lobby.");
            }
        }

        private void DoLobbyHeartbeat(float dt)
        {
            _heartBeatTime += dt;
            if (_heartBeatTime > HEART_BEAT_PERIOD)
            {
                _heartBeatTime -= HEART_BEAT_PERIOD;
                try
                {
                    _lobbyApiInterface.SendHeartbeatPing(CurrentUnityLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    // If Lobby is not found and if we are not the host, it has already been deleted. No need to publish the error here.
                    if (e.Reason != LobbyExceptionReason.LobbyNotFound && !_localLobbyUser.IsHost)
                    {
                        PublishError(e);
                    }
                }
            }
        }

        private async void SubscribeToJoinedLobbyAsync()
        {
            LobbyEventCallbacks lobbyEventCallbacks = new();
            lobbyEventCallbacks.LobbyChanged += OnLobbyChanges;
            lobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
            lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
            // The LobbyEventCallbacks object created here will now be managed by the Lobby SDK.
            // The callbacks will be unsubscribed from when we call UnsubscribeAsync on the ILobbyEvents object we receive and store here.
            _lobbyEvents = await _lobbyApiInterface.SubscribeToLobby(_localLobby.LobbyID, lobbyEventCallbacks);
        }

        private async void UnsubscribeToJoinedLobbyAsync()
        {
            if (_lobbyEvents != null && _lobbyEventConnectionState != LobbyEventConnectionState.Unsubscribed)
            {
#if UNITY_EDITOR
                try
                {
                    await _lobbyEvents.UnsubscribeAsync();
                }
                catch (WebSocketException e)
                {
                    // This exception occurs in the editor when exiting play mode without first leaving the lobby.
                    // This is because Wire closes the websocket internally when exiting playmode in the editor.
                    Debug.Log(e.Message);
                }
#else
                await _lobbyEvents.UnsubscribeAsync();  
#endif
            }
        }

        private void OnLobbyChanges(ILobbyChanges changes)
        {
            throw new NotImplementedException();
        }

        private void OnKickedFromLobby()
        {
            throw new NotImplementedException();
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
        {
            throw new NotImplementedException();
        }

        

        private void PublishError(LobbyServiceException e)
        {
            string reason = e.InnerException == null ? e.Message : $"{e.Message} ({e.InnerException.Message})"; // Lobby error type, then HTTP error type
            _unityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Lobby Error", reason, UnityServiceErrorMessage.Service.Lobby, e));
        }
    }
}