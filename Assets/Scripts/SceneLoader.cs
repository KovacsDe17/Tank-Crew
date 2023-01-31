using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for loading new scenes, reloading the current one, and for quitting the game
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads a scene by its sceneIndex
    /// </summary>
    /// <param name="sceneIndex">The unique index of the scene to be loaded</param>
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Reloads the currently open scene
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits from the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}