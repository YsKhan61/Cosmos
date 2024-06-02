using Cosmos.Gameplay.GameState;
using Cosmos.UnityServices.Auth;
using TMPro;
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

        [SerializeField]
        TMP_InputField _profileNameInputField;

        private void Awake()
        {
            ShowPanel();
        }

        /// <summary>
        /// Called from Button UI element. Attempts to sign in with Unity Player Account.
        /// </summary>
        public void SignInWithUnityPlayerAccount()
        {
            if (string.IsNullOrEmpty(_profileNameInputField.text))
            {
                PopupManager.DisplayStatus("Profile name can't be empty!", 2);
                return;
            }

            _clientMainMenuState.TrySignIn(AccountType.UnityPlayerAccount, _profileNameInputField.text);
            _signInWithPlayerAccountButton.interactable = false;
            _signInAsGuestButton.interactable = false;
        }

        /// <summary>
        /// Called from Button UI element. Attempts to sign in as a guest.
        /// </summary>
        public void SignInAsGuest()
        {
            if (string.IsNullOrEmpty(_profileNameInputField.text))
            {
                PopupManager.DisplayStatus("Profile name can't be empty!", 2);
                return;
            }

            _clientMainMenuState.TrySignIn(AccountType.GuestAccount, _profileNameInputField.text);
            _signInAsGuestButton.interactable = false;
            _signInWithPlayerAccountButton.interactable = false;
        }

        internal void ConfigurePanelOnSignInFailed()
        {
            _signInWithPlayerAccountButton.interactable = true;
            _signInAsGuestButton.interactable = true;
        }

        internal void ShowPanel()
        {
            _signInWithPlayerAccountButton.interactable = true;
            _signInAsGuestButton.interactable = true;

            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.interactable = true;
        }

        internal void HidePanel()
        {
            _canvasGroup.gameObject.SetActive(false);
            _canvasGroup.interactable = false;
        }    
    }
}