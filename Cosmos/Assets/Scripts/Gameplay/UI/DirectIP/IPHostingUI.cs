using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Cosmos.Gameplay.UI
{
    public class IPHostingUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _ipInputField;
        [SerializeField] private TMP_InputField _portInputField;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private Button _hostButton;

        [Inject] private IPUIMediator _ipUIMediator;

        private void Awake()
        {
            _ipInputField.text = IPUIMediator.DEFAULT_IP;
            _portInputField.text = IPUIMediator.DEFAULT_PORT.ToString();
        }

        public void OnCreateButtonClicked()
        {
            _ipUIMediator.HostIPRequest(_ipInputField.text, _portInputField.text);
        }

        /// <summary>
        /// Added to the InputField component's OnValueChanged callback for the Room/IP UI text.
        /// </summary>
        public void SanitizeIPInputText()
        {
            _ipInputField.text = IPUIMediator.SanitizeIP(_ipInputField.text);
            _hostButton.interactable = IPUIMediator.AreIpAddressAndPortValid(_ipInputField.text, _portInputField.text);
        }

        /// <summary>
        /// Added to the InputField component's OnValueChanged callback for the Port UI text.
        /// </summary>
        public void SanitizePortText()
        {
            _portInputField.text = IPUIMediator.SanitizePort(_portInputField.text);
            _hostButton.interactable = IPUIMediator.AreIpAddressAndPortValid(_ipInputField.text, _portInputField.text);
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
    }
}
