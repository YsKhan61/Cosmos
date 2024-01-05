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

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/

        // [SerializeField, Tooltip("If the angle of this visual is less than deadzone limit, consider the value to be 0")]

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/

        private void Update()
        {
            Vector3 v = _pivotTransform.InverseTransformDirection(transform.up);

            _controlMovement.TorqueDirection =
                new Vector3(
                    v.x, 
                    0f, 
                    v.z).normalized;

            _controlMovement.TorqueMultiplier = 1f;
        }
    }

}
