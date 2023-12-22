using Cosmos.Infrastructure;
using UnityEngine;


namespace Cosmos.Gameplay.GameplayObjects
{
    /// <summary>
    /// A runtime list of <see cref="PersistentPlayer"/> objects that is populated both on clients and server.
    /// </summary>
    [CreateAssetMenu(fileName = "PersistentPlayersRuntimeData", menuName = "ScriptableObjects/RuntimeDatas/PersistentPlayer")]
    public class PersistentPlayersRuntimeCollectionSO : RuntimeCollectionSO<PersistentPlayer>
    {
        public bool TryGetPlayer(ulong clientId, out PersistentPlayer persistentPlayer)
        {
            for (int i = 0, count = Items.Count; i < count; ++i)
            {
                if (Items[i].OwnerClientId == clientId)
                {
                    persistentPlayer = Items[i];
                    return true;
                }
            }

            persistentPlayer = null;
            return false;
        }
    }

}
