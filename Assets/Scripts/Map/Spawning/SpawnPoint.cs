using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A point where the specified GameObject will be spawned.
/// </summary>
public class SpawnPoint : NetworkBehaviour
{
    [SerializeField] private SpawnObjectType _spawnObjectType;
    [SerializeField] private GameObject _objectToSpawn; //The object to spawn
    [SerializeField] private bool _destroyAfterSpawn = true; //Wether to destroy this spawner object after spawning
    private Transform _spawnParent; //The parent of which the GameObject will be spawned under

    public override void OnNetworkSpawn()
    {
        SetSpawnParent();

        GameManager.Instance.OnGameStart += Spawn;
    }

    /// <summary>
    /// Spawn the GameObject
    /// </summary>
    public void Spawn(object sender, EventArgs e)
    {
        if (!IsServer) return;

        GameObject gameobject = Instantiate(_objectToSpawn, transform.position, _objectToSpawn.transform.rotation, _spawnParent);
        gameobject.GetComponent<NetworkObject>().Spawn();

        if (_spawnObjectType == SpawnObjectType.Player)
        {
            SetPlayerSpawned();
        }

        if (_destroyAfterSpawn)
            Destroy(gameObject);
    }

    private static void SetPlayerSpawned()
    {
        Debug.Log("Player spawned!");
        GameManager.Instance.InvokeOnPlayerSpawn();

    }

    /// <summary>
    /// Set the parent of the yet to be spawned GameObject.
    /// </summary>
    /// <param name="spawnParent">The parent Transform.</param>
    internal void SetSpawnParent(Transform spawnParent)
    {
        _spawnParent = spawnParent;
    }

    /// <summary>
    /// Set the parent of the yet to be spawned GameObject based on it's Spawn Object Type.
    /// </summary>
    internal void SetSpawnParent()
    {
        switch (_spawnObjectType)
        {
            case SpawnObjectType.Player: _spawnParent = null; break;
            case SpawnObjectType.Enemy: _spawnParent = GameManager.Instance.ParentTransformEnemies; break;
            case SpawnObjectType.PickUp: _spawnParent = GameManager.Instance.ParentTransformPickUps; break;
            case SpawnObjectType.StaticObject: _spawnParent = GameManager.Instance.ParentTransformStaticObjects; break;
        }
    }

    public override void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= Spawn;

        base.OnDestroy();
    }

    private enum SpawnObjectType { Player, Enemy, PickUp, StaticObject}
}