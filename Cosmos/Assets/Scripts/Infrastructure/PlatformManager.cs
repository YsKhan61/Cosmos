using Cosmos.Infrastructure;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes the game objects for the current platform.
/// This script is used to hide or show game objects depending on the platform. - FlatScreen or VR.
/// </summary>
public class PlatformManager : MonoBehaviour
{
    [SerializeField] private PlatformConfigSO _platformConfigData;

    [SerializeField] private List<GameObject> _gameObjectsForFlatscreen;
    [SerializeField] private List<GameObject> _gameObjectsForVR;

    private void Awake()
    {
        switch(_platformConfigData.Platform)
        {
            case Platform.FlatScreen:
                foreach (GameObject gameObject in _gameObjectsForFlatscreen)
                {
                    gameObject.SetActive(true);
                }
                foreach (GameObject gameObject in _gameObjectsForVR)
                {
                    gameObject.SetActive(false);
                }
                break;

            case Platform.VR:
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
