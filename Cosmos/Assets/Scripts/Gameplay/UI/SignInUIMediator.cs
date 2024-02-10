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

        private void Awake()
        {
            ShowPanel();
        }

        public void SignInWithUnityPlayerAccount()
        {
            
            _clientMainMenuState.TrySignIn(AccountType.UnityPlayerAccount);
        }

        public void SignInAsGuest()
        {
            _clientMainMenuState.TrySignIn(AccountType.GuestAccount);
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
    }
}