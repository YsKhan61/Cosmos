using Cosmos.Gameplay.Configuration;
using Cosmos.Gameplay.GameState;
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
        UITooltipDetector _lobbyButtonTooltipDetector;

        [SerializeField]
        NameGenerationDataSO _nameGenerationData;

        private void Awake()
        {
            _lobbyButton.interactable = false;
            HidePanel();
            ShowLobbyButtonTooltip();
        }

        public void LinkAccount()
        {

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

        public void PlayerSignedInWithUGS(string playerName)
        {
            _lobbyButton.interactable = true;
            ShowPanel();
            HideLobbyButtonTooltip();
            _playerNameInputField.text = playerName;
        }

        public void ShowPanel()
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.interactable = true;
        }

        public void HidePanel()
        {
            _canvasGroup.gameObject.SetActive(false);
            _canvasGroup.interactable = false;
        }

        public void ShowLobbyButtonTooltip()
        {
            _lobbyButtonTooltipDetector.enabled = true;
        }

        public void HideLobbyButtonTooltip()
        {
            _lobbyButtonTooltipDetector.enabled = false;
        }
    }
}