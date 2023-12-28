using Cosmos.ConnectionManagement;
using Cosmos.Gameplay.Configuration;
using Cosmos.Infrastructure;
using System;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Networking.Transport;
using UnityEngine;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    public class IPUIMediator : MonoBehaviour
    {
        public const string DEFAULT_IP = "127.0.0.1";      // Editorial build -> 127.0.0.1 , Local Network -> 0.0.0.0
        public const int DEFAULT_PORT = 9998;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _playerNameText;

        [SerializeField] private IPJoiningUI _ipJoiningUI;
        [SerializeField] private IPHostingUI _ipHostingUI;
        [SerializeField] private IPConnectionWindow _ipConnectionWindow;

        [SerializeField] private UITinter _joinTabButtonHighlightTinter;
        [SerializeField] private UITinter _joinTabButtonTabBlockerTinter;
        [SerializeField] private UITinter _hostTabButtonHighlightTinter;
        [SerializeField] private UITinter _hostTabButtonTabBlockerTinter;

        [SerializeField] private GameObject _signInSpinner;

        [Inject] NameGenerationDataSO _nameGenerationData;
        [Inject] ConnectionManager _connectionManager;

        public IPHostingUI IPHostingUI => _ipHostingUI;

        ISubscriber<ConnectStatus> _connectStatusSubscriber;

        [Inject]
        private void InjectDependencies(ISubscriber<ConnectStatus> connectStatusSubscriber)
        {
            _connectStatusSubscriber = connectStatusSubscriber;
            _connectStatusSubscriber.Subscribe(OnConnectStatusMessage);
        }

        private void Awake()
        {
            Hide();
        }

        private void Start()
        {
            ToggleCreateIPUI();
            RegenerateName();
        }

        private void OnDestroy()
        {
            _connectStatusSubscriber?.Unsubscribe(OnConnectStatusMessage);
        }

        /// <summary>
        /// Called by OnClick Event of the Host with IP Tab Button
        /// </summary>
        public void ToggleCreateIPUI()
        {
            _ipJoiningUI.Hide();
            _ipHostingUI.Show();
            _joinTabButtonHighlightTinter.SetToColor(0);
            _joinTabButtonTabBlockerTinter.SetToColor(1);
            _hostTabButtonHighlightTinter.SetToColor(1);
            _hostTabButtonTabBlockerTinter.SetToColor(0);
        }

        /// <summary>
        /// Called by OnClick Event of the Join with IP Tab Button
        /// </summary>
        public void ToggleJoinIPUI()
        {
            _ipHostingUI.Hide();
            _ipJoiningUI.Show();
            _joinTabButtonHighlightTinter.SetToColor(1);
            _joinTabButtonTabBlockerTinter.SetToColor(0);
            _hostTabButtonHighlightTinter.SetToColor(0);
            _hostTabButtonTabBlockerTinter.SetToColor(1);
        }

        public void HostIPRequest(string ip, string port)
        {
            int.TryParse(port, out int portNumber);
            if (portNumber <= 0)
            {
                portNumber = DEFAULT_PORT;
            }

            ip = string.IsNullOrEmpty(ip) ? DEFAULT_IP : ip;

            _signInSpinner.SetActive(true);
            _connectionManager.StartHostIp(_playerNameText.text, ip, portNumber);
        }

        public void JoinIPRequest(string ip, string port)
        {
            int.TryParse(port, out int portNumber);
            if (portNumber <= 0)
            {
                portNumber = DEFAULT_PORT;
            }

            ip = string.IsNullOrEmpty(ip) ? DEFAULT_IP : ip;

            _signInSpinner.SetActive(true);
            _connectionManager.StartClientIP(_playerNameText.text, ip, portNumber);
            _ipConnectionWindow.ShowConnectingWindow();
        }

        public void OnJoiningWindowCancelled()
        {
            DisableSignInSpinner();
            RequestShutdown();
        }

        /// <summary>
        /// To be called from the Cancel (X) UI button.
        /// </summary>
        public void CancelConnectingWindow()
        {
            RequestShutdown();
            _ipConnectionWindow.CancelConnectionWindow();
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void DisableSignInSpinner()
        {
            _signInSpinner.SetActive(false);
        }

        public void RegenerateName()
        {
            _playerNameText.text = _nameGenerationData.GenerateName();
        }

        #region STATIC METHODS

        /// <summary>
        /// Sanitize user IP address InputField box allowing only numbers and '.'.
        /// This also prevents undesirable invisible characters from being copy-pasted accidentally.
        /// </summary>
        /// <param name="dirtyString">string to sanitize</param>
        /// <returns>sanitized text string</returns>
        public static string SanitizeIP(string dirtyString)
        {
            return Regex.Replace(dirtyString, "[^0-9.]", "");
        }

        /// <summary>
        /// Sanitize user port InputField box allowing only numbers.
        /// This also prevents undesirable invisible characters from being copy-pasted accidentally.
        /// </summary>
        /// <param name="dirtyString">string to sanitize</param>
        /// <returns>sanitized text string</returns>
        public static string SanitizePort(string dirtyString)
        {
            return Regex.Replace(dirtyString, "[^0-9]", "");
        }

        public static bool AreIpAddressAndPortValid(string ipAddress, string port)
        {
            bool isPortValid = ushort.TryParse(port, out ushort portNum);
            return isPortValid && NetworkEndpoint.TryParse(ipAddress, portNum, out NetworkEndpoint endpoint);
        }

        #endregion

        private void OnConnectStatusMessage(ConnectStatus status)
        {
            DisableSignInSpinner();
        }

        private void RequestShutdown()
        {
            if (_connectionManager && _connectionManager.NetworkManager)
            {
                _connectionManager.RequestShutdown();
            }
        }
    }
}
