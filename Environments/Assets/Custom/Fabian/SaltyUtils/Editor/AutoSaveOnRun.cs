using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class AutoSaveOnRun
{
    private static readonly string path = "Assets/Backup/Backup.unity";


    static AutoSaveOnRun()
    {
        EditorApplication.playModeStateChanged += SaveScene;
    }

    private static void SaveScene(PlayModeStateChange state)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
            {
                if (EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), path, true))
                    Debug.Log("Saved backup scene");
                else
                    Debug.LogError("Not able to save backup");
            }
        }
    }
}
