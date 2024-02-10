using Cosmos.Gameplay.GameState;
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
        Button _lobbyButton;

        [SerializeField]
        Button _directIpButton;

        [SerializeField]
        UITooltipDetector _lobbyButtonTooltipDetector;

        private void Awake()
        {
            _lobbyButton.interactable = false;
            HidePanel();
            ShowLobbyButtonTooltip();
        }

        public void LinkAccount()
        {

        }

        public void PlayerSignedInWithUGS()
        {
            _lobbyButton.interactable = true;
            ShowPanel();
            HideLobbyButtonTooltip();
        }

        public void SignOut()
        {
            _clientMainMenuState.TrySignOut();
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