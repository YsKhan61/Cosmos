using Cosmos.Utilities;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    public class ControlHandleInputValueFromMobileJoystick : MonoBehaviour
    {
        [SerializeField] private VariableJoystick _joystick;

        [SerializeField] private Vector2DataSO _controlHandleInputData;

        private void Update()
        {
            _controlHandleInputData.value = _joystick.Direction;
        }
    }
}
