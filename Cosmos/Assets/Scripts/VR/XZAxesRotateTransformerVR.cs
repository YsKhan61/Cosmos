using Cosmos.SpaceShip;
using Oculus.Interaction;
using System.Collections;
using UnityEngine;


namespace Cosmos.VR
{
    /// <summary>
    /// Used to rotate a transform around any axis so that
    /// it's specified axis is oriented towards the grabber
    /// </summary>
    public class XZAxesRotateTransformerVR : MonoBehaviour, ITransformer, IControlHandle
    {
        private enum Axis
        {
            Right = 0,
            Up = 1,
            Forward = 2
        }        

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;
        public Transform PivotTransform => _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The axis of the visual transform that need to be oriented to the grabber")]
        private Axis _axisOfVisualTransformToOrient = Axis.Up;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")] 
        private int _angleConstraint = 10;
        public int AngleConstraint => _angleConstraint;

        private IGrabbable _grabbable;
        private Vector3 _localAxisToOrient;

        private Quaternion _initialVisualLocalRotation;
        private Coroutine _resetOrientationRoutine;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
            _localAxisToOrient = Vector3.zero;
            _localAxisToOrient[(int)_axisOfVisualTransformToOrient] = 1;
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;
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
            // Calculate the angle between the visual transform's axis of
            // orientation and pivot transform's Y axis
            /*float angleBetweenAxisToOrientAndPivotTransformYAxis = 
                Vector3.Angle(axisToOrient, _pivotTransform.up);*/

            Vector3 vectorFromPivotToGrabberInWorldSpace = 
                _grabbable.GrabPoints[0].position - _pivotTransform.position;

            Vector3 axisToOrient = _visualTransform.TransformDirection(_localAxisToOrient);


            float angleBetweenVectorFromPivotToGrabberInWorldSpaceAndPivotTransformYAxis =
                Vector3.Angle(vectorFromPivotToGrabberInWorldSpace, _pivotTransform.up);

            if (angleBetweenVectorFromPivotToGrabberInWorldSpaceAndPivotTransformYAxis > _angleConstraint)
            {
                return;
            }

            _visualTransform.rotation = 
                Quaternion.FromToRotation(
                    axisToOrient, 
                    vectorFromPivotToGrabberInWorldSpace) * _visualTransform.rotation;
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
                Quaternion startLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

                _visualTransform.rotation =
                    _pivotTransform.rotation * Quaternion.Slerp(startLocalRotation, _initialVisualLocalRotation, time / duration);

                yield return null;
            }
        }
    }

}
