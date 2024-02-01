using Cosmos.Utilities;
using Oculus.Interaction;
using System.Collections;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Calculate the control input based on the grabber's orientation respect to the pivot
    /// </summary>
    public class CalculateControlInputVR : MonoBehaviour, ITransformer, IControlHandle
    {
        private enum Axis : int      // In a Vector3 (x, y, z) the axes are (0, 1, 2)
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }

        [SerializeField]
        private Vector3DataSO _controlInput;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;
        public Transform PivotTransform => _pivotTransform;                 // not necessary

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")]
        private FloatDataSO _angleConstraint;

        private IGrabbable _grabbable;
        private Coroutine _resetOrientationRoutine;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;

            _controlInput.value = Vector3.zero;
        }

        public void BeginTransform()
        {
            if (_resetOrientationRoutine != null)
            {
                StopCoroutine(_resetOrientationRoutine);
                _resetOrientationRoutine = null;
            }
        }

        public void UpdateTransform()
        {

            Vector3 vectorFromPivotToGrabberInWorldSpace =
                _grabbable.GrabPoints[0].position - _pivotTransform.position;

            Quaternion deltaRotationXZ = Quaternion.FromToRotation(_pivotTransform.up, vectorFromPivotToGrabberInWorldSpace);
            Quaternion deltaRotationY = Quaternion.FromToRotation(_pivotTransform.forward, _grabbable.GrabPoints[0].forward);

            _controlInput.value = deltaRotationXZ.eulerAngles;
            _controlInput.value.y = deltaRotationY.eulerAngles.y;

            
            if (_controlInput.value.x > 180)
            {
                _controlInput.value.x -= 360;
            }
            if (_controlInput.value.y > 180)
            {
                _controlInput.value.y -= 360;
            }
            if (_controlInput.value.z > 180)
            {
                _controlInput.value.z -= 360;
            }

            _controlInput.value.x = Mathf.Clamp(_controlInput.value.x, -_angleConstraint.value  , _angleConstraint.value);
            _controlInput.value.y = Mathf.Clamp(_controlInput.value.y, -_angleConstraint.value, _angleConstraint.value);
            _controlInput.value.z = Mathf.Clamp(_controlInput.value.z, -_angleConstraint.value, _angleConstraint.value);

            _controlInput.value.x = Mathf.Lerp(-1, 1, Mathf.InverseLerp(-_angleConstraint.value, _angleConstraint.value, _controlInput.value.x));
            _controlInput.value.y = Mathf.Lerp(-1, 1, Mathf.InverseLerp(-_angleConstraint.value, _angleConstraint.value, _controlInput.value.y));
            _controlInput.value.z = Mathf.Lerp(-1, 1, Mathf.InverseLerp(-_angleConstraint.value, _angleConstraint.value, _controlInput.value.z));
        }

        public void EndTransform()
        {
            _resetOrientationRoutine ??= StartCoroutine(ResetOrientationRoutine());
        }

        IEnumerator ResetOrientationRoutine()
        {
            float time = 0f;
            float duration = 0.5f;

            while (time < duration)
            {
                time += Time.deltaTime;

                _controlInput.value = Vector3.Lerp(_controlInput.value, Vector3.zero, time / duration);

                if (Vector3.SqrMagnitude(_controlInput.value) < 0.01f)
                {
                    _controlInput.value = Vector3.zero;
                    _resetOrientationRoutine = null;
                    yield break;
                }

                yield return null;
            }
        }
    }

}
