using Cosmos.ApplicationLifecycle.Messages;
using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.UI;
using Cosmos.Infrastructure;
using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using Cosmos.Utilities;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
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
        private Button _lobbyButton;

        [SerializeField]
        private GameObject _signInSpinner;

        [SerializeField]
        UIProfileSelector _uiProfileSelector;


        /*[SerializeField, Tooltip("Detect hovering and check if UGS is initialized correctly or not, and show a tooltip!")]
        private UITooltipDetector _ugsSetupTooltipDetector;*/

        [SerializeField]
        TMP_InputField _playerNameInputField;

        private AuthenticationServiceFacade _authServiceFacade;
        ISubscriber<QuitApplicationMessage> _quitApplicationMessageSubscriber;
        private LocalLobbyUser _localLobbyUser;
        private LocalLobby _localLobby;
        private ProfileManager _profileManager;

        public override GameState ActiveState => GameState.MainMenu;

        private AccountType _accountType;

        protected override void Awake()
        {
            base.Awake();

            // _lobbyButton.interactable = false;
            _lobbyUIMediator.Hide();
            _signInSpinner.SetActive(false);

            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                OnSignInFailed();
                return;
            }

            // TrySignIn();         // now its called from the button of SignInUIMediator.cs
        }

        protected override void OnDestroy()
        {
            _profileManager.OnProfileChanged -= OnProfileChanged;
            Application.wantsToQuit -= OnApplicationWantsToQuit;
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
            LocalLobby localLobby,
            ProfileManager profileManager)
            // ISubscriber<QuitApplicationMessage> quitApplicationMessageSubscriber)
        {
            _authServiceFacade = authServiceFacade;
            _localLobbyUser = localLobbyUser;
            _localLobby = localLobby;
            _profileManager = profileManager;
            // _quitApplicationMessageSubscriber = quitApplicationMessageSubscriber;
            // _quitApplicationMessageSubscriber.Subscribe(SignOut);
            _ = _authServiceFacade.InitializeToUnityServicesAsync();
            _authServiceFacade.SubscribeToSignedInEvent();

            Application.wantsToQuit += OnApplicationWantsToQuit;
        }

        /// <summary>
        /// Called from the refresh button of Name Display UI.
        /// </summary>
        public void RefreshPlayerName()
        {
            _profileManager.ProfileName = _nameGenerationData.GetRandomName();
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

        public void OnChangeProfileButtonClicked()
        {
            _uiProfileSelector.Show();
        }

        /*public async void TrySignIn()
        {
            try
            {
                _profileManager.ProfileName = _nameGenerationData.GetRandomName();
                _playerNameInputField.text = _profileManager.ProfileName;

                InitializationOptions unityAuthenticationInitOptions =
                    _authServiceFacade.GenerateAuthenticationInitOptions(_profileManager.ProfileName);

                // await _authServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
                await _authServiceFacade.InitializeToUnityServicesAsync(unityAuthenticationInitOptions);
                await _authServiceFacade.SignInAnonymously();

                // Also update the player name in the authentication service
                await _authServiceFacade.UpdatePlayerNameAsync(_profileManager.ProfileName);

                OnAuthSignIn();

                _profileManager.OnProfileChanged += OnProfileChanged;
            }
            catch (Exception)
            {
                OnSignInFailed();
            }
        }*/

        public async void TrySignIn(AccountType accountType)
        {
            _signInSpinner.SetActive(true);
            _accountType = accountType;

            try
            {
                /*_profileManager.ProfileName = _nameGenerationData.GetRandomName();
                _playerNameInputField.text = _profileManager.ProfileName;

                InitializationOptions unityAuthenticationInitOptions =
                    _authServiceFacade.GenerateAuthenticationInitOptions(_profileManager.ProfileName);

                // await _authServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
                await _authServiceFacade.InitializeToUnityServicesAsync(unityAuthenticationInitOptions);*/
                // await _authServiceFacade.InitializeToUnityServicesAsync();

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

                // Also update the player name in the authentication service
                // await _authServiceFacade.UpdatePlayerNameAsync(_profileManager.ProfileName);

                OnAuthSignIn();

                // _profileManager.OnProfileChanged += OnProfileChanged;
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
            _authServiceFacade.SignOutFromAuthenticationService(true);

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

            // clear session token
            // 

            /*while (_authServiceFacade.IsSignedIn)
            {
                await Task.Yield();
            }*/
            // wait until 
        }

        private void OnAuthSignIn()
        {
            // _lobbyButton.interactable = true;
            // _ugsSetupTooltipDetector.enabled = false;
            _signInSpinner.SetActive(false);
            _signInUIMediator.HidePanel();
            _startMenuUIMediator.PlayerSignedInWithUGS();

#if UNITY_EDITOR
            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Signed in. Unity Player Name {AuthenticationService.Instance.PlayerName}");
#endif
            UpdateLocalLobbyUser();

            // The local lobby user object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already
            // when that happens.
            _localLobby.AddUser(_localLobbyUser);

        }

        private void OnAuthSignedOut()
        {
            _lobbyButton.interactable = false;
            // _ugsSetupTooltipDetector.enabled = true;
            _signInSpinner.SetActive(false);
            _startMenuUIMediator.ShowLobbyButtonTooltip();
            _startMenuUIMediator.HidePanel();
            _signInUIMediator.ShowPanel();
        }

        private void OnSignInFailed()
        {
            if (_lobbyButton)
            {
                _lobbyButton.interactable = false;
                // _ugsSetupTooltipDetector.enabled = true;
            }

            if (_signInSpinner)
            {
                _signInSpinner.SetActive(false);
            }
        }

        private async void OnProfileChanged()
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
        }

        private void UpdateLocalLobbyUser()
        {
            _localLobbyUser.ID = AuthenticationService.Instance.PlayerId;

            string playerName = AuthenticationService.Instance.PlayerName;

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
            TryAuthSignOut();
            _authServiceFacade.UnsubscribeFromSignedInEvent();
            return true;
        }
    }
}