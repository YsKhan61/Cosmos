using Cosmos.Gameplay.GameState;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Configures and display the lobby info box and informations related to lobby.
    /// </summary>
    public class LobbyInfoBoxUI : MonoBehaviour
    {
        [SerializeField]
        NetcodeHooks m_NetcodeHooks;

        [SerializeField]
        NetworkCharSelection m_NetworkCharSelection;

        [SerializeField]
        [Tooltip("Text element containing player count which updates as players connect")]
        TextMeshProUGUI m_NumPlayersText;

        private void Awake()
        {
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
            else
            {
                m_NetworkCharSelection.LobbyPlayers.OnListChanged += OnLobbyPlayerStateChanged;
            }
        }

        void OnNetworkDespawn()
        {
            if (m_NetworkCharSelection)
            {
                m_NetworkCharSelection.LobbyPlayers.OnListChanged -= OnLobbyPlayerStateChanged;
            }
        }

        private void OnLobbyPlayerStateChanged(NetworkListEvent<NetworkCharSelection.LobbyPlayerState> changeEvent)
        {
            UpdatePlayerCount();
            PrintPlayerNames();
        }

        void UpdatePlayerCount()
        {
            int count = m_NetworkCharSelection.LobbyPlayers.Count;
            var pstr = (count > 1) ? "players" : "player";
            m_NumPlayersText.text = "<b>" + count + "</b> " + pstr + " connected";
        }

        void PrintPlayerNames()
        {
            for (int i = 0; i < m_NetworkCharSelection.LobbyPlayers.Count; ++i)
            {
                Debug.Log("Player " + i + " is " + m_NetworkCharSelection.LobbyPlayers[i].PlayerName);
            }
        }
    }
}
