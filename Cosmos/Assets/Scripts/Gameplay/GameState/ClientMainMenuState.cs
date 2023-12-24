using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.UI;
using Cosmos.UnityServices.Auth;
using Cosmos.UnityServices.Lobbies;
using Cosmos.Utilities;
using System;
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
        private Button _lobbyButton;

        [SerializeField]
        private GameObject _signInSpinner;

        [SerializeField]
        UIProfileSelector _uiProfileSelector;

        

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
            
        }

        public void OnStartButtonClicked()
        {
            _lobbyUIMediator.ToggleJoinLobbyUI();
            _lobbyUIMediator.Show();
        }

        public void OnDirectIPClicked()
        {
            _lobbyUIMediator.Hide();
            
        }

        public void OnChangeProfileButtonClicked()
        {
            _uiProfileSelector.Show();
        }

        private async void TrySignIn()
        {
            try
            {
                InitializationOptions unityAuthenticationInitOptions =
                    _authServiceFacade.GenerateAuthenticationOptions(_profileManager.Profile);

                await _authServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);

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
            _signInSpinner.SetActive(false);

#if UNITY_EDITOR
            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
#endif

            _localLobbyUser.ID = AuthenticationService.Instance.PlayerId;

            // The local lobby user object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already
            // when that happens.
            _localLobby.AddUser(_localLobbyUser);
            
        }

        private void OnSignInFailed()
        {
            if (_lobbyButton)
            {
                _lobbyButton.interactable = false;

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
            await _authServiceFacade.SwitchProfileAndResignInAsync(_profileManager.Profile);

            _lobbyButton.interactable = true;
            _signInSpinner.SetActive(false);

#if UNITY_EDITOR
            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");
#endif

            _localLobby.RemoveUser(_localLobbyUser);
            _localLobbyUser.ID = AuthenticationService.Instance.PlayerId;
            _localLobby.AddUser(_localLobbyUser);
        }
    }
}
