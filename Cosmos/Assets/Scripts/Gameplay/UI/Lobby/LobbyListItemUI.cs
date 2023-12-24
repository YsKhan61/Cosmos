using Cosmos.UnityServices.Lobbies;
using TMPro;
using UnityEngine;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// An individual Lobby UI in the list of available lobbies.
    /// </summary>
    public class LobbyListItemUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _lobbyNameText;

        [SerializeField]
        private TextMeshProUGUI _lobbyCountText;

        [Inject] 
        private LobbyUIMediator _lobbyUIMediator;

        private LocalLobby _localLobby;

        public void SetData(LocalLobby localLobby)
        {
            _localLobby = localLobby;
            _lobbyNameText.SetText(localLobby.LobbyName);
            _lobbyCountText.SetText($"{localLobby.PlayerCount}/{localLobby.MaxPlayerCount}");
        }

        public void OnClick()
        {
            _lobbyUIMediator.JoinLobbyRequest(_localLobby);
        }
    }
}
