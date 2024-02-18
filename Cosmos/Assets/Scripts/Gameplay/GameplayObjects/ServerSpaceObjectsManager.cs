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
    public class ServerSpaceObjectsManager : MonoBehaviour
    {
        [SerializeField]
        NetworkObject[] m_PrefabNOs;

        [SerializeField]
        int m_SpawnCount = 10;

        [SerializeField, Tooltip("Spawn within min(X) max(Y) range")]
        Vector2 m_SpawnRange;

        public void Awake()
        {
            if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer &&
                NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            }
            else
            {
                // Destroy(gameObject);
            }
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
            if (m_PrefabNOs == null || m_PrefabNOs.Length == 0)
            {
                return;
            }

            for (int i = 0; i < m_SpawnCount; i++)
            {
                NetworkObject networkObject = m_PrefabNOs[Random.Range(0, m_PrefabNOs.Length)];
                SpawnNetworkObject(networkObject);
            }
        }

        private void SpawnNetworkObject(NetworkObject prefabNO)
        {
            Vector3 randomPos = Random.insideUnitSphere;
            randomPos *= Random.Range(m_SpawnRange.x, m_SpawnRange.y);

            var instantiatedNetworkObject = Instantiate(prefabNO, randomPos, transform.rotation, null);
            SceneManager.MoveGameObjectToScene(instantiatedNetworkObject.gameObject,
                SceneManager.GetSceneByName(gameObject.scene.name));
            instantiatedNetworkObject.transform.localScale = transform.lossyScale;
            instantiatedNetworkObject.Spawn(destroyWithScene: true);
        }
    }
}

