using System;
using UnityEditor;
using UnityEngine;

namespace Cosmos.PlatformConfiguration
{
    public enum PlatformType
    {
        FlatScreen,
        VR
    }
    /// <summary>
    /// The scriptable object will help for configuring platform specific in scene game objects as well as dynamically spawned game objects.
    /// </summary>
    [CreateAssetMenu(fileName = "PlatformConfig", menuName = "ScriptableObjects/PlatformConfigSO")]
    public class PlatformConfigSO : ScriptableObject
    {
        [SerializeField] private PlatformType _platform;
        public PlatformType Platform => _platform;

        // public event Action<PlatformType> OnPlatformChanged;

        /*public void ChangePlatform()
        {
            OnPlatformChanged?.Invoke(_platform);
        }*/
    }

    /*// Create an editor button to call ChangePlatform() in the inspector
    [CustomEditor(typeof(PlatformConfigSO))]
    public class PlatformConfigSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlatformConfigSO platformConfigSO = (PlatformConfigSO)target;
            if (GUILayout.Button("Change Platform"))
            {
                platformConfigSO.ChangePlatform();
            }
        }
    }*/

}
