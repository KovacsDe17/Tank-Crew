using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a map with Enemy types and their corresponding prefabs.
/// </summary>
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

    public enum EnemyType { Dummy, Tank, Tower, Landmine }

    [System.Serializable]
    internal class EnemyPrefab
    {
        public EnemyType enemyType;
        public GameObject enemyPrefab;
    }
}