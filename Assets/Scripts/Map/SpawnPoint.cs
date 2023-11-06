using System;
using UnityEngine;

/// <summary>
/// A point where the specified GameObject will be spawned.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject _objectToSpawn; //The object to spawn
    [SerializeField] private bool _destroyAfterSpawn = true; //Wether to destroy this object after spawning
    private Transform _spawnParent; //The parent to which the GameObject will be spawned under

    private void Start()
    {
        GameManager.Instance.OnGameStart += Spawn;
    }

    /// <summary>
    /// Spawn the GameObject
    /// </summary>
    public void Spawn(object sender, EventArgs e)
    {
        Instantiate(_objectToSpawn, transform.position, _objectToSpawn.transform.rotation, _spawnParent);

        if(_destroyAfterSpawn)
            Destroy(gameObject);
    }

    /// <summary>
    /// Set the parent of the yet to be spawned GameObject.
    /// </summary>
    /// <param name="spawnParent">The parent Transform.</param>
    internal void SetSpawnParent(Transform spawnParent)
    {
        _spawnParent = spawnParent;
    }
}
