using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene management

public class ResetSceneButton : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void ResetScene()
    {
        // Get the active scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}