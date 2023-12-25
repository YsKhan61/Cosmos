using Cosmos.ConnectionManagement;
using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    public class  LobbyUIMediator : MonoBehaviour
    {
        private const string DEFAULT_LOBBY_NAME = "no-name";

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private LobbyJoiningUI _lobbyJoiningUI;
        [SerializeField] private LobbyCreationUI _lobbyCreationUI;
        [SerializeField] private UITinter _joinToggleHighlight;
        [SerializeField] private UITinter _joinToggleTabBlocker;
        [SerializeField] private UITinter _createToggleHighlight;
        [SerializeField] private UITinter _createToggleTabBlocker;
        [SerializeField] private TextMeshProUGUI _playerNameLabel;
        [SerializeField] private GameObject _loadingSpinner;

        private AuthenticationServiceFacade _authenticationServiceFacade;
        private LobbyServiceFacade _lobbyServiceFacade;
        private LocalLobbyUser _localLobbyUser;
        private LocalLobby _localLobby;
        private NameGenerationDataSO _nameGenerationData;
        private ConnectionManager _connectionManager;
        ISubscriber<ConnectStatus> _connectStatusSubscriber;

        private void OnDestroy()
        {
            if (_connectStatusSubscriber != null)
            {
                _connectStatusSubscriber.Unsubscribe(OnConnectStatusChanged);
            }
        }

        [Inject]
        private void InjectDependenciesAndInitialize(
            AuthenticationServiceFacade authenticationServiceFacade,
            LobbyServiceFacade lobbyServiceFacade,
            LocalLobbyUser localLobbyUser,
            LocalLobby localLobby,
            NameGenerationDataSO nameGenerationData,
            ISubscriber<ConnectStatus> connectStatusSubscriber,
            ConnectionManager connectionManager)
        {
            _authenticationServiceFacade = authenticationServiceFacade;
            _lobbyServiceFacade = lobbyServiceFacade;
            _localLobbyUser = localLobbyUser;
            _localLobby = localLobby;
            _nameGenerationData = nameGenerationData;
            _connectStatusSubscriber = connectStatusSubscriber;
            _connectionManager = connectionManager;

            RegenerateName();

            _connectStatusSubscriber.Subscribe(OnConnectStatusChanged);
        }

        // Lobby and Relay calls done from UI

        public async void CreateLobbyRequest(string lobbyName, bool isPrivate)
        {
            // before sending request to lobby service, populate an empty lobby name, if necessary
            if (string.IsNullOrEmpty(lobbyName))
            {
                lobbyName = DEFAULT_LOBBY_NAME;
            }

            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            (bool Success, Lobby Lobby) lobbyCreationAttempt = 
                await _lobbyServiceFacade.TryCreateLobbyAsync(lobbyName, _connectionManager.MaxConnectedPlayers, isPrivate);

            if (lobbyCreationAttempt.Success)
            {
                _localLobbyUser.IsHost = true;
                _lobbyServiceFacade.SetRemoteLobby(lobbyCreationAttempt.Lobby);

#if UNITY_EDITOR
                Debug.Log($"Created lobby with ID: {_localLobby.LobbyID} and code {_localLobby.LobbyCode}");
#endif
                _connectionManager.StartHostLobby(_localLobbyUser.DisplayName);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        public async void QueryLobbiesRequest(bool blockUI)
        {
            if (Unity.Services.Core.UnityServices.State != Unity.Services.Core.ServicesInitializationState.Initialized)
            {
                return;
            }

            if (blockUI)
            {
                BlockUIWhileLoadingIsInProgress();
            }

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (blockUI && !playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            await _lobbyServiceFacade.RetrieveAndPublishLobbyListAsync();

            if (blockUI)
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        public async void JoinLobbyWithCodeRequest(string lobbyCode)
        {
            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            (bool Success, Lobby Lobby) lobbyJoinAttempt = await _lobbyServiceFacade.TryJoinLobbyAsync(null, lobbyCode);

            if (lobbyJoinAttempt.Success)
            {
                OnJoinedLobby(lobbyJoinAttempt.Lobby);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        public async void JoinLobbyRequest(LocalLobby localLobby)
        {
            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            (bool Success, Lobby Lobby) lobbyJoinAttempt = await _lobbyServiceFacade.TryJoinLobbyAsync(localLobby.LobbyID, localLobby.LobbyCode);

            if (lobbyJoinAttempt.Success)
            {
                OnJoinedLobby(lobbyJoinAttempt.Lobby);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        public async void QuickJoinRequest()
        {
            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            (bool Success, Lobby Lobby) lobbyJoinAttempt = await _lobbyServiceFacade.TryQuickJoinLobbyAsync();

            if (lobbyJoinAttempt.Success)
            {
                OnJoinedLobby(lobbyJoinAttempt.Lobby);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        public void ToggleCreateLobbyUI()
        {
            _lobbyJoiningUI.Hide();
            _lobbyCreationUI.Show();
            _joinToggleHighlight.SetToColor(0);
            _joinToggleTabBlocker.SetToColor(0);
            _createToggleHighlight.SetToColor(1);
            _createToggleTabBlocker.SetToColor(1);
        }

        public void ToggleJoinLobbyUI()
        {
            _lobbyJoiningUI.Show();
            _lobbyCreationUI.Hide();
            _joinToggleHighlight.SetToColor(1);
            _joinToggleTabBlocker.SetToColor(1);
            _createToggleHighlight.SetToColor(0);
            _createToggleTabBlocker.SetToColor(0);
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _lobbyCreationUI.Hide();
            _lobbyJoiningUI.Hide();
        }

        public void RegenerateName()
        {
            _localLobbyUser.DisplayName = _nameGenerationData.GenerateName();
            _playerNameLabel.text = _localLobbyUser.DisplayName;
        }

        private void OnConnectStatusChanged(ConnectStatus status)
        {
            if (status is ConnectStatus.GenericDisconnect or ConnectStatus.StartClientFailed)
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        private void BlockUIWhileLoadingIsInProgress()
        {
            _canvasGroup.interactable = false;
            _loadingSpinner.SetActive(true);
        }

        private void UnblockUIAfterLoadingIsComplete()
        {
            // this callback can happen after we've already switched to a different scene
            // in that case the canvas group would be null
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = true;
                _loadingSpinner.SetActive(false);
            }
        }

        private void OnJoinedLobby(Lobby remoteLobby)
        {
            _lobbyServiceFacade.SetRemoteLobby(remoteLobby);

#if UNITY_EDITOR
            Debug.Log($"Joined lobby with ID: {_localLobby.LobbyID} and Internal Relay join code {_localLobby.RelayJoinCode}");
            _connectionManager.StartClientLobby(_localLobbyUser.DisplayName);
#endif
        }
    }
}
