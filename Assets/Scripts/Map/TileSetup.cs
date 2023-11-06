using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// In this object, the different tile informations and their connections can be set.
/// </summary>
[CreateAssetMenu(fileName = "TileSetup", menuName = "Tile Setup")]
public class TileSetup : ScriptableObject
{
    [Tooltip("The order of these count, as the Map Generator builds the map using them layer to layer!")]
    [SerializeField] private List<TileInfo> _tileInfos; //List of all TileInfo objects that the Map Generator uses

    [Tooltip("These floats divide each TileInfo, indicating where the change in layers happen. There should only be one less divider than TileInfo!")]
    [Range(0f,1f)]
    [SerializeField] private List<float> _manualDivisionPoints; //List of points between 0 and 1, which determine where the different tiles are separated at, based on the Perlin Noise

    private Dictionary<Color, TileInfo> _tileInfosByColor = new Dictionary<Color, TileInfo>(); //A dictionary to access TileInfo objects by their AliasColor

    /// <summary>
    /// Returns all the division points of this TileSetup. There are always N+1 of these, where N is the number of TileInfos.
    /// </summary>
    /// <returns>A list of float values, in ascending order.</returns>
    public List<float> GetDivisionPoints()
    {
        List<float> divisionPoints = new List<float>();

        divisionPoints.AddRange(_manualDivisionPoints);
        divisionPoints.Add(0f);
        divisionPoints.Add(1f);
        divisionPoints.Sort();

        //Debug.Log("Division Points: " + divisionPoints.ToString());

        return divisionPoints;
    }

    /// <summary>
    /// Return the floor and ceil division points of a given TileInfo.
    /// </summary>
    /// <param name="tileInfo">The TileInfo which's division points are needed.</param>
    /// <returns>Floor and ceil division points of the TileInfo in a pair struct.</returns>
    public TileInfo.DivisionPoints GetDivisionPointsOfTile(TileInfo tileInfo)
    {
        int indexOfTileInfo = _tileInfos.IndexOf(tileInfo);

        return new TileInfo.DivisionPoints
        {
            floor = GetDivisionPoints()[indexOfTileInfo],
            ceil = GetDivisionPoints()[indexOfTileInfo + 1]
        };
    }

    /// <summary>
    /// Return a TileInfo by a color component.
    /// </summary>
    /// <param name="colorComponent">Any component (R,G or B) from a grey color, preferably from a Perlin Noise.</param>
    /// <returns>A TileInfo where the given color component is in between it's division points.</returns>
    public TileInfo GetTileByColorComponent(float colorComponent)
    {
        return _tileInfos
            .Where(x => x.divisionPoints.floor <= colorComponent && x.divisionPoints.ceil >= colorComponent)
            .FirstOrDefault();
    }

    /// <summary>
    /// Update all division points based on the list of TileInfos.
    /// </summary>
    public void UpdateDivisionPoints()
    {
        foreach (TileInfo tileInfo in _tileInfos)
        {
            tileInfo.divisionPoints = GetDivisionPointsOfTile(tileInfo);
        }
    }

    /// <summary>
    /// Return the main, walkable TileInfo.
    /// </summary>
    /// <returns>The TileInfo marked as main.</returns>
    public TileInfo GetMainTile()
    {
        return _tileInfos.Where(x => x.tileType == TileInfo.TileType.Main).FirstOrDefault();
    }

    /// <summary>
    /// Return a TileInfo it's alias color.
    /// </summary>
    /// <param name="color">The color to search by.</param>
    /// <returns>The TileInfo which's alias color matches the given color.</returns>
    public TileInfo GetTileInfoByColor(Color color)
    {
        //Get the tile from the dictionary
        TileInfo tileInfo;
        _tileInfosByColor.TryGetValue(color, out tileInfo);

        //If it doesn't exist there
        if(tileInfo == null)
        {
            //Search for it in the TileInfos list
            tileInfo = _tileInfos.Where(x => x.aliasColor == color).FirstOrDefault();
            //Add it to the dictionary
            _tileInfosByColor[color] = tileInfo;
        }

        //Return the tile
        return tileInfo;
    }
}
