using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script ensures, that leaving the multiplayer section removes the Network Manager Singleton.
/// Without this, multiple instances are created upon opening the multiplayer section.
/// </summary>
public class DestroyNetworkManager : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Destroy(NetworkManager.Singleton.gameObject);
        });
    }
}
