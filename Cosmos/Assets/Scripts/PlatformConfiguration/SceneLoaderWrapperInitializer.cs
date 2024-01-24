using Unity.Multiplayer.Samples.Utilities;
using UnityEngine;

namespace Cosmos.PlatformConfiguration
{
    /// <summary>
    /// Add the reference of ClientCosmosLoadingScreen to SceneLoaderWrapper matching the platform type
    /// </summary>
    [ExecuteInEditMode]
    public class SceneLoaderWrapperInitializer : MonoBehaviour
    {
        [SerializeField] private ClientLoadingScreen _clientLoadingScreenFS;
        [SerializeField] private ClientLoadingScreen _clientLoadingScreenVR;

        [SerializeField] private SceneLoaderWrapper _sceneLoaderWrapper;

        [SerializeField] private PlatformConfigSO _platformConfigData;

        private void OnValidate()
        {
            _platformConfigData.OnPlatformChanged -= Assign;
            _platformConfigData.OnPlatformChanged += Assign;
        }

        public void Assign(PlatformType platformType)
        {
            switch (platformType)
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
