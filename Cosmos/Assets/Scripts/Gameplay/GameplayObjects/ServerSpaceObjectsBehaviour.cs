using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


namespace Cosmos.Gameplay.GameplayObjects
{
    /// <summary>
    /// Various Space Objects like Asteroids, Meteors, Planets etc can randomly move in the space.
    /// </remarks>
    public class ServerSpaceObjectsBehaviour : MonoBehaviour
    {
        struct SpaceObject
        {
            public NetworkObject NetworkObject;
            public Transform NetworkObjectTransform;
            public Vector3 CenterOfOrbit;
            public Vector3 OrbitAxis;
            public float OrbitDirection; // 1 or -1
            public float RotateSpeed;
            public float RevolveSpeed;
        }

        [SerializeField]
        NetworkObject[] m_PrefabNOs;

        [SerializeField]
        int m_SpawnCount = 10;

        [SerializeField, Tooltip("Spawn within min(X) max(Y) range")]
        Vector2 m_SpawnRange;

        [SerializeField, Tooltip("Orbit center within min(X) max(Y) range")]
        Vector2 m_OrbitCenterRange;

        [SerializeField, Tooltip("Orbit radius within min(X) max(Y) range")]
        Vector2 m_RotateSpeedRange;

        [SerializeField, Tooltip("Spawn speed within min(X) max(Y) range")]
        Vector2 m_RevolveSpeedRange;

        private SpaceObject[] m_spawnedSpaceObjects;

        public void Awake()
        {
            if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer &&
                NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            }
            else
            {
                // NOTE: This is a server only script. Destroy it if it's not running on server.
                // Destroy(gameObject);             // Uncomment this line when we are ready to test on server.
            }
        }

        private void Update()
        {
            RotateAndRevolveSpaceObjects();
        }

        void OnDestroy()
        {
            if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer &&
                NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManagerOnOnLoadEventCompleted;
            }
        }

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            SpawnRandomAsteroids();
        }

        [ContextMenu("Spawn Random Asteroids")]
        void SpawnRandomAsteroids()
        {
            m_spawnedSpaceObjects = new SpaceObject[m_SpawnCount];

            if (m_PrefabNOs == null || m_PrefabNOs.Length == 0)
            {
                return;
            }

            for (int i = 0; i < m_SpawnCount; i++)
            {
                SpawnNetworkObject(m_PrefabNOs[Random.Range(0, m_PrefabNOs.Length)], out NetworkObject networkObject);
                if (networkObject == null)
                {
                    continue;
                }

                m_spawnedSpaceObjects[i].NetworkObject = networkObject;
                m_spawnedSpaceObjects[i].NetworkObjectTransform = networkObject.transform;
                m_spawnedSpaceObjects[i].CenterOfOrbit = Random.insideUnitSphere;
                m_spawnedSpaceObjects[i].OrbitAxis = Random.insideUnitSphere;
                m_spawnedSpaceObjects[i].OrbitDirection = Random.Range(0, 2) == 0 ? 1 : -1;
                m_spawnedSpaceObjects[i].RotateSpeed = Random.Range(m_RotateSpeedRange.x, m_RotateSpeedRange.y);
                m_spawnedSpaceObjects[i].RevolveSpeed = Random.Range(m_RevolveSpeedRange.x, m_RevolveSpeedRange.y);
            }
        }

        private void SpawnNetworkObject(NetworkObject prefabNO, out NetworkObject spawnedNetworkObject)
        {
            Vector3 randomPos = Random.insideUnitSphere;
            randomPos *= Random.Range(m_SpawnRange.x, m_SpawnRange.y);

            spawnedNetworkObject = Instantiate(prefabNO, randomPos, transform.rotation, null);
            SceneManager.MoveGameObjectToScene(spawnedNetworkObject.gameObject,
                SceneManager.GetSceneByName(gameObject.scene.name));
            spawnedNetworkObject.transform.localScale = transform.lossyScale;
            spawnedNetworkObject.Spawn(destroyWithScene: true);
        }

        private void RotateAndRevolveSpaceObjects()
        {
            if (m_spawnedSpaceObjects == null)
            {
                return;
            }

            for (int i = 0; i < m_spawnedSpaceObjects.Length; i++)
            {
                if (m_spawnedSpaceObjects[i].NetworkObject == null)
                {
                    continue;
                }

                if (m_spawnedSpaceObjects[i].NetworkObject.IsSpawned == false)
                {
                    continue;
                }

                m_spawnedSpaceObjects[i].NetworkObjectTransform.Rotate(
                    m_spawnedSpaceObjects[i].OrbitAxis,
                    m_spawnedSpaceObjects[i].OrbitDirection * m_spawnedSpaceObjects[i].RotateSpeed * Time.deltaTime);

                m_spawnedSpaceObjects[i].NetworkObjectTransform.RotateAround(
                    m_spawnedSpaceObjects[i].CenterOfOrbit,
                    m_spawnedSpaceObjects[i].OrbitAxis,
                    m_spawnedSpaceObjects[i].OrbitDirection * m_spawnedSpaceObjects[i].RevolveSpeed * Time.deltaTime);
            }
        }

        private void OnDrawGizmos()
        {
            if (m_spawnedSpaceObjects == null)
            {
                return;
            }

            for (int i = 0; i < m_spawnedSpaceObjects.Length; i++)
            {
                if (m_spawnedSpaceObjects[i].NetworkObject == null)
                {
                    continue;
                }

                if (m_spawnedSpaceObjects[i].NetworkObject.IsSpawned == false)
                {
                    continue;
                }

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(m_spawnedSpaceObjects[i].CenterOfOrbit, 0.1f);
            }
        }
    }
}

