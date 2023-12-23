using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Cosmos.Editor
{
    /// <summary>
    /// Class that permits auto-loading a bootstrap scene when the editor switches to play mode.
    /// This class is initialized when Unity is opened and wehn scripts are recompiled.
    /// This is to be able to subscribe to EditorApplication.playModeStateChanged event, which is when the editor switches to play mode or we wish to open a new scene.
    /// </summary>
    /// <remarks>
    /// A clitical edge case scenario regarding NetworkManager is accounted for here.
    /// A NetworkObject's GlobalObjectIdHash value is currently generated in OnValidate() which is invoked during a 
    /// build and when the asset is loaded/viewed in the editor.
    /// If we were to manually open Bootstrap Scen via EditorSceneManager.OpenScene(...) as the editor is exiting play mode,
    /// Bootstrap scene would be entering play mode within the editor prior to having loaded any assets, meaning
    /// NetworkManager itself has no entry within the AssetDatabase cache. 
    /// As a result of this, any referenced NetworkPrefabs wouldn't have any entry either.
    /// To account for this necessary AssetDatabase step, whenever we're redirecting from a new scene, or a scene
    /// existing in our EditorBuildSettings, we forcefully stop the editor, open Bootstrap scene, and re-enter play mode.
    /// This provides the editor the chance to create AssetDatabase cache entries for the Network Prefabs assigned to the NetworkManager.
    /// If we are entering play mode directly from Bootstrap scene, no additional steps need to be taken and the scene is loaded normally.
    /// </remarks>
    [InitializeOnLoad]
    public class SceneBootstrapper
    {
        private const string PREVIOUS_SCENE_KEY = "Previous Scene";
        private const string SHOULD_LOAD_BOOTSTRAP_SCENE_KEY = "Load Bootstrap Scene";

        private const string LOAD_BOOTSTRAP_SCENE_ON_PLAY = "Cosmos/Load Bootstrap Scene on play";
        private const string DO_NOT_LOAD_BOOTSTRAP_SCENE_ON_PLAY = "Cosmos/Do not load Bootstrap Scene on Play";

        private const string TESTRUNNER_SCENE_NAME = "InitTestScene";

        static bool _restartingToSwitchScene;

        static string _bootstrapScenePath => EditorBuildSettings.scenes[0].path;

        static string _previousScene
        {
            get => EditorPrefs.GetString(PREVIOUS_SCENE_KEY);
            set => EditorPrefs.SetString(PREVIOUS_SCENE_KEY, value);
        }

        static bool _shouldLoadBootstrapScene
        {
            get
            {
                if (!EditorPrefs.HasKey(SHOULD_LOAD_BOOTSTRAP_SCENE_KEY))
                {
                    EditorPrefs.SetBool(SHOULD_LOAD_BOOTSTRAP_SCENE_KEY, true);
                }

                return EditorPrefs.GetBool(SHOULD_LOAD_BOOTSTRAP_SCENE_KEY, true);
            }

            set => EditorPrefs.SetBool(SHOULD_LOAD_BOOTSTRAP_SCENE_KEY, value);
        }

        static SceneBootstrapper()
        {
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        [MenuItem(LOAD_BOOTSTRAP_SCENE_ON_PLAY, true)]
        static bool ShowLoadBootstrapSceneOnPlay()
        {
            return !_shouldLoadBootstrapScene;
        }

        [MenuItem(LOAD_BOOTSTRAP_SCENE_ON_PLAY)]
        static void EnableLoadBootstrapSceneOnPlay()
        {
            _shouldLoadBootstrapScene = true;
        }

        [MenuItem(DO_NOT_LOAD_BOOTSTRAP_SCENE_ON_PLAY, true)]
        static bool ShowDoNotLoadBootstrapSceneOnPlay()
        {
            return _shouldLoadBootstrapScene;
        }

        [MenuItem(DO_NOT_LOAD_BOOTSTRAP_SCENE_ON_PLAY)]
        static void DisableLoadBootstrapSceneOnPlay()
        {
            _shouldLoadBootstrapScene = false;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (IsTestRunnerActive())
            {
                return;
            }

            if (!_shouldLoadBootstrapScene)
            {
                return;
            }

            if (_restartingToSwitchScene)
            {
                if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    // For some reason there's multiple start and stops events happening while restarting the editor's play mode.
                    // We're making sure to set stopping and staring only when we're done and we've entered play mode.
                    // This way we won't corrupt "activeScene" with the multiple start and stop and will be able to return to the scene we were editing at first.
                    _restartingToSwitchScene = false;
                }
                return;
            }

            if (change == PlayModeStateChange.ExitingEditMode)
            {
                // cache previous scene so we return to this scene after play session, if possible
                _previousScene = EditorSceneManager.GetActiveScene().path;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // user either hit "Save" or "Don't Save" in the dialog - we can continue to exit edit mode (current scene) and open bootstrap scene
                    if (!string.IsNullOrEmpty(_bootstrapScenePath) &&
                        System.Array.Exists(EditorBuildSettings.scenes, scenes => scenes.path == _bootstrapScenePath))
                    {
                        Scene activeScene = EditorSceneManager.GetActiveScene();

                        _restartingToSwitchScene = activeScene.path == string.Empty || !_bootstrapScenePath.Contains(activeScene.path);

                        // we only manually inject Bootstrap scene if we are in a blank empty scene,
                        // or if the active scene is not already Bootstrap scene.
                        if (_restartingToSwitchScene)
                        {
                            EditorApplication.isPlaying = false;

                            // scene is included in build settings; open it
                            EditorSceneManager.OpenScene(_bootstrapScenePath);

                            EditorApplication.isPlaying = true;
                        }
                    }
                }
                else
                {
                    // user either hit "Cancel" or exited window; don't open bootstrap scnee & return to editor
                    EditorApplication.isPlaying = false;
                }
            }
            else if (change == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(_previousScene))
                {
                    EditorSceneManager.OpenScene(_previousScene);
                }
            }
        }

        static bool IsTestRunnerActive()
        {
            return EditorSceneManager.GetActiveScene().name.StartsWith(TESTRUNNER_SCENE_NAME);
        }
    }
}
