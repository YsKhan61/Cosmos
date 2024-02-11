﻿using Cosmos.ApplicationLifecycle.Messages;
using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.UI;
using Cosmos.Infrastructure;
using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Cosmos.Gameplay.GameState
{
    public enum AccountType
    {
        UnityPlayerAccount,
        GuestAccount,
    }

    /// <summary>
    /// Game logic that runs when sitting at the MainMenu. This is likely to be "nothing", as no game has been started.
    /// But it is nonetheless important to have a game state, as the GameStateBehaviour system requires that all scenes have states.
    /// </summary>
    /// <remarks>
    /// OnNetworkSpawn() won't ever run, because there is no network connection at the main menu screen.
    /// Fortunately we know you are a client, because all players are client when sitting at the main menu screen.
    /// </remarks>
    public class ClientMainMenuState : GameStateBehaviour
    {
        [SerializeField]
        private NameGenerationDataSO _nameGenerationData;

        [SerializeField]
        private SignInUIMediator _signInUIMediator;

        [SerializeField]
        private StartMenuUIMediator _startMenuUIMediator;

        [SerializeField]
        private LobbyUIMediator _lobbyUIMediator;

        [SerializeField]
        private IPUIMediator _ipUIMediator;

        [SerializeField]
        private GameObject _signInSpinner;

        private AuthenticationServiceFacade _authServiceFacade;
        ISubscriber<QuitApplicationMessage> _quitApplicationMessageSubscriber;
        private LocalLobbyUser _localLobbyUser;
        private LocalLobby _localLobby;

        public override GameState ActiveState => GameState.MainMenu;

        private AccountType _accountType;
        public AccountType AccountType => _accountType;

        protected override void Awake()
        {
            base.Awake();

            _lobbyUIMediator.Hide();
            _signInSpinner.SetActive(false);

            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                OnSignInFailed();
                return;
            }
        }

        protected override void OnDestroy()
        {
            // Application.wantsToQuit -= OnApplicationWantsToQuit;
            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(_nameGenerationData);
            builder.RegisterComponent(_lobbyUIMediator);
            builder.RegisterComponent(_ipUIMediator);
        }

        [Inject]
        private void AddDependencies(
            AuthenticationServiceFacade authServiceFacade,
            LocalLobbyUser localLobbyUser,
            LocalLobby localLobby)
            // ProfileManager profileManager)
            // ISubscriber<QuitApplicationMessage> quitApplicationMessageSubscriber)
        {
            _authServiceFacade = authServiceFacade;
            _localLobbyUser = localLobbyUser;
            _localLobby = localLobby;
            /*InitializationOptions options = _authServiceFacade.GenerateAuthenticationInitOptions();
            _ = _authServiceFacade.InitializeToUnityServicesAsync(options);*/
            _ = _authServiceFacade.InitializeToUnityServicesAsync();
            _authServiceFacade.SubscribeToSignedInEvent(OnAuthSignIn);

            Application.wantsToQuit += OnApplicationWantsToQuit;
        }

        /// <summary>
        /// Called from the On Edit End event of the Name Display UI(InputField).
        /// </summary>
        public async void SavePlayerName(string name)
        {
            if (name == _localLobbyUser.PlayerName)
                return;

            await _authServiceFacade.UpdatePlayerNameAsync(name);

            // Updating LocalLobbyUser and LocalLobby
            _localLobby.RemoveUser(_localLobbyUser);
            UpdateLocalLobbyUser();
            _localLobby.AddUser(_localLobbyUser);
        }

        public void OnLobbyStartButtonClicked()
        {
            _lobbyUIMediator.ToggleJoinLobbyUI();
            _lobbyUIMediator.Show();
        }

        public void OnDirectIPButtonClicked()
        {
            _lobbyUIMediator.Hide();
            _ipUIMediator.Show();
        }

        public async void TrySignIn(AccountType accountType)
        {
            _signInSpinner.SetActive(true);
            _accountType = accountType;

            try
            {
                switch (_accountType)
                {
                    case AccountType.UnityPlayerAccount:
                        await _authServiceFacade.SignInWithUnityAsync();
                        break;
                    case AccountType.GuestAccount:
                        await _authServiceFacade.SignInAnonymously();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(accountType), accountType, null);
                }
            }
            catch (Exception)
            {
                OnSignInFailed();
            }
        }

        public void SignOut()
        {
            TryAuthSignOut();
            Debug.Log("ClientMainMenuState: Player Signed out!");
            OnAuthSignedOut();
        }

        private void TryAuthSignOut()
        {
            _authServiceFacade.SignOutFromAuthService(true);

            switch (_accountType)
            {
                case AccountType.UnityPlayerAccount:
                    _authServiceFacade.SignOutFromPlayerAccountService();
                    break;
                case AccountType.GuestAccount:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAuthSignIn()
        {
            _signInSpinner.SetActive(false);
            _signInUIMediator.HidePanel();

            _startMenuUIMediator.ConfigureStartMenuAfterSignIn(_authServiceFacade.GetPlayerName());

            Debug.Log($"Signed in. Unity Player ID {_authServiceFacade.GetPlayerId()}");
            Debug.Log($"Signed in. Unity Player Name {_authServiceFacade.GetPlayerName()}");

            UpdateLocalLobbyUser();

            // The local lobby user object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already
            // when that happens.
            _localLobby.AddUser(_localLobbyUser);

        }

        private void OnAuthSignedOut()
        {
            _signInSpinner.SetActive(false);
            _startMenuUIMediator.ShowLobbyButtonTooltip();
            _startMenuUIMediator.HidePanel();
            _signInUIMediator.ShowPanel();
        }

        private void OnSignInFailed()
        {
            if (_signInSpinner)
            {
                _signInSpinner.SetActive(false);
            }
        }

        /*private async void OnProfileChanged()
        {
            _lobbyButton.interactable = false;
            _signInSpinner.SetActive(true);

            // await _authServiceFacade.SwitchProfileAndResignInAsync(_profileManager.ProfileName);
            await _authServiceFacade.UpdatePlayerNameAsync(_profileManager.ProfileName);

            // Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Signed in. Unity Player Name {AuthenticationService.Instance.PlayerName}");

            _lobbyButton.interactable = true;
            _signInSpinner.SetActive(false);

            // Updating LocalLobbyUser and LocalLobby
            _localLobby.RemoveUser(_localLobbyUser);
            UpdateLocalLobbyUser();
            _localLobby.AddUser(_localLobbyUser);

            _playerNameInputField.text = _profileManager.ProfileName;
        }*/

        private void UpdateLocalLobbyUser()
        {
            _localLobbyUser.ID = _authServiceFacade.GetPlayerId();

            string playerName = _authServiceFacade.GetPlayerName();

            // trim the player name from '#' character
            int hashIndex = playerName.IndexOf('#');
            if (hashIndex != -1)
            {
                playerName = playerName[..hashIndex];
            }

            _localLobbyUser.PlayerName = playerName[..Mathf.Min(10, playerName.Length)];
        }

        private bool OnApplicationWantsToQuit()
        {
            Application.wantsToQuit -= OnApplicationWantsToQuit;
            TryAuthSignOut();
            _authServiceFacade.UnsubscribeFromSignedInEvent();
            return true;
        }
    }
}