using System.Collections;
using UnityEngine;


namespace GalacticKittenVR.Spaceship
{
    public class BoundaryRestriction : MonoBehaviour
    {
        [SerializeField, Tooltip("The interval to check the boundary limit")]
        private float _checkInterval = 1f;

        [SerializeField, Tooltip("The distance to boundary from center of scene")]
        private float _boundaryDistance = 100f;

        [SerializeField, Tooltip("The hyperspace tunnel game object")]
        private GameObject _huperSpaceTunnelGO;

        private WaitForSeconds _checkIntervalWait;
        private float _boundaryDistanceSqr;

        private void Start()
        {
            _boundaryDistanceSqr = _boundaryDistance * _boundaryDistance;
            _checkIntervalWait = new WaitForSeconds(_checkInterval);
            StartCoroutine(BoundaryCheckRoutine());
        }

        IEnumerator BoundaryCheckRoutine()
        {
            while (true)
            {
                yield return _checkIntervalWait;

                if (Vector3.SqrMagnitude(transform.position) > _boundaryDistanceSqr)
                {
                    ReachedBoundary();
                }
            }
        }

        private void ReachedBoundary()
        {
            _huperSpaceTunnelGO.SetActive(true);
            transform.position = Vector3.zero;
        }
    }
}
