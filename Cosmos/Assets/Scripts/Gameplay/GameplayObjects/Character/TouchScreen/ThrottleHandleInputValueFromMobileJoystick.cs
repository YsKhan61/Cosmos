using Cosmos.Utilities;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Attach this to the mobile joystick to get throttle value from it.
    /// </summary>
    public class ThrottleHandleInputValueFromMobileJoystick : MonoBehaviour
    {
        [SerializeField] private VariableJoystick _joystick;

        [SerializeField] private FloatDataSO _throttleHandleInputData;

        private void Update()
        {
            _throttleHandleInputData.value = _joystick.Vertical;
        }
    }

}
