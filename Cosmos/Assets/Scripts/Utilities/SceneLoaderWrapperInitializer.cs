using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Cosmos.Utilities
{
    /// <summary>
    /// Add the reference of ClientCosmosLoadingScreen to SceneLoaderWrapper matching the platform type
    /// </summary>
    public class SceneLoaderWrapperInitializer : NetworkBehaviour
    {
        [SerializeField] private ClientLoadingScreen _clientLoadingScreenFS;
        [SerializeField] private ClientLoadingScreen _clientLoadingScreenVR;

        [SerializeField] private SceneLoaderWrapper _sceneLoaderWrapper;

        [SerializeField] private PlatformConfigSO _platformConfigData;

        public void Assign()
        {
            switch (_platformConfigData.Platform)
            {
                case PlatformType.FlatScreen:
                    _sceneLoaderWrapper.clientLoadingScreen = _clientLoadingScreenFS;
                    break;
                case PlatformType.VR:
                    _sceneLoaderWrapper.clientLoadingScreen = _clientLoadingScreenVR;
                    break;
                default:
                    break;
            }
        }
    }
}
