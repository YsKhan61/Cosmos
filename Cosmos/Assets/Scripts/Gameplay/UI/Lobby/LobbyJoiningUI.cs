using Cosmos.Infrastructure;
using Cosmos.UnityServices.Lobbies;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// handles the list of LobbyListItemUIs and ensures it stays synchronized with the lobby list from the service.
    /// </summary>
    public class LobbyJoiningUI : MonoBehaviour
    {
        [SerializeField] private LobbyListItemUI _lobbyListItemUIPrototype;
        [SerializeField] private TMP_InputField _joinCodeInputField;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Graphic _emptyLobbyListLabel;
        [SerializeField] private Button _joinLobbyButton;

        private IObjectResolver _objectResolver;
        private LobbyUIMediator _lobbyUIMediator;
        private UpdateRunner _updateRunner;
        private ISubscriber<LobbyListFetchedMessage> _localLobbiesRefreshedSubscribers;

        private List<LobbyListItemUI> _lobbyListItems = new List<LobbyListItemUI>();

        private void Awake()
        {
            _lobbyListItemUIPrototype.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_updateRunner != null)
            {
                _updateRunner.Unsubscribe(PeriodicRefresh);
            }
        }

        private void OnDestroy()
        {
            if (_localLobbiesRefreshedSubscribers != null)
            {
                _localLobbiesRefreshedSubscribers.Unsubscribe(UpdateUI);
            }
        }

        [Inject]
        void InjectDependenciesAndInitialize(
            IObjectResolver objectResolver,
            LobbyUIMediator lobbyMediator,
            UpdateRunner updateRunner,
            ISubscriber<LobbyListFetchedMessage> localLobbiesRefreshedSubscribers)
        {
            _objectResolver = objectResolver;
            _lobbyUIMediator = lobbyMediator;
            _updateRunner = updateRunner;
            _localLobbiesRefreshedSubscribers = localLobbiesRefreshedSubscribers;
            _localLobbiesRefreshedSubscribers.Subscribe(UpdateUI);
        }

        /// <summary>
        /// Added to the InputField component's OnValueChanged callback for the join code text.
        /// </summary>
        public void OnJoinCodeInputTextChanged()
        {
            _joinCodeInputField.text = SanitizeJoinCode(_joinCodeInputField.text);
            _joinCodeInputField.interactable = _joinCodeInputField.text.Length > 0;
        }

        public void OnJoinButtonPressed()
        {
            _lobbyUIMediator.JoinLobbyWithCodeRequest(SanitizeJoinCode(_joinCodeInputField.text));
        }

        public void OnRefresh()
        {
            _lobbyUIMediator.QueryLobbiesRequest(true);
        }

        private string SanitizeJoinCode(string dirtyString)
        {
            return Regex.Replace(dirtyString.ToUpper(), "[^A-Z0-9]", "");
        }

        public void OnQuickJoinClicked()
        {
            _lobbyUIMediator.QuickJoinRequest();
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _updateRunner.Unsubscribe(PeriodicRefresh);
        }

        public void Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _updateRunner.Subscribe(PeriodicRefresh, 10f);
        }

        private void PeriodicRefresh(float _)
        {
            // this is a soft refresh without needing to lock the UI and such
            _lobbyUIMediator.QueryLobbiesRequest(false);
        }

        private void UpdateUI(LobbyListFetchedMessage message)
        {
            EnsureNumberOfActiveUISlots(message.LocalLobbies.Count);

            for (int i = 0; i < message.LocalLobbies.Count; i++)
            {
                LocalLobby localLobby = message.LocalLobbies[i];
                _lobbyListItems[i].SetData(localLobby);
            }

            if (message.LocalLobbies.Count == 0)
            {
                _emptyLobbyListLabel.enabled = true;
            }
            else
            {
                _emptyLobbyListLabel.enabled = false;
            }
        }

        private void EnsureNumberOfActiveUISlots(int requiredNumber)
        {
            int delta = requiredNumber - _lobbyListItems.Count;

            for (int i = 0; i < delta; i++)
            {
                _lobbyListItems.Add(CreateLobbyListItem());
            }

            for (int i = 0; i< _lobbyListItems.Count; i++)
            {
                _lobbyListItems[i].gameObject.SetActive(i < requiredNumber);
            }
        }

        private LobbyListItemUI CreateLobbyListItem()
        {
            LobbyListItemUI lobbyListItemUI = Instantiate(_lobbyListItemUIPrototype.gameObject, _lobbyListItemUIPrototype.transform.parent)
                .GetComponent<LobbyListItemUI>();

            lobbyListItemUI.gameObject.SetActive(true);

            _objectResolver.Inject(lobbyListItemUI);

            return lobbyListItemUI;
        }
    }
}