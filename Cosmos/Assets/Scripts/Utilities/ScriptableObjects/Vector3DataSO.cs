using UnityEngine;

namespace Cosmos.Utilities
{
    [CreateAssetMenu(fileName = "Vector3Data", menuName = "ScriptableObjects/DataContainers/Vector3DataSO")]
    public class Vector3DataSO : ScriptableObject
    {
        public Vector3 value;
    }
}