using UnityEngine;


namespace Cosmos
{
    public interface ISpawnable
    {
        public string Name { get; }

        public GameObject GameObject { get; }

        public int PoolSize { get; }
    }

}