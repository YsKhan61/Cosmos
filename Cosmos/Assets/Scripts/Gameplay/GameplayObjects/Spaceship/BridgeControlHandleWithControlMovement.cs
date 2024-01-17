using Cosmos.SpaceShip;
using Oculus.Interaction;
using UnityEngine;


namespace Cosmos.Spaceship
{
    /// <summary>
    /// Attach this component to a control handle visual transform
    /// which is rotated by IControlHandle
    /// </summary>
    public class BridgeControlHandleWithControlMovement : MonoBehaviour
    {
        [SerializeField] private Transform _pivotTransform;

        [SerializeField]
        private ControlMovement _controlMovement = null;

        private Vector3 _projectedVectorInWorldSpace;

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/

        // [SerializeField, Tooltip("If the angle of this visual is less than deadzone limit, consider the value to be 0")]

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/

        private void Update()
        {
            _projectedVectorInWorldSpace = Vector3.ProjectOnPlane(transform.up, _pivotTransform.up).normalized;
            _projectedVectorInWorldSpace = Quaternion.AngleAxis(90f, _pivotTransform.up) * _projectedVectorInWorldSpace;    // This direction will be the axis of the torque
            _controlMovement.TorqueDirection = _projectedVectorInWorldSpace;
            _controlMovement.TorqueMultiplier = 1f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_pivotTransform.position, _projectedVectorInWorldSpace);
        }
    }

}
