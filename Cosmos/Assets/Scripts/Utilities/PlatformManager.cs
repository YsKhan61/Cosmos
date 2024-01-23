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
        public UnityEvent OnStart;

        [SerializeField] private PlatformConfigSO _platformConfigData;

        [SerializeField] private List<GameObject> _gameObjectsForFlatscreen;
        [SerializeField] private List<GameObject> _gameObjectsForVR;

        

        /// <summary>
        /// Setup at start, coz we need the NetworkManager and some other stuff to be initialized first.
        /// </summary>
        private void Start()
        {
            OnStart?.Invoke();              // In the Startup scene, first initialize the SceneLoaderWrapper, then activate the ClientLoadingScreen gameobject. We want ClientLoading Screen gameobject's start() to run after SceneLoaderWrapper's Initialize() is done.

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
        }
    }
}
