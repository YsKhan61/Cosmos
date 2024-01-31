using UnityEngine;

namespace Cosmos.Utilities
{
    [CreateAssetMenu(fileName = "Vector2Data", menuName = "ScriptableObjects/DataContainers/Vector2DataSO")]
    public class Vector2DataSO : ScriptableObject
    {
        public Vector2 value;
    }
}