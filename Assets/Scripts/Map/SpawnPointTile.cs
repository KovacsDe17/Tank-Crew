using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPointTile
{
    [SerializeField] private Tile tile; //The tile which determines the position of the spawned object
    [SerializeField] private GameObject objectToSpawn; //The object to spawn
}
