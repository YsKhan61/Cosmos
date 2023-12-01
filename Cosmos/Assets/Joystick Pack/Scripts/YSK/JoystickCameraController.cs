using UnityEngine;

namespace Cosmos.TouchScreen
{
    public class JoystickCameraController : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] VariableJoystick variableJoyStick;

        [SerializeField] Transform camTransform;

        Vector3 _direction;

        private void Awake()
        {
            // camTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        private void LateUpdate()
        {
            /*
            print("joystick horizontal: " + dynamicJoystick.Horizontal + 
                " joystick vertical: " + dynamicJoystick.Vertical);

            */

            _direction = new Vector3(-variableJoyStick.Vertical, variableJoyStick.Horizontal, 0) * speed;
            camTransform.localEulerAngles += _direction;

        }
    }
}

