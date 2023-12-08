using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Spawn point of an enemy.
/// </summary>
public class EnemySpawnPoint : NetworkBehaviour
{
    private EnemyPrefabSetup _setup; //Prefab setup for enemies

    private bool _canDestroyAfterSpawn = true; //Wether to destroy the spawner after spawning an enemy
    private bool _canPlace = true; //Can the enemy be placed at the moment (on certain tiles, they can not)
    private bool _colliding = false; //Wether this object is colliding at the moment

    private EnemyPrefabSetup.EnemyType _enemyType; //Type of the enemy to spawn
    private Transform _playerTransform; //Transform of the Player Tank
    private bool _stickToPlayer = false; //Wether to move this spawn point with the Player Tank
    private float _distanceFromPlayer; //Distance from the Player Tank, when sticked to it

    private void Start()
    {
        _setup = GameObject.FindGameObjectWithTag("EnemyHolder").GetComponent<EnemyPrefabSetup>();

        GameManager.Instance.OnGameStart += SpawnOnGameStart;
    }

    private void Update()
    {
        if (_stickToPlayer)
            UpdateRelativePosition();
    }

    private void UpdateRelativePosition()
    {
        transform.position = _playerTransform.TransformPoint((Vector3)(Vector2.up * _distanceFromPlayer));

        _canPlace = !_colliding;
    }

    public void StickToPlayer(Transform playerTransform, float distanceFromPlayer)
    {
        _playerTransform = playerTransform;
        _distanceFromPlayer = distanceFromPlayer;
        _stickToPlayer = true;
    }

    /// <summary>
    /// Spawn the enemy when the player spawns
    /// </summary>
    private void SpawnOnGameStart(object sender, EventArgs e)
    {
        Spawn(_enemyType);
    }

    public void UpdateEnemyType(EnemyPrefabSetup.EnemyType enemyType)
    {
        _enemyType = enemyType;
    }

    /// <summary>
    /// Spawn an enemy of type.
    /// </summary>
    /// <param name="type">The type of the enemy.</param>
    public void Spawn(EnemyPrefabSetup.EnemyType type)
    {
        if (!IsServer) return; //Only the server can spawn!

        //Start the spawning
        StartCoroutine(SpawnCR(type));
    }

    /// <summary>
    /// Coroutine for the spawning of a type of enemy.
    /// </summary>
    /// <param name="type">Type of the enemy.</param>
    private IEnumerator SpawnCR(EnemyPrefabSetup.EnemyType type)
    {
        //Should wait for the flag to be able to continue
        yield return new WaitUntil(() => _canPlace);

        //Get the prefab from the setup and instantiate it
        GameObject prefab = _setup.GetPrefab(type);
        GameObject enemy = Instantiate(
            prefab,
            transform.position,
            prefab.transform.rotation,
            _setup.transform
        );

        enemy.GetComponent<NetworkObject>().Spawn(true);

        enemy.transform.SetParent(_setup.transform);
        
        //If destroy is set, do it
        if (_canDestroyAfterSpawn)
        {
            gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }

    public void SpawnTankFromPool(EnemyTankPool pool)
    {
        StartCoroutine(SpawnTankOnPlayer(pool));
    }

    private IEnumerator SpawnTankOnPlayer(EnemyTankPool pool)
    {
        _canPlace = false;
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => _canPlace);

        Enemy tank = pool.SpawnNextTank(transform.position).GetComponentInChildren<Enemy>();
        tank.Initialize();

        //If destroy is set, do it
        if (_canDestroyAfterSpawn)
        {
            gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_stickToPlayer) return;

        Debug.Log("SpawnPoint -  Trigger entered");
        _colliding = true;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!_stickToPlayer) return;

        Debug.Log("SpawnPoint -  Trigger stayed");
        _colliding = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(!_stickToPlayer) return;

        Debug.Log("SpawnPoint -  Trigger exited");
        _colliding = false;
    }
}