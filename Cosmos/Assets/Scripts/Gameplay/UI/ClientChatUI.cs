using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


namespace Cosmos.Gameplay.UI
{
    public class ClientChatUI : MonoBehaviour
    {
        

        

        [SerializeField]
        ServerChatSystem m_NetworkChatting;

        private void Start()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
        }

        
    }
}