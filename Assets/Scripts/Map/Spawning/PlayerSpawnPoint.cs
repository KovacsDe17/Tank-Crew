using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("PlayerSpawnPoint Started");
        TileMapGeneration.Instance.OnMapGenerated += SpawnPlayer;
    }

    /// <summary>
    /// Move the PlayerTank to the position of this Transform then destroy this GameObject.
    /// </summary>
    private void SpawnPlayer(object sender, EventArgs e)
    {
        Debug.Log("PlayerSpawnPoint.SpawnPlayer() is being called with TileMapGeneration.OnMapGenerated event");
        PlayerTank.Instance.transform.position = transform.position;

        PlayerTank.Instance.transform.AddComponent<SoundOnMove>();

        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
