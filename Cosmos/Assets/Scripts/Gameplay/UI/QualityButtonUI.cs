using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmos.Gameplay.UI
{
    public class QualityButtonUI : MonoBehaviour
    {
        [SerializeField] private Button _qualityButton;
        [SerializeField] private TMP_Text _qualityButtonText;

        private void OnEnable()
        {
            _qualityButton.onClick.AddListener(OnQualityButtonClicked);
        }

        private void Start()
        {
            int index = QualitySettings.GetQualityLevel();
            _qualityButtonText.text = QualitySettings.names[index];
        }

        private void OnDestroy()
        {
            _qualityButton.onClick.RemoveListener(OnQualityButtonClicked);
        }

        private void OnQualityButtonClicked()
        {
            int qualityLevels = QualitySettings.names.Length - 1;
            int currentLevel = QualitySettings.GetQualityLevel();

            if (currentLevel < qualityLevels)
            {
                QualitySettings.IncreaseLevel();
                _qualityButtonText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
            }
            else
            {
                QualitySettings.SetQualityLevel(0);
                _qualityButtonText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
            }
        }
    }
}
