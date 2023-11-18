using Oculus.Interaction;
using UnityEngine;


namespace GalacticKittenVR.Spaceship
{
    public class ControlHandleGrabbbable : MonoBehaviour, ITransformer
    {
        private const int _ANGLE_LIMIT = 30;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;
        private Quaternion _lastOrientationOfGrabberRelToPivot;
        private IGrabbable _grabbable;

        public void Initialize(IGrabbable grabbable)
        {
            _grabbable = grabbable;
        }

        public void BeginTransform()
        {
           
        }

        public void UpdateTransform()
        {
            Vector3 vectorFromPivotToGrabberInWorldSpace = _grabbable.GrabPoints[0].position - _pivotTransform.position;
            _visualTransform.rotation = Quaternion.FromToRotation(_visualTransform.up, vectorFromPivotToGrabberInWorldSpace) * _visualTransform.rotation;
        }

        public void EndTransform()
        {

        }
    }

}
