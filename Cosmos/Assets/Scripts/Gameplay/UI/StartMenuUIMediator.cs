using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.GameState;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Used to handle the Start Menu UI
    /// </summary>
    public class StartMenuUIMediator : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup _canvasGroup;

        [SerializeField]
        ClientMainMenuState _clientMainMenuState;

        [SerializeField]
        TMP_InputField _playerNameInputField;

        [SerializeField]
        Button _lobbyButton;

        [SerializeField]
        Button _directIpButton;

        [SerializeField]
        Button _linkAccountButton;

        [SerializeField]
        Button _unlinkAccountButton;

        // also need to add delete account button.

        [SerializeField]
        UITooltipDetector _lobbyButtonTooltipDetector;

        [SerializeField]
        NameGenerationDataSO _nameGenerationData;

        private void Awake()
        {
            _lobbyButton.interactable = false;
            HidePanel();
            ShowLobbyButtonTooltip();
        }

        /// <summary>
        /// called froim Link button of Start Main Menu UI
        /// </summary>
        public void LinkAccountWithUnity()
        {
            _clientMainMenuState.LinkAccountWithUnityAsync();
        }

        /// <summary>
        /// Called from Unlink button of Start Main Menu UI
        /// </summary>
        public void UnlinkAccountWithUnity() 
        {
            _clientMainMenuState.UnlinkAccountWithUnityAsync();
        }

        /// <summary>
        /// Called from Sign Out button of Start Main Menu UI
        /// </summary>
        public void SignOut()
        {
            _clientMainMenuState.SignOut();
        }

        /// <summary>
        /// Called from refresh button of Start Main Menu UI
        /// </summary>
        public void RefreshPlayerName()
        {
            _playerNameInputField.text = _nameGenerationData.GetRandomName();
        }

        /// <summary>
        /// Called from save button of Start Main Menu UI
        /// </summary>
        public void SavePlayerName()
        {
            _clientMainMenuState.SavePlayerName(_playerNameInputField.text);
        }

        internal void ConfigureStartMenuAfterSignInSuccess(string playerName)
        {
            _lobbyButton.interactable = true;
            ShowPanel();
            HideLobbyButtonTooltip();
            _playerNameInputField.text = playerName;

            if (_clientMainMenuState.AccountType == AccountType.GuestAccount)
            {
                if (!_linkAccountButton.gameObject.activeSelf)
                    _linkAccountButton.gameObject.SetActive(true);

                if (_unlinkAccountButton.gameObject.activeSelf)
                    _unlinkAccountButton.gameObject.SetActive(false);
            }
            else
            {
                if (_linkAccountButton.gameObject.activeSelf)
                    _linkAccountButton.gameObject.SetActive(false);

                if (!_unlinkAccountButton.gameObject.activeSelf)
                    _unlinkAccountButton.gameObject.SetActive(true);
            }
        }

        internal void ConfigureStartMenuAfterLinkAccountSuccess()
        {
            if (_linkAccountButton.gameObject.activeSelf)
                _linkAccountButton.gameObject.SetActive(false);

            if (!_unlinkAccountButton.gameObject.activeSelf)
                _unlinkAccountButton.gameObject.SetActive(true);
        }

        internal void ConfigureStartMenuAfterLinkAccountFailed()
        {
            if (!_linkAccountButton.gameObject.activeSelf)
                _linkAccountButton.gameObject.SetActive(true);

            if (_unlinkAccountButton.gameObject.activeSelf)
                _unlinkAccountButton.gameObject.SetActive(false);
        }

        internal void ConfigureStartMenuAfterUnlinkAccount()
        {
            _linkAccountButton.gameObject.SetActive(true);
            _unlinkAccountButton.gameObject.SetActive(false);
        }

        internal void ShowPanel()
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.interactable = true;    
        }

        internal void HidePanel()
        {
            _canvasGroup.gameObject.SetActive(false);
            _canvasGroup.interactable = false;
        }

        internal void ShowLobbyButtonTooltip()
        {
            _lobbyButtonTooltipDetector.enabled = true;
        }

        internal void HideLobbyButtonTooltip()
        {
            _lobbyButtonTooltipDetector.enabled = false;
        }
    }
}