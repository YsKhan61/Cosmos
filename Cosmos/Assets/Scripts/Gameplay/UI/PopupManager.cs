using Cosmos.Utilities;
using System.Collections.Generic;
using UnityEngine;


namespace Cosmos.Gameplay.UI
{
    /// <summary>
    /// Handles the display of Popup messages. 
    /// Instantiates and reuses popup panel prefabs to allow displaying multiple messages in succession
    /// </summary>
    public class PopupManager : SingletonPersistent<PopupManager>
    {
        private const float OFFSET = 30;
        private const float MAX_OFFSET = 200;

        [SerializeField]
        private GameObject _popupPanelPrefab;

        [SerializeField]
        private GameObject _canvas;

        private List<PopupPanel> _popupPanels = new List<PopupPanel>();

        /// <summary>
        /// Displays a popup panel message with the specified title and main text
        /// </summary>
        /// <param name="titleText">The title at the top of the panel</param>
        /// <param name="mainText">the text just under the title- the main body of text</param>
        /// <param name="closeableByUser">Whether or not user can close the panel with a close button.</param>
        /// <returns></returns>
        public static PopupPanel ShowPopupPanel(string titleText, string mainText, bool closeableByUser = true)
        {
            if (Instance != null)
            {
                return Instance.DisplayPopupPanel(titleText, mainText, closeableByUser);
            }

            Debug.LogError($"No popuppanel instance found. Cannot display message: {titleText}: {mainText}");
            return null;
        }

        private PopupPanel DisplayPopupPanel(string titleText, string mainText, bool closeableByUser)
        {
            PopupPanel panel = GetNextAvailablePopupPanel();
            
            if (panel != null)
            {
                panel.SetupPopupPanel(titleText, mainText, closeableByUser);
            }

            return panel;
        }

        private PopupPanel GetNextAvailablePopupPanel()
        {
            PopupPanel panel = _popupPanels.Find(p => !p.gameObject.activeSelf);

            if (panel == null)
            {
                panel = Instantiate(_popupPanelPrefab, transform).GetComponent<PopupPanel>();
                panel.gameObject.transform.position += new Vector3(1, -1) * (OFFSET * _popupPanels.Count % MAX_OFFSET);
                _popupPanels.Add(panel);
            }

            return panel;
        }
    }

}
