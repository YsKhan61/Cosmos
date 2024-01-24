
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.PlatformConfiguration
{
    /// <summary>
    /// Initializes the game objects for the current platform.
    /// This script is used to hide or show game objects depending on the platform. - FlatScreen or VR.
    /// </summary>
    [ExecuteInEditMode]
    public class PlatformManagerCharSelectScene : PlatformManager
    {
        [SerializeField] private PlatformConfigSO _platformConfigData;

        [SerializeField] private GameObject[] _gameObjectsForFlatscreen;
        [SerializeField] private GameObject[] _gameObjectsForVR;

        private void OnEnable()
        {
            _platformConfigData.OnPlatformChanged -= ChangePlatform;
            _platformConfigData.OnPlatformChanged += ChangePlatform;
        }

        /// <summary>
        /// Setup at editor time, when the platform is changed in the PlatformConfigSO.
        /// </summary>
        protected override void ChangePlatform(PlatformType platformType)
        {
            switch (platformType)
            {
                case PlatformType.FlatScreen:
                    foreach (GameObject gameObject in _gameObjectsForFlatscreen)
                    {
                        gameObject.SetActive(true);
                    }
                    foreach (GameObject gameObject in _gameObjectsForVR)
                    {
                        gameObject.SetActive(false);
                    }
                    break;

                case PlatformType.VR:
                    foreach (GameObject gameObject in _gameObjectsForFlatscreen)
                    {
                        gameObject.SetActive(false);
                    }
                    foreach (GameObject gameObject in _gameObjectsForVR)
                    {
                        gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }
}
