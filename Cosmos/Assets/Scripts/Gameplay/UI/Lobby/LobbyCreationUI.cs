using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    public class LobbyCreationUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _lobbyNameInputField;
        [SerializeField] private GameObject _loadingIndicatorObject;
        [SerializeField] private Toggle _isPrivate;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Inject] private LobbyUIMediator _lobbyUIMediator;

        private void Awake()
        {
            EnableUnityRelayUI();
        }

        public void OnCreateButtonClicked()
        {
            _lobbyUIMediator.CreateLobbyRequest(_lobbyNameInputField.text, _isPrivate.isOn);
        }

        internal void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        internal void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }

        private void EnableUnityRelayUI()
        {
            _loadingIndicatorObject.SetActive(false);
        }
    }
}
