using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Controls the specific Canvas that has the settings icon and the settings window.
    /// The window itself is controlled by UISettingsPanel; the button is controlled here.
    /// </summary>
    public class UISettingsCanvas : MonoBehaviour
    {
        [SerializeField]
        private GameObject _settingsPanelRootGO;

        [SerializeField]
        private GameObject _quitPanelRootGO;

        [SerializeField]
        private Button _settingsButton;

        [SerializeField]
        private Button _quitButton;

        private void Awake()
        {
            // hide the settings window at startup (this is just to handle the common case where
            // an artist forgets to disable the window in the prefab)
            DisablePanels();
        }

        private void OnEnable()
        {
            _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnDisable()
        {
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }

        private void DisablePanels()
        {
            _settingsPanelRootGO.SetActive(false);
            _quitPanelRootGO.SetActive(false);
        }

        /// <summary>
        /// Called directly by the settings button OnClick event
        /// </summary>
        public void OnSettingsButtonClicked()
        {
            _settingsPanelRootGO.SetActive(!_settingsPanelRootGO.activeSelf);
            _quitPanelRootGO.SetActive(false);
        }

        /// <summary>
        /// Called directly by the quit button OnClick event
        /// </summary>
        public void OnQuitButtonClicked()
        {
            _quitPanelRootGO.SetActive(!_quitPanelRootGO.activeSelf);
            _settingsPanelRootGO.SetActive(false);
        }
    }
}
