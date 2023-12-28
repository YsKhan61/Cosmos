using Cosmos.ConnectionManagement;
using Cosmos.Infrastructure;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    public class IPConnectionWindow : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TextMeshProUGUI _titleText;

        [Inject] private IPUIMediator _ipUIMediator;

        ISubscriber<ConnectStatus> _connectStatusSubscriber;

        private void Awake()
        {
            Hide();
        }

        private void OnDestroy()
        {
            _connectStatusSubscriber?.Unsubscribe(OnConnectStatusMessage);
        }

        [Inject]
        private void InjectDependencies(ISubscriber<ConnectStatus> connectStatusSubscriber)
        {
            _connectStatusSubscriber = connectStatusSubscriber;
            _connectStatusSubscriber.Subscribe(OnConnectStatusMessage);
        }

        public void ShowConnectingWindow()
        {
            void OnTimeElapsed()
            {
                Hide();
                _ipUIMediator.DisableSignInSpinner();
            }

            UnityTransport utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            int maxConnectAttempts = utp.MaxConnectAttempts;
            int connectTimeoutMS = utp.ConnectTimeoutMS;
            StartCoroutine(DisplayUTPConnectionDuration(maxConnectAttempts, connectTimeoutMS, OnTimeElapsed));
        }

        public void CancelConnectionWindow()
        {
            Hide();
            StopAllCoroutines();
        }

        /// <summary>
        /// Invoked by UI Cancel button
        /// </summary>
        public void OnCancelJoinButtonPressed()
        {
            CancelConnectionWindow();
            _ipUIMediator.OnJoiningWindowCancelled();
        }

        public void Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        private void OnConnectStatusMessage(ConnectStatus status)
        {
            CancelConnectionWindow();
            _ipUIMediator.DisableSignInSpinner();
        }

        private IEnumerator DisplayUTPConnectionDuration(int maxConnectAttempts, int connectTimeoutMS, Action endAction)
        {
            float connectionDuration = maxConnectAttempts * connectTimeoutMS / 1000f;

            int seconds = Mathf.CeilToInt(connectionDuration);

            while (seconds > 0)
            {
                _titleText.text = $"Connecting... \n{seconds}";
                yield return new WaitForSeconds(1f);
                seconds--;
            }

            _titleText.text = $"Connecting...";

            endAction();
        }

        private void EndAction()
        {
            throw new NotImplementedException();
        }
    }
}

