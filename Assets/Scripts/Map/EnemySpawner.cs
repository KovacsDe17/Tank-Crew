using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    private EnemyPrefabSetup _setup;
    private bool _canDestroyAfterSpawn = true;

    private void Start()
    {
        _setup = GameObject.FindGameObjectWithTag("EnemyHolder").GetComponent<EnemyPrefabSetup>();
    }

    public void Spawn(EnemyPrefabSetup.EnemyType type)
    {
        if (!IsServer) return;

        GameObject prefab = _setup.GetPrefab(type);
        GameObject enemy = Instantiate(
            prefab,
            transform.position,
            prefab.transform.rotation, 
            _setup.GetCommonParent()
        );

        enemy.GetComponent<NetworkObject>().Spawn(true);

        if (_canDestroyAfterSpawn)
            Destroy(gameObject);
    }
}