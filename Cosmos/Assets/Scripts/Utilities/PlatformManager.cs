using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmos.Utilities
{
    /// <summary>
    /// Initializes the game objects for the current platform.
    /// This script is used to hide or show game objects depending on the platform. - FlatScreen or VR.
    /// </summary>
    public class PlatformManager : MonoBehaviour
    {
        [SerializeField] private PlatformConfigSO _platformConfigData;

        [SerializeField] private List<GameObject> _gameObjectsForFlatscreen;
        [SerializeField] private List<GameObject> _gameObjectsForVR;

        public UnityEvent onAwake;

        private void Awake()
        {
            switch (_platformConfigData.Platform)
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

            onAwake?.Invoke();
        }
    }
}
