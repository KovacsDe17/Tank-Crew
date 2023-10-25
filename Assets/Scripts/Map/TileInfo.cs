using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "TileInfo", menuName = "Tile Info")]
public class TileInfo : ScriptableObject
{
    [SerializeField] internal Tile tile;
    [SerializeField] internal Color aliasColor;
    [SerializeField] internal bool walkable;
    internal DivisionPoints divisionPoints;

    internal float relativeThreshold; //TODO: delete this variable

    public float GetThreshold()
    {
        return relativeThreshold;
    }

    public void SetThreshold(float percentage)
    {
        relativeThreshold = percentage;
    }

    internal Color getAliasColor() {
        return aliasColor;
    }

    public struct DivisionPoints
    {
        internal float from;
        internal float to;
    }
}