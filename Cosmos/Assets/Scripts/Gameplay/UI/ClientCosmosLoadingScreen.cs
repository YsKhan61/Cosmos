using Cosmos.Gameplay.GameplayObjects;
using Unity.Multiplayer.Samples.Utilities;
using UnityEngine;


namespace Cosmos.Gameplay.UI
{
    public class ClientCosmosLoadingScreen : ClientLoadingScreen
    {
        [SerializeField]
        private PersistentPlayersRuntimeCollectionSO _persistentPlayerRuntimeCollection;

        protected override void AddOtherPlayerProgressBar(ulong clientId, NetworkedLoadingProgressTracker progressTracker)
        {
            base.AddOtherPlayerProgressBar(clientId, progressTracker);
            m_LoadingProgressBars[clientId].NameText.text = GetPlayerName(clientId);
        }

        private string GetPlayerName(ulong clientId)
        {
            foreach (var player in _persistentPlayerRuntimeCollection.Items)
            {
                if (player.OwnerClientId == clientId)
                {
                    return player.NetworkNameState.Name.Value;
                }
            }

            return "";
        }
    }

}
