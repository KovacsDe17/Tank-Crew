using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileMapGeneration : MonoBehaviour
{
    private enum DisplayTestImage { Colored, Perlin, Both}
    private int currentImageDisplay = 0;
    private enum MapQuarter { TopLeft, TopRight, BottomLeft, BottomRight };

    private struct EndPoints
    {
        internal Vector2Int objectivePoint;
        internal Vector2Int playerSpawnPoint;
    }

    [SerializeField] private RawImage testImageColored;
    [SerializeField] private RawImage testImagePerlin;
    [SerializeField] private RawImage testImageMarked;
    [SerializeField] private TextMeshProUGUI testButtonText;

    [SerializeField] private Tilemap _baseTilemap;
    [SerializeField] private Tilemap _markTilemap;

    [Space]
    [Header("Map setup")]
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private float _mapScale;
    [SerializeField] private Vector2Int _offset;
    [Space]

    [Space]
    [Header("Tile setup")]
    private Dictionary<Color, Tile> _colorToTile;
    [SerializeField] private TileSetup _baseTileSetup;
    [Space]

    private List<float> _normalizedAbsoluteThresholds = new List<float>();
    [SerializeField] private List<Slider> valuesFromSlider = new List<Slider>();

    public void GenerateTileMap()
    {
        _baseTilemap.ClearAllTiles();

        PerlinNoise perlinNoise = new PerlinNoise(_mapSize, _mapScale, _offset);
        Texture2D texture = perlinNoise.GenerateTexture();

        //UpdateThresholds();
        _baseTileSetup.UpdateDivisionPoints();

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                float color = texture.GetPixel(x, y).r;

                //tilemap.SetTile((Vector3Int)position, GetTileByColorUniform(color));
                _baseTilemap.SetTile((Vector3Int)position, _baseTileSetup.GetTileByColorComponent(color).tile);
            }
        }
    }

    private Texture2D GenerateColoredMap(Texture2D perlinMap)
    {
        Texture2D texture = new Texture2D(perlinMap.width, perlinMap.height);

        //UpdateThresholds();
        _baseTileSetup.UpdateDivisionPoints();

        for (int x = 0; x < perlinMap.width; x++)
        {
            for (int y = 0; y < perlinMap.height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                float color = perlinMap.GetPixel(x, y).r;

                texture.SetPixel(position.x, position.y, _baseTileSetup.GetTileByColorComponent(color).getAliasColor());
            }
        }

        texture.Apply();

        return texture;
    }

    public void GenerateOnTestImage()
    {
        PerlinNoise perlinNoise = new PerlinNoise(_mapSize, _mapScale, _offset);
        Texture2D baseTexture = GenerateColoredMap(perlinNoise.GenerateTexture()); //Create colored base map

        Texture2D perlinTexture = perlinNoise.GenerateTextureBetweenValues(_baseTileSetup.GetWalkableTile().divisionPoints); //Create partial perlin map
        
        Texture2D markedTexture = new Texture2D(_mapSize.x,_mapSize.y); //Create a blank map to be the marked one
        markedTexture = FillTexture(markedTexture, Color.clear);

        List<Vector2Int> midPoints = FindMidPoints(perlinTexture, _baseTileSetup.GetWalkableTile().divisionPoints);
        markedTexture = MarkPoints(midPoints, Color.yellow, markedTexture);

        EndPoints endPoints = FindEndPoints(midPoints);
        markedTexture = MarkPoints(endPoints, Color.red, markedTexture);

        testImagePerlin.texture = perlinTexture;
        testImageColored.texture = baseTexture;
        testImageMarked.texture = markedTexture;
    }

    private Texture2D FillTexture(Texture2D texture, Color color)
    {
        for(int x = 0; x < _mapSize.x; x++)
        {
            for(int y = 0; y < _mapSize.y; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    private EndPoints FindEndPoints(List<Vector2Int> midPoints)
    {
        Vector2Int center = new Vector2Int(_mapSize.x/2, _mapSize.y/2);

        Dictionary<Vector2Int, MapQuarter> pointsInQuarters = new Dictionary<Vector2Int, MapQuarter>();
        Dictionary<MapQuarter, int> counts = new Dictionary<MapQuarter, int>();

        EndPoints finalPoints = new EndPoints();

        //Divide points into quarters
        foreach (Vector2Int point in midPoints)
        {
            if(point.x <= center.x && point.y > center.y)
            {
                pointsInQuarters.Add(point, MapQuarter.TopLeft);

                if(counts.ContainsKey(MapQuarter.TopLeft))
                    counts[MapQuarter.TopLeft] += 1;
                else 
                    counts[MapQuarter.TopLeft] = 1;
            }

            if (point.x > center.x && point.y > center.y)
            {
                pointsInQuarters.Add(point, MapQuarter.TopRight);

                if (counts.ContainsKey(MapQuarter.TopRight))
                    counts[MapQuarter.TopRight] += 1;
                else
                    counts[MapQuarter.TopRight] = 1;
            }

            if (point.x <= center.x && point.y <= center.y)
            {
                pointsInQuarters.Add(point, MapQuarter.BottomLeft);

                if (counts.ContainsKey(MapQuarter.BottomLeft))
                    counts[MapQuarter.BottomLeft] += 1;
                else
                    counts[MapQuarter.BottomLeft] = 1;
            }

            if (point.x > center.x && point.y <= center.y)
            {
                pointsInQuarters.Add(point, MapQuarter.BottomRight);

                if (counts.ContainsKey(MapQuarter.BottomRight))
                    counts[MapQuarter.BottomRight] += 1;
                else
                    counts[MapQuarter.BottomRight] = 1;
            }
        }

        //Get the quarter with the most points and also get it's opposite quarter
        MapQuarter objectiveQuarter = counts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        MapQuarter playerSpawnPointQuarter = Opposite(objectiveQuarter);

        finalPoints.objectivePoint = GetFurthestPointInQuarter(pointsInQuarters, objectiveQuarter, center, center.x * 0.85f);
        finalPoints.playerSpawnPoint = GetFurthestPointInQuarter(pointsInQuarters, playerSpawnPointQuarter, center, center.x * 0.85f);

        Debug.Log("Final Points are: Obj = " +  finalPoints.objectivePoint + ", PSP = " + finalPoints.playerSpawnPoint);

        return finalPoints;
    }

    private Vector2Int GetFurthestPointInQuarter(Dictionary<Vector2Int, MapQuarter> pointsInQuarters, MapQuarter myQuarter, Vector2Int center, float maxDistance)
    {
        float highestDistance = 0f;
        Vector2Int positionWithMaxDistance = center;

        List<Vector2Int> pointsInMyQuarter = pointsInQuarters.Where(x => x.Value == myQuarter).Select(x => x.Key).ToList();

        foreach(Vector2Int point in pointsInMyQuarter)
        {
            float distance = Vector2.Distance(point, center);

            if(distance <= maxDistance && distance > highestDistance)
            {
                highestDistance = distance;
                positionWithMaxDistance = point;
            }
        }

        return positionWithMaxDistance;
    }

    private Texture2D MarkPoints(List<Vector2Int> points, Color color, Texture2D previousTexture)
    {
        foreach(Vector2Int position in  points)
        {
            previousTexture.SetPixel(position.x, position.y, color);
        }

        previousTexture.Apply();

        return previousTexture;
    }

    private Texture2D MarkPoints(EndPoints points, Color color, Texture2D previousTexture)
    {
        previousTexture.SetPixel(points.objectivePoint.x, points.objectivePoint.y, color);
        previousTexture.SetPixel(points.playerSpawnPoint.x, points.playerSpawnPoint.y, color);

        previousTexture.Apply();

        return previousTexture;
    }

    private List<Vector2Int> FindMidPoints(Texture2D perlinTexture, float floor, float ceil)
    {
        List<Vector2Int> midPoints = new List<Vector2Int>();

        float mid = (floor + ceil) / 2f; //The middle between floor and ceil
        decimal roundedMid = Math.Round((decimal)mid, 2);

        for (int x = 0; x < perlinTexture.width; x++)
        {
            for (int y = 0; y < perlinTexture.height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                float color = perlinTexture.GetPixel(x, y).r;
                decimal roundedColor = Math.Round((decimal)color, 2);

                if (roundedColor == roundedMid) //If the current pixel is the same value as the mid
                {
                    midPoints.Add(position);    //Add the position to the list
                }
            }
        }

        Debug.Log("MidCount with MidPoint=" + roundedMid + " is " + midPoints.Count);

        return midPoints;
    }

    private List<Vector2Int> FindMidPoints(Texture2D perlinTexture, TileInfo.DivisionPoints divisionPoints)
    {
        return FindMidPoints(perlinTexture, divisionPoints.from, divisionPoints.to);
    }

    public void SetSizeX(int mapSizeX)
    {
        _mapSize.x = mapSizeX;
        Debug.Log("MapSizeX set to " + mapSizeX);
    }

    public void SetSizeY(int mapSizeY)
    {
        _mapSize.y = mapSizeY;
        Debug.Log("MapSizeY set to " + mapSizeY);
    }

    public void SetScale(float scale)
    {
        _mapScale = scale;
        Debug.Log("MapScale set to " + scale);
    }

    public void SetOffsetX(int offsetX)
    {
        _offset.x = offsetX;
        Debug.Log("OffsetX set to " +  offsetX);
    }

    public void SetOffsetY(int offsetY)
    {
        _offset.y = offsetY;
        Debug.Log("OffsetY set to " + offsetY);
    }

    public TileSetup GetTileSetups()
    {
        return _baseTileSetup;
    }

    /// <summary>
    /// Translates the given string to a number. If the seed is too short or too long, the value gets to be corrected automatically.
    /// </summary>
    /// <param name="seed">A 0-10 characters long text.</param>
    public void SetOffsetFromSeed(string seed)
    {
        int.TryParse(seed, out int numSeed); //Setting to a number
        string newSeed = numSeed.ToString("0000000000"); //Setting back to string to get 10 decimals for splitting
        string part1 = newSeed.Substring(0, 5); //Splitting the first half
        string part2 = newSeed.Substring(5, 5); //Splitting the second half

        int.TryParse(part1, out int offsetX);
        int.TryParse(part2, out int offsetY);

        SetOffsetX(offsetX);
        SetOffsetY(offsetY);
    }

    public void SetOffSetFromSeed(TMPro.TMP_InputField inputField)
    {
        SetOffsetFromSeed(inputField.text);
    }

    public void SwitchColoredAndPerlinMaps()
    {
        currentImageDisplay++;

        if (currentImageDisplay > 2)
            currentImageDisplay = 0;

        bool displayColored;
        bool displayPerlin;

        switch ((DisplayTestImage) currentImageDisplay)
        {
            case DisplayTestImage.Colored:
                displayColored = true;
                displayPerlin = false;
                testButtonText.text = "Show Perlin Map";
                break;

            case DisplayTestImage.Perlin:
                displayColored = false;
                displayPerlin = true;
                testImagePerlin.color = new Color(1, 1, 1, 1);
                testButtonText.text = "Show Both Maps";
                break;

            case DisplayTestImage.Both:
                displayColored = true;
                displayPerlin = true;
                testImagePerlin.color = new Color(1, 1, 1, 0.3f);
                testButtonText.text = "Show Colored Map";
                break;

            default:
                displayColored = true;
                displayPerlin = false;
                break;
        }

        testImageColored.gameObject.SetActive(displayColored);
        testImagePerlin.gameObject.SetActive(displayPerlin);
    }

    private MapQuarter Opposite(MapQuarter quarter)
    {
        switch (quarter)
        {
            case MapQuarter.TopLeft: return MapQuarter.BottomRight;
            case MapQuarter.TopRight: return MapQuarter.BottomLeft;
            case MapQuarter.BottomLeft: return MapQuarter.TopRight;
            case MapQuarter.BottomRight: return MapQuarter.TopLeft;

            default: return MapQuarter.TopLeft;
        }
    }
}
