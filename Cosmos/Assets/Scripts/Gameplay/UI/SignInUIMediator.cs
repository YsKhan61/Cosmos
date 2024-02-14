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

        public async void SignInWithUnityPlayerAccount()
        {
            bool success = await _clientMainMenuState.TrySignIn(AccountType.UnityPlayerAccount, _profileNameInputField.text);
            if (success) _signInWithPlayerAccountButton.interactable = false;
        }

        public async void SignInAsGuest()
        {
            bool success = await _clientMainMenuState.TrySignIn(AccountType.GuestAccount, _profileNameInputField.text);
            if (success) _signInAsGuestButton.interactable = false;
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