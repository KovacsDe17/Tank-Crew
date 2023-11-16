using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// This object is responsible for the generation of different Tilemaps and a minimap.
/// </summary>
public class TileMapGeneration : MonoBehaviour
{
    //Colors for the marked texture
    private Color _playerSpawnPointColor = Color.blue;
    private Color _objectivePointColor = Color.magenta;
    private Color _midPointColor = Color.yellow;

    [Header("Minimaps")]
    [SerializeField] private List<Minimap> _minimaps; //All minimaps to be seen throughout the game (menu, lobby, ingame, etc)
    [Space]
    [Header("Test images")]
    [SerializeField] private RawImage _testImagePerlin; //Test image for the partial Perlin Noise
    [SerializeField] private RawImage _testImageMarked; //Test image for the Marked Map
    [Space]
    [Header("Map")]
    [SerializeField] private Vector2Int _mapSize; //Size of the map
    [SerializeField] private float _mapScale; //Zoom of the Perlin Noise used for generating the map
    [SerializeField] private Vector2Int _offset; //Offset of the Perlin Noise used for generating the map
    [SerializeField] private Tilemap _baseTilemap; //Tilemap for the main walkable surface
    [SerializeField] private Tilemap _dragTilemap; //Tilemap for the surfaces that use drag
    [SerializeField] private Tilemap _lowerColliderTilemap; //Tilemap for the colliders in a lower layer
    [SerializeField] private Tilemap _upperColliderTilemap; //Tilemap for the colliders in an upper layer
    [Space]
    [Header("Tiles")]
    [SerializeField] private TileSetup _tileSetup; //TileSetup that is being applied
    [Space]
    [Header("Loading Icon")]
    [SerializeField] private GameObject _loadingIcon; //The loading icon get's enabled on map generation
    [Space]
    [Header("Spawning")]
    [SerializeField] private Transform _parentGrid; //The Grid that parents the tilemaps
    [SerializeField] private GameObject _playerSpawnPointPrefab; //Prefab for the Player's spawn point
    [SerializeField] private GameObject _objectiveSpawnPointPrefab; //Prefab for the objective's spawn point
    private GameObject _playerSpawnPoint; //Spawn point of the Player
    private GameObject _objectiveSpawnPoint; //Spawn point of the objective
    
    private EndPoints _endPoints; //The position of the Player's and the objective's spawn point

    /// <summary>
    /// Generate tilemaps based on the previously set size, scale and offset.
    /// </summary>
    public void GenerateTileMaps()
    {
        //_loadingIcon.SetActive(true);
        //Debug.Log("I am " + (_loadingIcon.activeSelf ? "active" : "not active"));

        //Clear all tilemaps
        _baseTilemap.ClearAllTiles();
        _dragTilemap.ClearAllTiles();
        _lowerColliderTilemap.ClearAllTiles();

        //Create new Perlin Noise and generate the base texture from it
        PerlinNoise perlinNoise = new PerlinNoise(_mapSize, _mapScale, _offset);
        Texture2D baseTexture = GenerateBaseTexture();

        //Set the spawn points. The informataion needed for them are updated during the texture generation
        SetSpawnPoints();

        //Update all of the minimaps
        UpdateMinimaps(baseTexture);

        //Fill Base layer with Main
        _baseTilemap.BoxFill((Vector3Int)(_mapSize - Vector2Int.one), _tileSetup.GetGroundTileInfo().tile, 0, 0, _mapSize.x-1, _mapSize.y-1);

        //Set the tiles based on the baseTexture
        ForEachPixel(baseTexture, (x,y) =>
        {
            SetTileToPosition(baseTexture, x, y);
        });

        //_loadingIcon.SetActive(false);
        //Debug.Log("I am " + (_loadingIcon.activeSelf ? "active" : "not active"));
    }

    /// <summary>
    /// Set a tile at the given position based on the pixel of the given texture at the same coordinates.
    /// </summary>
    /// <param name="baseTexture">A texture with the alias colors of the available tiles.</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    private void SetTileToPosition(Texture2D baseTexture, int x, int y)
    {
        //Construct a position based on the coordinates
        Vector2Int position = new Vector2Int(x, y);

        //Get the color of the pixel at the given position
        Color color = baseTexture.GetPixel(x, y);

        //Get the proper tile based on the color
        TileInfo tileInfo = _tileSetup.GetTileInfoByColor(color);

        switch (tileInfo.tileType)
        {
            case TileInfo.TileType.Ground: 
                return; //Main is already filled onto Base layer

            case TileInfo.TileType.Mud: 
                _dragTilemap.SetTile((Vector3Int)position, tileInfo.tile); 
                break;

            case TileInfo.TileType.Water: 
                _lowerColliderTilemap.SetTile((Vector3Int)position, tileInfo.tile); 
                if(tileInfo.divisionPoints.floor == 0f) //if it's the bottom layer (water) add Mud beneath
                    _dragTilemap.SetTile((Vector3Int)position, _tileSetup.GetTileByType(TileInfo.TileType.Mud));
                break;

            case TileInfo.TileType.Forest:
                _upperColliderTilemap.SetTile((Vector3Int)position, tileInfo.tile);
                break;
        }
    }

    /// <summary>
    /// Set the spawn points based on the calculated endpoints.
    /// </summary>
    private void SetSpawnPoints()
    {
        //Setting up Player Spawn Point
        //If there is already a spawn point, destroy it
        if (_playerSpawnPoint != null)
            Destroy(_playerSpawnPoint.gameObject);
        //Instantiate a new one and assign
        _playerSpawnPoint = Instantiate(_playerSpawnPointPrefab, (Vector2)_endPoints.playerSpawnPoint, _playerSpawnPointPrefab.transform.rotation);
        
        //Setting up Objective Spawn Point
        //If there is already a spawn point, destroy it
        if (_objectiveSpawnPoint != null)
            Destroy(_objectiveSpawnPoint.gameObject);
        //Instantiate a new one and assign
        _objectiveSpawnPoint = Instantiate(_objectiveSpawnPointPrefab,(Vector2)_endPoints.objectivePoint, _objectiveSpawnPointPrefab.transform.rotation);
        //Set the parent to the map
        _objectiveSpawnPoint.GetComponent<SpawnPoint>().SetSpawnParent(_parentGrid);
       
        Debug.Log("EndPoints - OBJ: " + _endPoints.objectivePoint + ", PSP: " + _endPoints.playerSpawnPoint);
    }

    /// <summary>
    /// Update the list of minimaps.
    /// </summary>
    /// <param name="baseTexture">The generated base texture.</param>
    private void UpdateMinimaps(Texture2D baseTexture)
    {
        foreach(Minimap minimap in _minimaps)
        {
            minimap.SetTexture(baseTexture);
            minimap.SetEndPointMarkers(_mapSize.x, _endPoints);
        }
    }

    /// <summary>
    /// Iteration shortcut on the pixels of a texture.
    /// </summary>
    /// <param name="texture">A texture which's pixels are getting iterated through.</param>
    /// <param name="action">An action to be applied at every pixel.</param>
    private void ForEachPixel(Texture2D texture, Action<int, int> action)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                action?.Invoke(x,y);
            }
        }
    }

    /// <summary>
    /// Generate a colored texture based on a Perlin Noise map.
    /// </summary>
    /// <param name="perlinMap">A texture based on a Perlin Noise.</param>
    /// <returns>A colored texture.</returns>
    private Texture2D GenerateColoredMap(Texture2D perlinMap)
    {
        //Create a new texture with the same dimensions of the Perlin Noise map
        Texture2D texture = new Texture2D(perlinMap.width, perlinMap.height);

        //Update the division points of the tilesetup
        _tileSetup.UpdateDivisionPoints();

        //Set a color on the new texture based on the Perlin Noise map
        ForEachPixel(perlinMap, (x, y) =>
        {
            SetColorToPixel(perlinMap, x, y, texture);
        });

        //Must set the filter mode to point, as otherwise it would blur the texture and it wouldn't be usable
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Color the pixel at the given position on the texture, based on a Perlin Noise map.
    /// </summary>
    /// <param name="perlinMap">A Perlin Noise map.</param>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="texture">The texture to apply the color on.</param>
    private void SetColorToPixel(Texture2D perlinMap, int x, int y, Texture2D texture)
    {
        //Create a position based on the coordinates
        Vector2Int position = new Vector2Int(x, y);
        
        //Get a color component (such as red) from the Perlin Noise map. All components are the same as it's always a grey.
        float color = perlinMap.GetPixel(x, y).r;

        //Get the alias color of the appropriate tile
        Color aliasColor = _tileSetup.GetTileByColorComponent(color).getAliasColor();

        //Set the alias color at the pixel
        texture.SetPixel(position.x, position.y, aliasColor);
    }

    /// <summary>
    /// Generates the base texture and with the previously set map size, scale and offset.
    /// </summary>
    /// <returns></returns>
    public Texture2D GenerateBaseTexture()
    {
        //Create new Perlin Noise
        PerlinNoise perlinNoise = new PerlinNoise(_mapSize, _mapScale, _offset);

        //Generate a colored texture based on the Perlin Noise
        Texture2D baseTexture = GenerateColoredMap(perlinNoise.GenerateTexture());

        //Generate a partial Perlin Noise texture
        Texture2D partialPerlinTexture = GeneratePartialPerlinTexture(perlinNoise);

        GenerateMarkedTexture(partialPerlinTexture);

        return baseTexture;
    }

    /// <summary>
    /// Generate a partial Perlin Noise map, which only shows the pixels for the main walkable tiles.
    /// </summary>
    /// <param name="perlinNoise">The Perlin Noise map that this generation is based on.</param>
    /// <returns>A partial Perlin Noise texture, only containing the pixels for the main walkable tiles.</returns>
    private Texture2D GeneratePartialPerlinTexture(PerlinNoise perlinNoise)
    {
        //Create a partial Perlin Noise map, which only shows the pixels for the main walkable tiles
        Texture2D partialPerlinTexture = perlinNoise.GeneratePerlinBetweenValues(_tileSetup.GetGroundTileInfo().divisionPoints); //Create partial perlin map

        //If there's a test image given, draw the partial Perlin Noise map on it
        if (_testImagePerlin != null)
            _testImagePerlin.texture = partialPerlinTexture;
        return partialPerlinTexture;
    }

    /// <summary>
    /// Generate a texture with marked points.
    /// These are midpoints, which are equal distances from the walkable tiles' floor and ceil division points,
    /// and endpoints, which are the start and end positions of the game.
    /// </summary>
    /// <param name="perlinTexture">The Perlin Noise texture that the generation is based on.</param>
    /// <returns>A blank texture that only has the marks.</returns>
    private Texture2D GenerateMarkedTexture(Texture2D perlinTexture)
    {
        //Create a new blank texture with the maps dimensions to be the marked one and fill it with clear pixels
        Texture2D markedTexture = new Texture2D(_mapSize.x, _mapSize.y);
        markedTexture = FillTexture(markedTexture, Color.clear);

        //Find the midpoints on the partial Perlin Noise map
        //These mean those points that are equal distances from the walkable tiles' floor and ceil division points
        List<Vector2Int> midPoints = FindMidPoints(perlinTexture, _tileSetup.GetGroundTileInfo().divisionPoints);

        //Mark these midpoints on the new, marked texture
        markedTexture = MarkPoints(midPoints, markedTexture, _midPointColor);

        //Find the endpoints (start and end positions) and set them on the marked texture
        _endPoints = FindEndPoints(midPoints);

        markedTexture = MarkPoints(_endPoints, markedTexture, _playerSpawnPointColor, _objectivePointColor);

        //If there's a test image given, draw the marked map on it
        if (_testImageMarked != null)
            _testImageMarked.texture = markedTexture;

        return markedTexture;
    }

    /// <summary>
    /// Change all of the pixels of the given texture to the specified color.
    /// </summary>
    /// <param name="texture">The texture to recolor.</param>
    /// <param name="color">The color to fill the texture with.</param>
    /// <returns>The texture recolored with the specified color.</returns>
    private Texture2D FillTexture(Texture2D texture, Color color)
    {
        ForEachPixel(texture, (x, y) =>
        {
            texture.SetPixel(x, y, color);
        });

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Find the two endpoints (start and end) for the game.
    /// </summary>
    /// <param name="midPoints">Points to choose the endpoints from.</param>
    /// <returns>The two endpoints (start and end).</returns>
    private EndPoints FindEndPoints(List<Vector2Int> midPoints)
    {
        //Create a point representing the center of the map
        Vector2Int center = new Vector2Int(_mapSize.x/2, _mapSize.y/2);

        //Create dictionaries for separating the points into quarters and keeping their count
        Dictionary<Vector2Int, MapQuarter> pointsInQuarters = new Dictionary<Vector2Int, MapQuarter>();
        Dictionary<MapQuarter, int> counts = new Dictionary<MapQuarter, int>();

        //Divide points into quarters
        foreach (Vector2Int point in midPoints)
        {
            Vector2Int pointFromCenter = new Vector2Int(point.x - center.x, point.y - center.y);
            MapQuarter quarter = WhichQuarter(pointFromCenter);

            AddPointToQuarter(pointsInQuarters, counts, point, quarter);
        }

        //Get the quarter with the most points and also get it's opposite quarter
        MapQuarter objectiveQuarter = counts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        MapQuarter playerSpawnPointQuarter = Opposite(objectiveQuarter);

        //Get the furthest points from each selected quarter, but only at 85% of the maximum distance,
        //to ensure the points not to be at the edges of the map
        return new EndPoints
        {
            objectivePoint = GetFurthestPointInQuarter(pointsInQuarters, objectiveQuarter, center, center.x * 0.85f),
            playerSpawnPoint = GetFurthestPointInQuarter(pointsInQuarters, playerSpawnPointQuarter, center, center.x * 0.85f)
        };
    }

    /// <summary>
    /// Add the given point to the specified quarter and update the count of that quarter.
    /// </summary>
    /// <param name="pointsInQuarters">A dictionary where the points are stored regarding their quarters.</param>
    /// <param name="counts">A dictionary where the count of the points in the same quarters are stored.</param>
    /// <param name="point">The point to add to the quarter.</param>
    /// <param name="mapQuarter">The quarter to add the point to.</param>
    private void AddPointToQuarter(Dictionary<Vector2Int, MapQuarter> pointsInQuarters, Dictionary<MapQuarter, int> counts, Vector2Int point, MapQuarter mapQuarter)
    {
        pointsInQuarters.Add(point, mapQuarter);

        if (counts.ContainsKey(mapQuarter))
            counts[mapQuarter] += 1;
        else
            counts[mapQuarter] = 1;
    }
     /// <summary>
     /// Get the furthest point from the center, at the maximum distance.
     /// </summary>
     /// <param name="pointsInQuarters">A dictionary where the points are stored regarding the quarters they are in.</param>
     /// <param name="myQuarter">The quarter to search in.</param>
     /// <param name="center">The center point to search by.</param>
     /// <param name="maxDistance">The maximum distance to search at.</param>
     /// <returns>A point that is the furthest away from the center at the maximum distance.</returns>
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

    /// <summary>
    /// Mark the given points on the texture.
    /// </summary>
    /// <param name="points">The points to mark on the texture.</param>
    /// <param name="texture">The texture to mark on.</param>
    /// <param name="color">The color to mark the points with.</param>
    /// <returns>The same texture, marked.</returns>
    private Texture2D MarkPoints(List<Vector2Int> points, Texture2D texture, Color color)
    {
        foreach(Vector2Int position in  points)
        {
            texture.SetPixel(position.x, position.y, color);
        }

        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Mark the given points on the texture.
    /// </summary>
    /// <param name="endPoints">The endpoints to mark on the texture.</param>
    /// <param name="texture">The texture to mark on.</param>
    /// <param name="playerPointColor">The color to mark the player point with.</param>
    /// <param name="objectiveColor">The color to mark the objective point with.</param>
    /// <returns>The same texture, marked.</returns>
    private Texture2D MarkPoints(EndPoints endPoints, Texture2D texture, Color playerPointColor, Color objectiveColor)
    {
        texture.SetPixel(endPoints.playerSpawnPoint.x, endPoints.playerSpawnPoint.y, playerPointColor);
        texture.SetPixel(endPoints.objectivePoint.x, endPoints.objectivePoint.y, objectiveColor);

        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Find all of the midpoints based on the given Perlin Noise texture.
    /// </summary>
    /// <param name="perlinTexture">The Perlin Noise texture that the search is based on.</param>
    /// <param name="divisionPoints">The floor and ceil values of a TileInfo.</param>
    /// <returns>
    /// A list of points that are equal distances from the 
    /// floor and ceil values on the Perlin Noise texture.
    /// </returns>
    private List<Vector2Int> FindMidPoints(Texture2D perlinTexture, TileInfo.DivisionPoints divisionPoints)
    {
        List<Vector2Int> midPoints = new List<Vector2Int>();

        float mid = (divisionPoints.floor + divisionPoints.ceil) / 2f; //The middle between floor and ceil
        decimal roundedMid = Math.Round((decimal)mid, 2);

        ForEachPixel(perlinTexture, (x, y) =>
        {
            Vector2Int position = new Vector2Int(x, y);
            float color = perlinTexture.GetPixel(x, y).r;
            decimal roundedColor = Math.Round((decimal)color, 2);

            if (roundedColor == roundedMid) //If the current pixel is the same value as the mid
            {
                midPoints.Add(position);    //Add the position to the list
            }
        });

        return midPoints;
    }


    /// <summary>
    /// Translates the given string to a number.
    /// If the seed is too short or too long, the value gets to be corrected automatically.
    /// </summary>
    /// <param name="seed">A 0-10 characters long text.</param>
    public void SetOffsetFromSeed(string seed)
    {
        long.TryParse(seed, out long numSeed); //Setting to a number
        string newSeed = numSeed.ToString("0000000000"); //Setting back to string to get 10 decimals for splitting
        string part1 = newSeed.Substring(0, 5); //Splitting the first half
        string part2 = newSeed.Substring(5, 5); //Splitting the second half

        int.TryParse(part1, out int offsetX);
        int.TryParse(part2, out int offsetY);

        SetOffsetX(offsetX);
        SetOffsetY(offsetY);
    }

    /// <summary>
    /// Translates the given string to a number.
    /// If the seed is too short or too long, the value gets to be corrected automatically.
    /// </summary>
    /// <param name="inputField">An Input Field to get the seed text from.</param>
    public void SetOffSetFromSeed(TMP_InputField inputField)
    {
        SetOffsetFromSeed(inputField.text);
    }

    /// <summary>
    /// Find the opposite quarter of the given quarter.
    /// </summary>
    /// <param name="quarter">The quarter which opposite is searched.</param>
    /// <returns>The opposite quarter of the given quarter.</returns>
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

    /// <summary>
    /// Generate a random seed and set the seed value into an Input Field.
    /// </summary>
    /// <param name="inputField">The Input Field to set the seed value into.</param>
    public void GenerateRandomSeed(TMP_InputField inputField)
    {
        int lenght = 10;
        long seed = 0;

        for(int i = 0; i < lenght; i++)
        {
            int digit = UnityEngine.Random.Range(0, 10);
            long addition = (long)(Math.Pow(10, i) * digit);            
            seed += addition;
        }

        inputField.SetTextWithoutNotify(seed.ToString());

        SetOffsetFromSeed(seed.ToString());
    }

    /// <summary>
    /// Find which quarter is the point in, when it is viewed from the center of the map.
    /// </summary>
    /// <param name="pointFromCenter">A point from the center.</param>
    /// <returns>The quarter in which the given point is in.</returns>
    private MapQuarter WhichQuarter(Vector2Int pointFromCenter)
    {
        if (pointFromCenter.x < 0 && pointFromCenter.y >= 0)
            return MapQuarter.TopLeft;

        if (pointFromCenter.x >= 0 && pointFromCenter.y >= 0)
            return MapQuarter.TopRight;

        if (pointFromCenter.x < 0 && pointFromCenter.y < 0)
            return MapQuarter.BottomLeft;

        if (pointFromCenter.x >= 0 && pointFromCenter.y < 0)
            return MapQuarter.BottomRight;

        return MapQuarter.TopLeft;
    }

    #region Getters and setters

    public void SetSizeX(int mapSizeX)
    {
        _mapSize.x = mapSizeX;
    }

    public void SetSizeY(int mapSizeY)
    {
        _mapSize.y = mapSizeY;
    }

    public void SetScale(float scale)
    {
        _mapScale = scale;
    }

    public void SetOffsetX(int offsetX)
    {
        _offset.x = offsetX;
    }

    public void SetOffsetY(int offsetY)
    {
        _offset.y = offsetY;
    }

    public TileSetup GetTileSetups()
    {
        return _tileSetup;
    }

    public EndPoints GetEndPoints()
    {
        return _endPoints;
    }

    #endregion

    #region Enums and structs

    private enum MapQuarter { TopLeft, TopRight, BottomLeft, BottomRight };

    public struct EndPoints
    {
        internal Vector2Int objectivePoint;
        internal Vector2Int playerSpawnPoint;
    }

    #endregion
}