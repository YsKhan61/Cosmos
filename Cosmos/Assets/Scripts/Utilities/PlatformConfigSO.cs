using UnityEngine;

namespace Cosmos.Utilities
{
    public enum PlatformType
    {
        FlatScreen,
        VR
    }

    [CreateAssetMenu(fileName = "PlatformConfig", menuName = "ScriptableObjects/PlatformConfigSO")]
    public class PlatformConfigSO : ScriptableObject
    {
        [SerializeField] private PlatformType _platform;
        public PlatformType Platform => _platform;
    }
}
