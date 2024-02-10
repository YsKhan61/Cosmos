using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.UI;
using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using Cosmos.Utilities;
using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Cosmos.Gameplay.GameState
{
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
        private LobbyUIMediator _lobbyUIMediator;

        [SerializeField]
        private IPUIMediator _ipUIMediator;

        [SerializeField]
        private Button _lobbyButton;

        [SerializeField]
        private GameObject _signInSpinner;

        [SerializeField]
        UIProfileSelector _uiProfileSelector;


        [SerializeField, Tooltip("Detect hovering and check if UGS is initialized correctly or not, and show a tooltip!")]
        private UITooltipDetector _ugsSetupTooltipDetector;

        /*[SerializeField]
        TextMeshProUGUI _playerNameText;*/

        [SerializeField]
        TMP_InputField _playerNameInputField;

        [Inject]
        private AuthenticationServiceFacade _authServiceFacade;

        [Inject]
        private LocalLobbyUser _localLobbyUser;

        [Inject]
        private LocalLobby _localLobby;

        [Inject]
        private ProfileManager _profileManager;

        public override GameState ActiveState => GameState.MainMenu;

        protected override void Awake()
        {
            base.Awake();

            _lobbyButton.interactable = false;
            _lobbyUIMediator.Hide();

            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                OnSignInFailed();
                return;
            }

            TrySignIn();
        }

        protected override void OnDestroy()
        {
            _profileManager.OnProfileChanged -= OnProfileChanged;
            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(_nameGenerationData);
            builder.RegisterComponent(_lobbyUIMediator);
            builder.RegisterComponent(_ipUIMediator);
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

        private async void TrySignIn()
        {
            try
            {
                _profileManager.ProfileName = _nameGenerationData.GetRandomName();
                _playerNameInputField.text = _profileManager.ProfileName;

                InitializationOptions unityAuthenticationInitOptions =
                    _authServiceFacade.GenerateAuthenticationInitOptions(_profileManager.ProfileName);

                await _authServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);

                // Also update the player name in the authentication service
                await _authServiceFacade.UpdatePlayerNameAsync(_profileManager.ProfileName);

                OnAuthSignIn();

                _profileManager.OnProfileChanged += OnProfileChanged;
            }
            catch (Exception)
            {
                OnSignInFailed();
            }
        }

        private void OnAuthSignIn()
        {
            _lobbyButton.interactable = true;
            _ugsSetupTooltipDetector.enabled = false;
            _signInSpinner.SetActive(false);

#if UNITY_EDITOR
            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Signed in. Unity Player Name {AuthenticationService.Instance.PlayerName}");
#endif
            UpdateLocalLobbyUser();

            // The local lobby user object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already
            // when that happens.
            _localLobby.AddUser(_localLobbyUser);
            
        }

        private void OnSignInFailed()
        {
            if (_lobbyButton)
            {
                _lobbyButton.interactable = false;
                _ugsSetupTooltipDetector.enabled = true;

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

            await _authServiceFacade.SwitchProfileAndResignInAsync(_profileManager.ProfileName);
            await _authServiceFacade.UpdatePlayerNameAsync(_profileManager.ProfileName);

            _lobbyButton.interactable = true;
            _signInSpinner.SetActive(false);

#if UNITY_EDITOR
            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Signed in. Unity Player Name {AuthenticationService.Instance.PlayerName}");
#endif

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
                playerName = playerName.Substring(0, hashIndex);
            }

            _localLobbyUser.PlayerName = playerName.Substring(0, Mathf.Min(10, playerName.Length));
        }
    }
}
