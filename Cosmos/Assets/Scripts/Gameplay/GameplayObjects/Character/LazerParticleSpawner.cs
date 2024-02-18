using Cosmos.Utilities;
using UnityEngine;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Spawns the lazer particles
    /// </summary>
    public class LazerParticleSpawner : MonoBehaviour
    {
        [SerializeField]
        BoolEventChannelSO m_EventToSpawn;

        [SerializeField]
        ParticleSystem[] m_particleSystems;

        [SerializeField]
        LayerMask m_LayerMask;

        [SerializeField]
        float m_RayLength;

        [SerializeField]
        Transform m_explosionSurface;

        RaycastHit[] hits = new RaycastHit[1];

        bool m_IsSpawning;

        private void Start()
        {
            m_EventToSpawn.OnEventRaised += SpawnLazer;
            m_IsSpawning = false;
            m_explosionSurface.gameObject.SetActive(false);
        }

        private void Update()
        {
            TryPlayExplosionEffectIfHit();
        }

        private void OnDestroy()
        {
            m_EventToSpawn.OnEventRaised -= SpawnLazer;
        }

        private void SpawnLazer(bool spawn)
        {
            if (spawn && !m_IsSpawning)
            {
                foreach (var particleSystem in m_particleSystems)
                {
                    particleSystem.Play();
                    m_IsSpawning = true;
                    m_explosionSurface.gameObject.SetActive(true);
                }
            }
            else if (!spawn && m_IsSpawning)
            {
                foreach (var particleSystem in m_particleSystems)
                {
                    particleSystem.Stop();
                    m_IsSpawning = false;
                    m_explosionSurface.gameObject.SetActive(false);
                }
            }
        }

        private void TryPlayExplosionEffectIfHit()
        {
            // return if not spawning
            if (!m_IsSpawning)
                return;

            // return if not hitting anything
            int hitCount = Physics.CapsuleCastNonAlloc(transform.position, transform.forward, 0.5f, transform.forward, hits, m_RayLength, m_LayerMask);

            if (hitCount == 0)
                return;

            m_explosionSurface.position = hits[0].point + hits[0].normal * 1f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (hits != null && hits.Length > 0)
            Gizmos.DrawWireSphere(hits[0].point + hits[0].normal * 1f, 0.5f);
        }
    }
}

