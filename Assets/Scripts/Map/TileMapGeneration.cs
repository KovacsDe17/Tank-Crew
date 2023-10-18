using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileMapGeneration : MonoBehaviour
{
    [SerializeField] private RawImage testImage;

    [SerializeField] private Tilemap tilemap;

    [Space]
    [Header("Map setup")]
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private float _mapScale;
    [SerializeField] private Vector2Int _offset;
    [Space]

    [SerializeField] private List<TileSetup> tiles;

    private List<float> _normalizedAbsoluteThresholds = new List<float>();
    [SerializeField] private List<Slider> valuesFromSlider = new List<Slider>();

    public void GenerateTileMap()
    {
        tilemap.ClearAllTiles();

        PerlinNoise perlinNoise = new PerlinNoise(_mapSize, _mapScale, _offset);
        Texture2D texture = perlinNoise.GenerateTexture();

        UpdateThresholds();

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                float color = texture.GetPixel(x, y).r;

                //tilemap.SetTile((Vector3Int)position, GetTileByColorUniform(color));
                tilemap.SetTile((Vector3Int)position, GetTileTypeByColorThresholds(color).tile);
            }
        }
    }

    private Texture2D GenerateColoredMap(Texture2D perlinMap)
    {
        Texture2D texture = new Texture2D(perlinMap.width, perlinMap.height);

        UpdateThresholds();

        for (int x = 0; x < perlinMap.width; x++)
        {
            for (int y = 0; y < perlinMap.height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                float color = perlinMap.GetPixel(x, y).r;

                texture.SetPixel(position.x, position.y, GetTileTypeByColorThresholds(color).getAverageColor());
            }
        }

        texture.Apply();

        return texture;
    }

    public void GenerateOnTestImage()
    {
        PerlinNoise perlinNoise = new PerlinNoise(_mapSize, _mapScale, _offset);
        Texture2D texture = GenerateColoredMap(perlinNoise.GenerateTexture());

        testImage.texture = texture;
    }

    private Tile GetTileByColorUniform(float color)
    {
        float span = (float)1 / tiles.Count;

        int level = Mathf.FloorToInt(color / span);

        if (level >= tiles.Count)
            level = tiles.Count - 1;

        return tiles[level].tile;
    }

    private void UpdateThresholds()
    {
        List<float> normalizedRelativeThresholds = new List<float>();
        float relativeSum = 0;

        foreach (TileSetup ts in tiles)
        {
            relativeSum += ts.threshold;
        }

        foreach (TileSetup ts in tiles)
        {
            normalizedRelativeThresholds.Add(ts.threshold / relativeSum);
        }

        float absoluteSum = 0;
        foreach(float threshold in normalizedRelativeThresholds)
        {
            _normalizedAbsoluteThresholds.Add(absoluteSum);
            absoluteSum += threshold;
        }

        _normalizedAbsoluteThresholds.Reverse();
    }

    private TileSetup GetTileTypeByColorThresholds(float color)
    {
        int level = tiles.Count - 1;

        foreach(float threshold in _normalizedAbsoluteThresholds)
        {
            if (color >= threshold)
                break;
            else
                level--;
        }

        if (level < 0)
            level = 0;

        return tiles[level];
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

    public void SaveTilesThreshold()
    {
        Debug.Log("Saved thresholds:\n\n");

        int index = 0;
        foreach(TileSetup setup in tiles)
        {
            setup.SetThreshold(valuesFromSlider[index].value);
            Debug.Log("Setup threshold for " + setup.tile.name + " is " + setup.threshold.ToString("0.000"));
            index++;
        }

    }

    public List<TileSetup> GetTileSetups()
    {
        return tiles;
    }

    [System.Serializable]
    public class TileSetup
    {
        [SerializeField] internal Tile tile;
        [SerializeField] [Range(0f, 1f)] internal float threshold;
        [SerializeField] internal Color averageColor;

        public float GetPercentage()
        {
            return threshold;
        }

        public void SetThreshold(float percentage)
        {
            threshold = percentage;
        }

        internal Color getAverageColor() {
            return averageColor;
        }
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
}
