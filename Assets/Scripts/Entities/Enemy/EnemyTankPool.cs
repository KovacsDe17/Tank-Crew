using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An object for pooling pre-placed Enemy Tanks, since they cannot be instantiated and spawned on the network properly.
/// </summary>
public class EnemyTankPool : MonoBehaviour
{
    public static EnemyTankPool Instance; //Singleton instance
    public static int NEXT_INDEX = 0; //Static index counter for the list of Enemy Tanks
    [SerializeField] private List<GameObject> _enemyTankList = new List<GameObject>(); //List of Enemy Tanks

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Get the next Enemy Tank from the pool.
    /// </summary>
    /// <returns>An Enemy Tank if there was any left in the pool, otherwise null.</returns>
    public GameObject GetNextTank()
    {
        if(NEXT_INDEX > _enemyTankList.Count) return null;

        return _enemyTankList[NEXT_INDEX++];
    }

    /// <summary>
    /// Get an Enemy Tank from the pool directly by it's index.
    /// </summary>
    /// <param name="index">The index of the Enemy Tank.</param>
    /// <returns>The Enemy Tank if the index was appropriate, otherwise null.</returns>
    public GameObject GetTankByIndex(int index)
    {
        if (index < 0 || index > _enemyTankList.Count) return null;

        return _enemyTankList[index];
    }

    /// <summary>
    /// Gets a new Enemy Tank from the pool and sets it's local position and parent,
    /// and updates the rotation to look at the Player Tank.
    /// </summary>
    /// <param name="localPosition">Position relative to the parent.</param>
    /// <param name="parent">Parent Transform of the object.</param>
    /// <returns></returns>
    public Transform SpawnNextTank(Vector2 localPosition, Transform parent = null)
    {
        Transform tank = GetNextTank().transform;

        tank.parent = parent;
        tank.localPosition = localPosition;
        tank.LookAt(PlayerTank.Instance.transform, Vector3.up);

        tank.gameObject.SetActive(true);

        return tank;
    }
}
