using Cosmos.Infrastructure;
using System;
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

        public Task RetrieveAndPublishLobbyListAsync()
        {
            throw new NotImplementedException();
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

        private void PublishError(LobbyServiceException e)
        {
            throw new NotImplementedException();
        }

        private void LeaveLobbyAsync()
        {
            
        }

        private void DeleteLobbyAsync()
        {
            
        }

        private void DoLobbyHeartbeat(float obj)
        {
            
        }

        private void SubscribeToJoinedLobbyAsync()
        {
            throw new NotImplementedException();
        }

        private void UnsubscribeToJoinedLobbyAsync()
        {
            
        }
    }
}