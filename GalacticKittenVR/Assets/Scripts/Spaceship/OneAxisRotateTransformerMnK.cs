using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Spaceship
{
    /// <summary>
    /// Used to rotate a transform around a single axis based on the MnK input
    /// </summary>
    public class OneAxisRotateTransformerMnK : MonoBehaviour
    {
        private const int _ANGLE_LIMIT = 10;

        [SerializeField] private InputActionReference _throttleForwardInputActionReference;
        [SerializeField] private InputActionReference _throttleBackwardInputActionReference;

        

    }

}
