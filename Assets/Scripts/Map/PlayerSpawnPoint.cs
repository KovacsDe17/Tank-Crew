using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnPlayerSpawn += SpawnPlayer;
    }

    /// <summary>
    /// Move the PlayerTank to the position of this Transform then destroy this GameObject.
    /// </summary>
    private void SpawnPlayer(object sender, EventArgs e)
    {
        PlayerTank.Instance.transform.position = transform.position;

        Destroy(gameObject);
    }
}
