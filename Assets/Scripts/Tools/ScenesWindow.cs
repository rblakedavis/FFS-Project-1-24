using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ScenesWindow : EditorWindow
{
    // Add menu item named "Window/Scenes Window"
    [MenuItem("Window/Scenes Window")]
    static void Init()
    {
        // Get existing open window or if none, create a new one:
        ScenesWindow window = (ScenesWindow)EditorWindow.GetWindow(typeof(ScenesWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Scenes Window", EditorStyles.boldLabel);

        // Get all scenes in "Assets/Scenes" folder
        string scenesFolderPath = "Assets/Scenes";
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { scenesFolderPath });
        string[] scenePaths = new string[sceneGuids.Length];

        for (int i = 0; i < sceneGuids.Length; i++)
        {
            scenePaths[i] = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
        }

        // Display scene names without path
        foreach (string scenePath in scenePaths)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            // Check if the scene name is clicked
            if (GUILayout.Button(sceneName))
            {
                // Check for unsaved changes
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // Open the scene
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
        }
    }
}
