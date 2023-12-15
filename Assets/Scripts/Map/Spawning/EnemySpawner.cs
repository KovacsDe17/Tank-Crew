using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// With this, spawnpoints of enemies be can placed.
/// </summary>
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _enemySpawnPointPrefab; //Prefab of an enemy spawn point
    [SerializeField] private float distanceFromPlayer = 20f; //Distance from the player (when attached to it)

    private Transform _playerTransform; //Transform component of the PlayerTank
    private EnemyTankPool _enemyTankPool;

    private void Start()
    {
        _playerTransform = PlayerTank.Instance.transform;
        _enemyTankPool = EnemyTankPool.Instance;
    }

    /// <summary>
    /// Spawn a type of enemy on the player, with the specified distance ahead of it.
    /// </summary>
    /// <param name="type">Type of the enemy to spawn.</param>
    public void SpawnOnPlayer(EnemyPrefabSetup.EnemyType type)
    {
        if(!IsServer) return;

        EnemySpawnPoint spawnPoint = Instantiate(
            _enemySpawnPointPrefab, 
            Vector2.zero,
            Quaternion.identity,
            _playerTransform)
        .GetComponent<EnemySpawnPoint>();

        spawnPoint.GetComponent<NetworkObject>().Spawn(true);
        
        spawnPoint.transform.localPosition = Vector2.up * distanceFromPlayer;

        spawnPoint.Spawn(type);
    }

    /// <summary>
    /// Spawn a type of enemy on the player, with the specified distance ahead of it.
    /// </summary>
    /// <param name="_enemyTankPool">Pool to spawn from.</param>
    public void SpawnOnPlayerFromPool()
    {
        if (!IsServer) return;

        EnemySpawnPoint spawnPoint = Instantiate(
            _enemySpawnPointPrefab,
            Vector2.zero,
            Quaternion.identity,
            _playerTransform)
        .GetComponent<EnemySpawnPoint>();

        spawnPoint.GetComponent<NetworkObject>().Spawn(true);

        //spawnPoint.transform.position = _playerTransform.TransformPoint((Vector3)(Vector2.up * distanceFromPlayer));
        spawnPoint.StickToPlayer(_playerTransform, distanceFromPlayer);

        spawnPoint.SpawnTankFromPool(_enemyTankPool);
    }

    /// <summary>
    /// Spawn multiple Tank enemies with a specified delay.
    /// </summary>
    /// <param name="numberOfTanks">Nu,ber of tanks to spawn.</param>
    public void SpawnEnemyTanks(int numberOfTanks)
    {
        if(!IsServer) return;

        StartCoroutine(SpawnWithDelay(numberOfTanks, 3, EnemyPrefabSetup.EnemyType.Tank));
    }

    /// <summary>
    /// Spawns a number of enemies of the given type with a delay between each spawning.
    /// </summary>
    /// <param name="numberOfEnemies">Number of enemies to spawn.</param>
    /// <param name="delay">Time between each spawning.</param>
    /// <param name="type">Type of the enemy to spawn.</param>
    private IEnumerator SpawnWithDelay(int numberOfEnemies, int delay, EnemyPrefabSetup.EnemyType type)
    {
        for (int i=0; i < numberOfEnemies; i++)
        {
            SpawnOnPlayer(type);
            yield return new WaitForSeconds(delay);
        }
    }
}
