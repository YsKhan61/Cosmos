using Cosmos.Utilities;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// This is attached to the VR Player of Cosmos scene, and is used to handle the quit panel UI.
    /// </summary>
    public class UIQuitPanelVR : MonoBehaviour
    {
        private enum QuitMode
        {
            ReturnToMenu,
            QuitApplication
        }

        [Header("Publish to")]
        [SerializeField] private VoidEventChannelSO _onReturnToMenuEventRaised;
        [SerializeField] private VoidEventChannelSO _onQuitApplicationEventRaised;

        [Space(5)]

        [SerializeField] private QuitMode _quitMode = QuitMode.ReturnToMenu;

        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private void Awake()
        {
            _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            _cancelButton.onClick.AddListener(Cancel);
        }

        private void OnDestroy()
        {
            _confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            _cancelButton.onClick.RemoveListener(Cancel);
        }

        private void OnConfirmButtonClicked()
        {
            switch (_quitMode)
            {
                case QuitMode.ReturnToMenu:
                    _onReturnToMenuEventRaised?.RaiseEvent();
                    break;
                case QuitMode.QuitApplication:
                    _onQuitApplicationEventRaised?.RaiseEvent();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            gameObject.SetActive(false);
        }

        private void Cancel()
        {
            gameObject.SetActive(false);
        }
    }
}
