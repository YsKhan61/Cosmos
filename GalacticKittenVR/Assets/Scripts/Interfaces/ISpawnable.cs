using UnityEngine;


namespace GalacticKittenVR
{
    public interface ISpawnable
    {
        public string Name { get; }

        public GameObject GameObject { get; }

        public int PoolSize { get; }
    }

}