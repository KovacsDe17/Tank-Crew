using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefabSetup : MonoBehaviour
{
    [SerializeField] private List<EnemyPrefab> _enemyPrefabs;

    public GameObject GetPrefab(EnemyType type)
    {
        foreach(EnemyPrefab enemyPrefab in _enemyPrefabs)
        {
            if (enemyPrefab.enemyType == type)
                return enemyPrefab.enemyPrefab;
        }

        throw new Exception("No Enemy of the given type was found!");
    }

    public enum EnemyType { Dummy, Tank, Turret, Landmine }

    [System.Serializable]
    internal class EnemyPrefab
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
    }
}