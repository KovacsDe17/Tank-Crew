using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This object holds information of a tile, an alias color and a tile type. It is used for TileSetups for the Map Generator.
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "TileInfo", menuName = "Tile Info")]
public class TileInfo : ScriptableObject
{
    [SerializeField] internal Tile tile; //The tile which will be placed on the TileMap
    [SerializeField] internal Color aliasColor; //The associated color for the Map Generator
    [SerializeField] internal TileType tileType; //The type of this tile. Can be Main, Drag or Collide

    internal DivisionPoints divisionPoints; //The floor and ceil points, used when converting Perlin Noise to Colored Map

    /// <summary>
    /// Returns the alias color of the TileInfo.
    /// </summary>
    /// <returns>The color of the TileInfo.</returns>
    internal Color getAliasColor() {
        return aliasColor;
    }

    /// <summary>
    /// These points represent an interval for converting from Perlin Noise to Colored Map.
    /// If a value is between these, it means that this tile will be used.
    /// </summary>
    public struct DivisionPoints
    {
        internal float floor;
        internal float ceil;
    }

    /// <summary>
    /// Tile type which helps separating tilemaps to different layers.
    /// </summary>
    public enum TileType { Main, Drag, Collide }

}