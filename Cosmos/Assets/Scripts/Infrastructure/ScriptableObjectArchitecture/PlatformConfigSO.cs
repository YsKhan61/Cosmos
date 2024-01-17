using UnityEngine;

namespace Cosmos.Infrastructure
{
    public enum Platform
    {
        FlatScreen,
        VR
    }

    [CreateAssetMenu(fileName = "PlatformConfig", menuName = "ScriptableObjects/PlatformConfigSO")]
    public class PlatformConfigSO : ScriptableObject
    {
        [SerializeField] private Platform _platform;
        public Platform Platform => _platform;
    }
}
