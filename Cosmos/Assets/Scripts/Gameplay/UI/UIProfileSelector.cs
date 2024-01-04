using Cosmos.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;


namespace Cosmos.Gameplay.UI
{
    public class UIProfileSelector : MonoBehaviour
    {
        // Authentication service only accepts profile names of 30 characters or under.
        private const int AUTHENTICATION_MAX_PROFILE_LENGTH = 30;

        [SerializeField]
        private ProfileListItemUI _profileListItemUIPrototype;

        [SerializeField]
        private TMP_InputField _newProfileInputField;

        [SerializeField]
        private Button _createProfileButton;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private Graphic _emptyProfileListLabel;

        private List<ProfileListItemUI> _profileListItems = new List<ProfileListItemUI>();

        [Inject] IObjectResolver _resolver;
        [Inject] ProfileManager _profileManager;

        private void Awake()
        {
            _profileListItemUIPrototype.gameObject.SetActive(false);
            Hide();
            _createProfileButton.interactable = false;
        }

        /// <summary>
        /// Added to the InputField component's OnValueChanged event in the inspector.
        /// </summary>
        public void SanitizeProfileNameInputText()
        {
            _newProfileInputField.text = SanitizeProfileName(_newProfileInputField.text);
            _createProfileButton.interactable = !string.IsNullOrEmpty(_newProfileInputField.text) && !_profileManager.AvailableProfiles.Contains(_newProfileInputField.text);
        }

        public void OnNewProfileButtonPressed()
        {
            string profile = _newProfileInputField.text;
            if (!string.IsNullOrEmpty(profile) && !_profileManager.AvailableProfiles.Contains(profile))
            {
                _profileManager.CreateProfile(profile);
                // _profileManager.Profile = profile; // - added to CreateProfile Method of ProfileManager
            }
            else
            {
                PopupManager.ShowPopupPanel(
                    "Could not create new profile",
                    "A profile already exists with this same name. Select one of the already existing profiles or create a new one with a different name.");
            }
        }


        public void InitializeUI()
        {
            EnsureNumberOfActiveUISlots(_profileManager.AvailableProfiles.Count);
            for (int i = 0, count = _profileManager.AvailableProfiles.Count; i < count; i++)
            {
                _profileListItems[i].SetProfileName(_profileManager.AvailableProfiles[i]);
            }

            _emptyProfileListLabel.enabled = _profileManager.AvailableProfiles.Count == 0;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _newProfileInputField.text = "";
            InitializeUI();
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        private string SanitizeProfileName(string dirtyString)
        {
            string output = Regex.Replace(dirtyString, "[^a-zA-Z0-9_]", "");
            return output[..Mathf.Min(output.Length, AUTHENTICATION_MAX_PROFILE_LENGTH)];
        }

        private void EnsureNumberOfActiveUISlots(int requiredNumber)
        {
            int delta = requiredNumber - _profileListItems.Count;

            for (int i = 0; i < delta; i++)
            {
                CreateProfileListItem();
            }

            for (int i = 0, count = _profileListItems.Count; i < count; i++)
            {
                _profileListItems[i].gameObject.SetActive(i < requiredNumber);
            }
        }

        private void CreateProfileListItem()
        {
            ProfileListItemUI profileListItemUI = 
                Instantiate(_profileListItemUIPrototype.gameObject, _profileListItemUIPrototype.transform.parent).GetComponent<ProfileListItemUI>();

            _profileListItems.Add(profileListItemUI);
            profileListItemUI.gameObject.SetActive(true);
            _resolver.Inject(profileListItemUI);
        }
    }
}


