using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cosmos.Test
{
    [ExecuteInEditMode]
    public class EmissiveStarsPlacer : MonoBehaviour
    {
        [SerializeField]
        GameObject[] m_Stars;

        [SerializeField]
        Vector2 m_PositionRange;

        [ContextMenu("Place Stars")]
        public void PlaceStars()
        {
            for (int i = 0; i < m_Stars.Length; i++)
            {
                // Choose a random direction from center
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                // Choose a random distance from center
                float distance = Random.Range(m_PositionRange.x, m_PositionRange.y);
                // Place the star
                m_Stars[i].transform.position = direction * distance;

                // Look at center
                // m_Stars[i].transform.LookAt(Vector3.zero);
                LookAtWorldCenter(m_Stars[i].transform);
            }
        }

        private void LookAtWorldCenter(Transform transform)
        {
            Vector3 direction = Vector3.zero - transform.position;
            float angle = Mathf.Acos(Vector3.Dot(transform.forward, direction));
            Vector3 axis = Vector3.Cross(transform.forward, direction);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
        }
    }
}

