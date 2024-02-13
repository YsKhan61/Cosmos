using Cosmos.ApplicationLifecycle.Messages;
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

        [SerializeField]
        AuthStatusUI _authStatusUI;

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
        {
            _authServiceFacade = authServiceFacade;
            _localLobbyUser = localLobbyUser;
            _localLobby = localLobby;
            _ = _authServiceFacade.InitializeToUnityServicesAsync();
            _authServiceFacade.SubscribeToAuthenticationEvents();
            
            _authServiceFacade.onAuthSignInSuccess += OnAuthSignInSuccess;
            _authServiceFacade.onAuthSignInFailed += OnSignInFailed;
            _authServiceFacade.onAuthSignedOutSuccess += OnAuthSignedOutSuccess;
            _authServiceFacade.onLinkedInWithUnitySuccess += OnLinkSuccess;
            _authServiceFacade.onLinkedInWithUnityFailed += OnLinkFailed;
            _authServiceFacade.onUnlinkFromUnitySuccess += OnUnlinkSuccess;
            _authServiceFacade.onAccountNameUpdateSuccess += UpdateNameSuccess;
            _authServiceFacade.onAccountNameUpdateFailed += UpdateNameFailed;

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

        internal async void TrySignIn(AccountType accountType)
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
                // OnSignInFailed();
            }
        }

        internal async void LinkAccountWithUnityAsync()
        {
            _signInSpinner.SetActive(true);

            try
            {
                await _authServiceFacade.LinkAccountWithUnityAsync();
            }
            catch (Exception)
            {

            }
            finally
            {
                _signInSpinner.SetActive(false);
            }
        }

        internal async void UnlinkAccountWithUnityAsync()
        {
            _signInSpinner.SetActive(true);

            try
            {
                await _authServiceFacade.UnlinkAccountWithUnityAsync();

                OnUnlinkSuccess();
            }
            catch (Exception)
            {

            }
            finally
            {
                _signInSpinner.SetActive(false);
            }
        }

        internal void SignOut()
        {
            TryAuthSignOut();
        }

        private void TryAuthSignOut()
        {
            _authServiceFacade.SignOutFromAuthService(true);
            Debug.Log("ClientMainMenuState: Player Signed out from Authentication services!");

            switch (_accountType)
            {
                case AccountType.UnityPlayerAccount:
                    _authServiceFacade.SignOutFromPlayerAccountService();
                    Debug.Log("ClientMainMenuState: Player Signed out from Unity Player Account!");
                    break;
                case AccountType.GuestAccount:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAuthSignInSuccess()
        {
            _signInSpinner.SetActive(false);
            _signInUIMediator.HidePanel();

            _startMenuUIMediator.ConfigureStartMenuAfterSignInSuccess(_authServiceFacade.GetPlayerName());

            Debug.Log($"Signed in. Unity Player ID {_authServiceFacade.GetPlayerId()}");
            Debug.Log($"Signed in. Unity Player Name {_authServiceFacade.GetPlayerName()}");

            UpdateLocalLobbyUser();

            // The local lobby user object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already
            // when that happens.
            _localLobby.AddUser(_localLobbyUser);

            _authStatusUI.DisplayStatus("Signed in success!", 3);
        }

        private void OnAuthSignedOutSuccess()
        {
            _signInSpinner.SetActive(false);
            _startMenuUIMediator.ShowLobbyButtonTooltip();
            _startMenuUIMediator.HidePanel();
            _signInUIMediator.ShowPanel();

            _authStatusUI.DisplayStatus("Signed out success!", 3);
        }

        private void OnSignInFailed()
        {
            if (_signInSpinner)
            {
                _signInSpinner.SetActive(false);
            }

            _authStatusUI.DisplayStatus("Sign in failed!", 2);
        }

        private void OnLinkSuccess()
        {
            _startMenuUIMediator.ConfigureStartMenuAfterLinkAccountSuccess();
            _accountType = AccountType.UnityPlayerAccount;

            _authStatusUI.DisplayStatus("Link account success!", 3);
        }

        private void OnLinkFailed()
        {
            _authServiceFacade.SignOutFromPlayerAccountService();

            if (_signInSpinner)
            {
                _signInSpinner.SetActive(false);
            }

            _startMenuUIMediator.ConfigureStartMenuAfterLinkAccountFailed();

            _authStatusUI.DisplayStatus("Link account failed!", 2);
        }

        private void OnUnlinkSuccess()
        {
            _authServiceFacade.SignOutFromPlayerAccountService();
            Debug.Log("ClientMainMenuState: Player Signed out from Unity Player Account!");

            _startMenuUIMediator.ConfigureStartMenuAfterUnlinkAccount();
            _accountType = AccountType.GuestAccount;

            _authStatusUI.DisplayStatus("Unlink account success!", 3);
        }

        private void UpdateNameSuccess()
        {
            // Updating LocalLobbyUser and LocalLobby
            _localLobby.RemoveUser(_localLobbyUser);
            UpdateLocalLobbyUser();
            _localLobby.AddUser(_localLobbyUser);

            _authStatusUI.DisplayStatus("Name update success!", 3);
        }

        private void UpdateNameFailed()
        {
            _authStatusUI.DisplayStatus("Name update failed!", 2);
        }

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
            _authServiceFacade.UnsubscribeFromAuthenticationEvents();
            return true;
        }
    }
}