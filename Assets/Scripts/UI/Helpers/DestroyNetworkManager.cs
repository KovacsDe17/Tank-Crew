using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

/// <summary>
/// This script ensures, that leaving the multiplayer section removes the Network Manager Singleton.
/// Without this, multiple instances are created upon opening the multiplayer section.
/// </summary>
public class DestroyNetworkManager : MonoBehaviour
{
    [SerializeField] private int _multiplayerSceneId;

    private void OnLevelWasLoaded(int level)
    {
        if(level != _multiplayerSceneId)
            DestroyNetworkManagerInstance();
    }

    /// <summary>
    /// Shut down the Network Manager and destroy it's singleton instance
    /// </summary>
    private void DestroyNetworkManagerInstance()
    {
        AuthenticationService.Instance.SignOut();

        NetworkManager.Singleton.Shutdown();
        
        Destroy(NetworkManager.Singleton.gameObject);
    }
}
