using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileMapGeneration : MonoBehaviour
{
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

    public void GenerateMap()
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
                tilemap.SetTile((Vector3Int)position, GetTileByColorThresholds(color));
            }
        }
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

    private Tile GetTileByColorThresholds(float color)
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

        return tiles[level].tile;
    }
    public void SetSizeX(string text)
    {
        int.TryParse(text, out int num);

        _mapSize.x = num;
    }

    public void SetSizeY(string text)
    {
        int.TryParse(text, out int num);

        _mapSize.y = num;
    }


    public void SetScale(string text)
    {
        float.TryParse(text, out float num);

        _mapScale = num;
    }

    public void SetOffsetX(string text)
    {
        int.TryParse(text, out int num);

        _offset.x = num;
    }

    public void SetOffsetY(string text)
    {
        int.TryParse(text, out int num);

        _offset.y = num;
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

        public float GetPercentage()
        {
            return threshold;
        }

        public void SetThreshold(float percentage)
        {
            threshold = percentage;
        }
    }
}
