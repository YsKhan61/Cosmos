using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.PlatformConfiguration
{
    /// <summary>
    /// Initializes the game objects for the current platform.
    /// This script is used to hide or show game objects depending on the platform. - FlatScreen or VR.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class PlatformManager : MonoBehaviour
    {
        protected abstract void ChangePlatform(PlatformType platformType);
    }
}
