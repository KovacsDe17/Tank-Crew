using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileSetup", menuName = "Tile Setup")]
public class TileSetup : ScriptableObject
{
    [Tooltip("The order of these count, as the Map Generator builds the map using them layer to layer!")]
    [SerializeField] private List<TileInfo> _tileInfos;

    [Tooltip("These floats divide each TileInfo, indicating where the change in layers happen. There should only be one less divider than TileInfo!")]
    [Range(0f,1f)]
    [SerializeField] private List<float> _manualDivisionPoints;

    public List<float> GetDivisionPoints()
    {
        List<float> divisionPoints = new List<float>();

        divisionPoints.AddRange(_manualDivisionPoints);
        divisionPoints.Add(0f);
        divisionPoints.Add(1f);
        divisionPoints.Sort();

        Debug.Log("Division Points: " + divisionPoints.ToString());

        return divisionPoints;
    }

    public TileInfo.DivisionPoints GetDivisionPointsOfTile(TileInfo tileInfo)
    {
        int indexOfTileInfo = _tileInfos.IndexOf(tileInfo);

        return new TileInfo.DivisionPoints
        {
            from = GetDivisionPoints()[indexOfTileInfo],
            to = GetDivisionPoints()[indexOfTileInfo + 1]
        };
    }

    public TileInfo GetTileByColorComponent(float colorComponent)
    {
        return _tileInfos
            .Where(x => x.divisionPoints.from <= colorComponent && x.divisionPoints.to >= colorComponent)
            .FirstOrDefault();
    }

    public void UpdateDivisionPoints()
    {
        foreach (TileInfo tileInfo in _tileInfos)
        {
            tileInfo.divisionPoints = GetDivisionPointsOfTile(tileInfo);
        }
    }

    public TileInfo GetWalkableTile()
    {
        return _tileInfos.Where(x => x.walkable == true).FirstOrDefault();
    }
}
