using Cosmos.ConnectionManagement;
using Cosmos.Gameplay.GameplayObjects;
using Cosmos.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Handles the display of in-game messages in a message feed in VR Mode.
    /// </summary>
    public class UIMessageFeedVR : MonoBehaviour
    {
        [Header ("Listens to")]

        [SerializeField]
        NetworkChatMessageEventChannelSO m_NetworkChatMessageEventChannelFromServer;

        [Header("Sends to")]

        [SerializeField]
        NetworkChatMessageEventChannelSO m_NetworkChatMessageEventChannelToServer;

        [SerializeField]
        List<UIMessageSlot> m_MessageSlots;

        [SerializeField]
        GameObject m_MessageSlotPrefab;

        [SerializeField]
        VerticalLayoutGroup m_VerticalLayoutGroup;

        [SerializeField]
        ScrollRect m_ScrollRect;

        [SerializeField]
        Button m_MinimizeButton;

        [SerializeField]
        TMP_InputField m_ChatInputField;

        [SerializeField]
        Button m_SendButton;

        [SerializeField]
        PersistentPlayersRuntimeCollectionSO m_PersistentPlayersRuntimeCollection;

        FixedPlayerName m_OwnerClientName;

        private void Start()
        {
            m_NetworkChatMessageEventChannelFromServer.OnEventRaised += OnChatMessageReceived;
        }

        private void OnDestroy()
        {
            m_NetworkChatMessageEventChannelToServer.OnEventRaised -= OnChatMessageReceived;
        }

        /// <summary>
        /// Called from Send button to send a chat message.
        /// </summary>
        [ContextMenu("SendMessage")]
        public void SendMessage()
        {
            if (string.IsNullOrEmpty(m_ChatInputField.text))
            {
                return;
            }

            if (string.IsNullOrEmpty(m_OwnerClientName))
            {
                m_PersistentPlayersRuntimeCollection.TryGetPlayerName(NetworkManager.Singleton.LocalClientId, out m_OwnerClientName);
            }

            m_NetworkChatMessageEventChannelToServer.RaiseEvent(new NetworkChatMessage
            {
                Name = m_OwnerClientName,
                Message = m_ChatInputField.text
            });
        }

        public void ShowScrollRect()
        {
            m_ScrollRect.gameObject.SetActive(true);
            m_MinimizeButton.gameObject.SetActive(true);
        }

        public void HideScrollRect()
        {
            m_ScrollRect.gameObject.SetActive(false);
            m_MinimizeButton.gameObject.SetActive(false);
        }

        private void OnChatMessageReceived(NetworkChatMessage chat)
        {
            DisplayMessage($"{chat.Name}: {chat.Message}",
                chat.Name == m_OwnerClientName,
                false);
        }

        public void OnConnectionEvent(ConnectionEventMessage eventMessage)
        {
            switch (eventMessage.ConnectStatus)
            {
                case ConnectStatus.Success:
                    DisplayMessage($"{eventMessage.PlayerName} has joined the game!");
                    break;
                case ConnectStatus.KickedByHost:
                    DisplayMessage($"{eventMessage.PlayerName} has been kicked by the host!");
                    break;
                case ConnectStatus.ServerFull:
                case ConnectStatus.LoggedInAgain:
                case ConnectStatus.UserRequestedDisconnect:
                case ConnectStatus.GenericDisconnect:
                case ConnectStatus.IncompatibleBuildType:
                case ConnectStatus.HostEndedSession:
                    DisplayMessage($"{eventMessage.PlayerName} has left the game!");
                    break;
            }
        }

        void DisplayMessage(string text, bool isRightAlligned = false, bool autoClose = true)
        {
            ShowScrollRect();
            var messageSlot = GetAvailableSlot();
            messageSlot.Display(text, isRightAlligned);

            if (autoClose)
                StartCoroutine(HideRoutine());
        }

        IEnumerator HideRoutine()
        {
            yield return new WaitForSeconds(3);
            HideScrollRect();
        }

        UIMessageSlot GetAvailableSlot()
        {
            var go = Instantiate(m_MessageSlotPrefab, m_VerticalLayoutGroup.transform);
            var messageSlot = go.GetComponentInChildren<UIMessageSlot>();
            m_MessageSlots.Add(messageSlot);
            return messageSlot;
        }
    }
}
