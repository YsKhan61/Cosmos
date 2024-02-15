using Cosmos.Gameplay.GameState;
using System;
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
        /// <summary>
        /// Information related to the specific member.
        /// </summary>
        [Serializable]
        public class MemberInfo
        {
            public ulong ClientId;
            public TextMeshProUGUI NameText;
            public Button KickButton;
        }

        [SerializeField]
        MemberInfo[] m_MemberInfos;

        [SerializeField]
        NetcodeHooks m_NetcodeHooks;

        [SerializeField]
        NetworkCharSelection m_NetworkCharSelection;

        [SerializeField]
        GameObject m_LobbyInfoPanelGO;

        [SerializeField]
        [Tooltip("Text element containing player count which updates as players connect")]
        TextMeshProUGUI m_NumPlayersText;

        [SerializeField]
        Sprite m_OwnerIcon;

        private void Awake()
        {
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;

            HideMemberInfosPanel();
        }

        /// <summary>
        /// called when user clicks on number of players text.
        /// </summary>
        public void ShowMemberInfosPanel()
        {
            m_LobbyInfoPanelGO.SetActive(true);
        }

        /// <summary>
        /// called when user clicks on close button of lobby member info panel.
        /// </summary>
        public void HideMemberInfosPanel()
        {
               m_LobbyInfoPanelGO.SetActive(false);
        }

        /// <summary>
        /// called when user clicks on kick button of a member.
        /// </summary>
        public void KickPlayer()
        {
            var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            var memberInfo = Array.Find(m_MemberInfos, info => info.KickButton.gameObject == button);
            if (memberInfo != null)
            {
                m_NetworkCharSelection.KickMemberFromLobbyServerRpc(memberInfo.ClientId);
            }
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
            ResetInfoManel();

            UpdatePlayerCount();
            DisplayPlayerNames();
        }

        void UpdatePlayerCount()
        {
            int count = m_NetworkCharSelection.LobbyPlayers.Count;
            var pstr = (count > 1) ? "players" : "player";
            m_NumPlayersText.text = "<b>" + count + "</b> " + pstr + " connected";
        }

        void DisplayPlayerNames()
        {
            for (int i = 0; i < m_NetworkCharSelection.LobbyPlayers.Count; ++i)
            {
                // Debug.Log("Player " + i + " is " + m_NetworkCharSelection.LobbyPlayers[i].PlayerName);
                m_MemberInfos[i].ClientId = m_NetworkCharSelection.LobbyPlayers[i].ClientId;
                m_MemberInfos[i].NameText.text = m_NetworkCharSelection.LobbyPlayers[i].PlayerName;


                if (m_NetworkCharSelection.LobbyPlayers[i].ClientId == NetworkManager.ServerClientId)
                {
                    m_MemberInfos[i].KickButton.gameObject.SetActive(true);
                    m_MemberInfos[i].KickButton.image.sprite = m_OwnerIcon;
                    continue;
                }
                else if (NetworkManager.Singleton.IsHost)
                {
                    m_MemberInfos[i].KickButton.gameObject.SetActive(true);
                }
                else
                {
                    m_MemberInfos[i].KickButton.gameObject.SetActive(false);
                }

                /*// Only the host can kick other members
                if (NetworkManager.Singleton.IsHost
                    && !m_NetworkCharSelection.LobbyPlayers[i].IsHost)
                {
                    m_MemberInfos[i].KickButton.gameObject.SetActive(true);
                }
                else
                {
                    m_MemberInfos[i].KickButton.gameObject.SetActive(false);
                }*/
            }
        }

        private void ResetInfoManel()
        {
            for (int i = 0; i < m_MemberInfos.Length; ++i)
            {
                m_MemberInfos[i].ClientId = ulong.MaxValue;
                m_MemberInfos[i].NameText.text = "No member present";
                m_MemberInfos[i].KickButton.gameObject.SetActive(false);
            }
        }
    }
}
