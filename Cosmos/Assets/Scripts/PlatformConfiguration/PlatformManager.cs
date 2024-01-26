using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.PlatformConfiguration
{
    /// <summary>
    /// Activate Deactivate game objects based on each platform, at runtime, this is only for testing with ParrelSync.
    /// During build, we will configure in editor, then build for each platform.
    /// </summary>
    public class PlatformConfiguration : MonoBehaviour
    {
        [SerializeField] private PlatformConfigSO m_platformData;

        [SerializeField] private GameObject[] m_flatScreenObjects;
        [SerializeField] private GameObject[] m_vrObjects;

        private void Awake()
        {
            switch(m_platformData.Platform)
            {
                case PlatformType.FlatScreen:
                    foreach (GameObject obj in m_flatScreenObjects)
                    {
                        obj.SetActive(true);
                    }
                    foreach (GameObject obj in m_vrObjects)
                    {
                        obj.SetActive(false);
                    }
                    break;
                case PlatformType.VR:
                    foreach (GameObject obj in m_flatScreenObjects)
                    {
                        obj.SetActive(false);
                    }
                    foreach (GameObject obj in m_vrObjects)
                    {
                        obj.SetActive(true);
                    }
                    break;
            }
        }
    }
}
