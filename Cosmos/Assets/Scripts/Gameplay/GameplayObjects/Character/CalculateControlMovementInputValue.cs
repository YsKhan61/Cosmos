using Cosmos.Utilities;
using Oculus.Interaction;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Calculates the control direction from the control handle visual's rotation respect to pivotTransform
    /// which is rotated by IControlHandle
    /// </summary>
    public class CalculateControlMovementInputValue : MonoBehaviour
    {
        [SerializeField] private Transform _pivotTransform;

        [SerializeField] private Vector3DataSO _controlMovementInput;

        private Vector3 _projectedVectorInWorldSpace;

        private void Update()
        {
            _projectedVectorInWorldSpace = Vector3.ProjectOnPlane(transform.up, _pivotTransform.up).normalized;
            _projectedVectorInWorldSpace = Quaternion.AngleAxis(90f, _pivotTransform.up) * _projectedVectorInWorldSpace;    // This direction will be the axis of the torque
            _controlMovementInput.value = _projectedVectorInWorldSpace;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_pivotTransform.position, _projectedVectorInWorldSpace);
        }
    }

}
