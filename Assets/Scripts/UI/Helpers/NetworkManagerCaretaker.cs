using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

/// <summary>
/// This script ensures, that leaving the multiplayer section removes the Network Manager Singleton.
/// Without this, multiple instances are created upon opening the multiplayer section.
/// </summary>
public class NetworkManagerCaretaker : MonoBehaviour
{
    [SerializeField] private int _multiplayerSceneId;

    private void OnEnable()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Manage instances of NetworkManager when loading a scene.
    /// </summary>
    /// <param name="level">The ID of the loaded scene.</param>
    private void OnLevelWasLoaded(int level)
    {
        if(level != _multiplayerSceneId)
            DestroyInstance();
        else
        {
            EnsureSingleton();
        }
    }

    /// <summary>
    /// Ensure that only one instance exists of the NetworkManager
    /// </summary>
    private void EnsureSingleton()
    {
        Debug.Log("Ensuring singleton");

        if (NetworkManager.Singleton == null)
            GetComponent<NetworkManager>().SetSingleton();

        if (GetComponent<NetworkManager>() != NetworkManager.Singleton)
            Destroy(gameObject);

        return;
    }

    /// <summary>
    /// Shut down the Network Manager and destroy it's singleton instance
    /// </summary>
    private void DestroyInstance()
    {
        AuthenticationService.Instance.SignOut();
        
        Destroy(NetworkManager.Singleton.gameObject);
    }

    private List<NetworkManager> GetNetworkManagerList()
    {
        return FindObjectsByType<NetworkManager>(FindObjectsSortMode.InstanceID).ToList();
    }
}
