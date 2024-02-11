using Cosmos.Gameplay.GameState;
using UnityEngine;
using UnityEngine.UI;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Used to handle the UI for signing in.
    /// </summary>
    public class SignInUIMediator : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup _canvasGroup;

        [SerializeField]
        ClientMainMenuState _clientMainMenuState;

        [SerializeField]
        Button _signInWithPlayerAccountButton;

        [SerializeField]
        Button _signInAsGuestButton;

        private void Awake()
        {
            ShowPanel();
        }

        public void SignInWithUnityPlayerAccount()
        {
            _clientMainMenuState.TrySignIn(AccountType.UnityPlayerAccount);
            _signInWithPlayerAccountButton.interactable = false;
        }

        public void SignInAsGuest()
        {
            _clientMainMenuState.TrySignIn(AccountType.GuestAccount);
            _signInAsGuestButton.interactable = false;
        }

        public void ShowPanel()
        {
            _signInWithPlayerAccountButton.interactable = true;
            _signInAsGuestButton.interactable = true;

            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.interactable = true;
        }

        public void HidePanel()
        {
            _canvasGroup.gameObject.SetActive(false);
            _canvasGroup.interactable = false;
        }    
    }
}