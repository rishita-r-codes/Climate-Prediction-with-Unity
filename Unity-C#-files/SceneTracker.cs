using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneTracker : MonoBehaviour
{
    // list to stores the history of scenes accessed
    static List<string> sceneHistory = new List<string>();

    public static void TrackScene(string sceneName)
    {
        sceneHistory.Add(sceneName);
    }
    // called to load the previous scene
    public static void LoadPreviousScene()
    {
        if (sceneHistory.Count >= 2) // needs at least 2 scenes in the history to go back
        {
            // Get the scene at the second last position in the history list
            string sceneToLoad = sceneHistory[sceneHistory.Count - 2];
            // Remove the current scene from the history
            sceneHistory.RemoveAt(sceneHistory.Count - 1);
            // Load the previous scene
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }
}
