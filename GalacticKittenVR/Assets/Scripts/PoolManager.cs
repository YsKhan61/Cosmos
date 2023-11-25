using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Cosmos
{
    /// <summary>
    /// GameObjects that are spawned and despawned are pooled
    /// and they must be inherited from ISpawnable interface
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _spawnablesGO;

        // Expose the spawnables to other scripts
        private ISpawnable[] _spawnables;
        private Dictionary<string, ObjectPool<ISpawnable>> _objectsPool = new Dictionary<string, ObjectPool<ISpawnable>>();

        void Awake()
        {
            FetchSpawnableFromGameObjects();

            FillupPool();
        }

        public ISpawnable GetObjectFromPool(string name)
        {
            // search the pool for the object
            if (!_objectsPool.TryGetValue(name, out var pool))
            {
                Debug.LogError("No pool found for " + name);
                return null;
            }

            // get the object from the pool
            var spawnable = pool.Get();
            return spawnable;
        }

        public void ReleaseObjectToPool(ISpawnable spawnable)
        {
            if (spawnable == null)
            {
                Debug.LogError("No spawnable found to release to pool!");
                return;
            }

            // search for the pool for the object in the dictionary,
            // if not found, create a new pool and add the spawnable to the pool
            // if found, just add the spawnable to the pool
            if (!_objectsPool.TryGetValue(spawnable.Name, out var pool))
            {
                Debug.Log("No pool found to release the spawnable! Creating a new one!");

                pool = CreatePoolForSpawnable(spawnable);
                _objectsPool.Add(spawnable.Name, pool);
            }

            // add the spawnable to the pool
            pool.Release(spawnable);
        }

        private void FetchSpawnableFromGameObjects()
        {
            _spawnables = new ISpawnable[_spawnablesGO.Length];

            for (int i = 0; i < _spawnablesGO.Length; i++)
            {
                if (!_spawnablesGO[i].TryGetComponent<ISpawnable>(out var spawnable))
                {
                    Debug.LogError("No ISpawnable component found on " + _spawnablesGO[i].name + " game object");
                    continue;
                }

                _spawnables[i] = spawnable;
            }
        }

        private void FillupPool()
        {
            foreach (var spawnable in _spawnables)
            {
                var pool = CreatePoolForSpawnable(spawnable);

                _objectsPool.Add(spawnable.Name, pool);
            }
        }

        private ObjectPool<ISpawnable> CreatePoolForSpawnable(ISpawnable spawnable)
        {
            var pool = new ObjectPool<ISpawnable>(() =>

            // Create a new object
            {
                ISpawnable newSpawnable = Instantiate(spawnable.GameObject).GetComponent<ISpawnable>();
                return newSpawnable;
            },

                // Get the object from the pool
                (spawnable) =>
                {
                    spawnable.GameObject.SetActive(true);
                },

                // Return the object to the pool
                (spawnable) =>
                {
                    spawnable.GameObject.SetActive(false);
                },

                // Destroy the object
                (spawnable) =>
                {
                    Destroy(spawnable.GameObject);
                },

                // Collection Check
                false,

                // Initial Pool Size
                spawnable.PoolSize > 0 ? spawnable.PoolSize : 1,

                // Max Pool Size
                spawnable.PoolSize > 0 ? spawnable.PoolSize + 10 : 10
            );

            return pool;
        }
    }
}
