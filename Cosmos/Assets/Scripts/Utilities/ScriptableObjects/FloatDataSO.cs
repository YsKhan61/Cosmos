using UnityEngine;

namespace Cosmos.Utilities
{
    [CreateAssetMenu(fileName = "FloatData", menuName = "ScriptableObjects/DataContainers/FloatDataSO")]
    public class FloatDataSO : ScriptableObject
    {
        public float value;
    }
}