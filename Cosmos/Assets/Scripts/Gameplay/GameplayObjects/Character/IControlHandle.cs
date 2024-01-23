using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public interface IControlHandle
    {
        public Transform PivotTransform { get; }
    }
}
