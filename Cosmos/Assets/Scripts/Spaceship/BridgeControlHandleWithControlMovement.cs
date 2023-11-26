using Cosmos.SpaceShip;
using Oculus.Interaction;
using UnityEngine;


namespace Cosmos.Spaceship
{
    public class BridgeControlHandleWithControlMovement : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IControlHandle))] 
        private Object _controlTransformerO = null;

        [SerializeField]
        private ControlMovement _controlMovement = null;

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/

        [SerializeField, Tooltip("If the angle of this visual is less than deadzone limit, consider the value to be 0")]

        /*[SerializeField, Tooltip("1 or -1 : to invert the control visual angle value")]
        private int _invertMultiplier = 1;*/
        private IControlHandle _controlHandle = null;

        private void Start()
        {
            if (_controlTransformerO == null)
            {
                Debug.LogError("Control Transformer is not set");
                enabled = false;
                return;
            }
            _controlHandle = _controlTransformerO as IControlHandle;
        }

        private void Update()
        {
            Vector3 v = _controlHandle.PivotTransform.InverseTransformDirection(transform.up);

            _controlMovement.TorqueDirection =
                new Vector3(
                    v.x, 
                    0f, 
                    v.z).normalized;

            _controlMovement.TorqueMultiplier = 1f;
        }
    }

}
