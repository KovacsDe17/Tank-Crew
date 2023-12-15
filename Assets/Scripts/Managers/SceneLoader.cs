using Unity.Netcode;
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
        if(NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Reloads the currently open scene
    /// </summary>
    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits from the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Music.MainTheme);

        if(gameObject.GetComponent<MenuButtonSounds>() == null)
            gameObject.AddComponent<MenuButtonSounds>();
    }
}
