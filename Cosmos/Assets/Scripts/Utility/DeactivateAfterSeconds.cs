using System.Collections;
using UnityEngine;


namespace Cosmos
{
    public class DeactivateAfterSeconds : MonoBehaviour
    {
        [SerializeField] private float _secondsToWait = 3f;

        private void OnEnable()
        {
            StartCoroutine(DeactivateAfterSecondsRoutine());
        }

        IEnumerator DeactivateAfterSecondsRoutine()
        {
            yield return new WaitForSeconds(_secondsToWait);
            gameObject.SetActive(false);
        }
    }

}
