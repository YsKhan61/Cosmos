using Cosmos.Utilities;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    public class UISettingsPanel : MonoBehaviour
    {
        [SerializeField]
        private Slider _masterVolumeSlider;

        [SerializeField]
        private Slider _musicVolumeSlider;

        private void OnEnable()
        {
            // Note that we initialize the slider BEFORE we listen for changes, (so we don't get notified of our own change!)
            _masterVolumeSlider.value = ClientPrefs.GetMasterVolume();
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderChanged);

            // initialize music slider smartly.
            _musicVolumeSlider.value = ClientPrefs.GetMusicVolume();
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderChanged);
        }

        private void OnMasterVolumeSliderChanged(float value)
        {
            ClientPrefs.SetMasterVolume(value);
            
        }

        private void OnMusicVolumeSliderChanged(float value)
        {
            ClientPrefs.SetMusicVolume(value);
        }
    }
}
