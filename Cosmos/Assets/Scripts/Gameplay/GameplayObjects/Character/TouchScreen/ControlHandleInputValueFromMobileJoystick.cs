using Cosmos.Utilities;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Gets the input value from the touch screen joysticks (pitch, yaw, roll) and stores it in a Vector3DataSO.
    /// </summary>
    public class ControlHandleInputValueFromMobileJoystick : MonoBehaviour
    {
        [SerializeField] private VariableJoystick _pitchStick;
        [SerializeField] private VariableJoystick _yawStick;
        [SerializeField] private VariableJoystick _rollStick;
        

        [SerializeField] private Vector3DataSO _controlHandleInputData;

        [SerializeField]
        private bool _invertPitch = false;
        [SerializeField]
        private bool _invertYaw = false;
        [SerializeField]
        private bool _invertRoll = false;

        private void Update()
        {
            _controlHandleInputData.value = new Vector3(
                _pitchStick.Vertical * (_invertPitch ? -1 : 1), 
                _yawStick.Horizontal * (_invertYaw ? -1 : 1), 
                _rollStick.Horizontal * (_invertRoll ? -1 : 1));
        }
    }
}
