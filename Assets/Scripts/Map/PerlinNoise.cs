using UnityEngine;

/// <summary>
/// Perlin Noise for map generation purposes.
/// </summary>
public class PerlinNoise
{
    private int _width = 256;
    private int _height = 256;

    private float _scale = 20f;
    private float _offsetX = 0f;
    private float _offsetY = 0f;

    public PerlinNoise()
    {
        _scale = 20f;
        _offsetX = 0f;
        _offsetY = 0f;
    }

    public PerlinNoise(float scale, Vector2Int offset)
    {
        _scale = scale;
        _offsetX = offset.x;
        _offsetY = offset.y;
    }

    public PerlinNoise(Vector2Int size, float scale, Vector2Int offset)
    {
        _width = size.x;
        _height = size.y;

        _scale = scale;
        _offsetX = offset.x;
        _offsetY = offset.y;
    }

    /// <summary>
    /// Generate a Perlin Noise texture.
    /// </summary>
    /// <returns>A monochrome texture, where values are between 0f and 1f.</returns>
    public Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(_width, _height);

        for(int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Color color = GenerateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Generate a Perlin Noise texture where the values only exist between the specified floor and ceil values.
    /// </summary>
    /// <param name="floor">The lower bound.</param>
    /// <param name="ceil">The upper bound.</param>
    /// <returns>A monochrome texture, where values are between floor and ceil.</returns>
    public Texture2D GenerateTextureBetweenValues(float floor, float ceil)
    {
        Texture2D texture = new Texture2D(_width, _height);

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Color color = GenerateColor(x, y);
                if(color.r >= floor && color.r <= ceil)
                    texture.SetPixel(x, y, color);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Generate a Perlin Noise texture where the values only exist between the specified division points.
    /// </summary>
    /// <param name="divisionPoints">The lower and upper bounds.</param>
    /// <returns>A monochrome texture, where values are between division points.</returns>
    public Texture2D GeneratePerlinBetweenValues(TileInfo.DivisionPoints divisionPoints)
    {
        return GenerateTextureBetweenValues(divisionPoints.floor, divisionPoints.ceil);
    }

    /// <summary>
    /// Generate a greyscale color based on the given coordinate of the Perlin Noise.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>A greyscale color.</returns>
    private Color GenerateColor(int x, int y)
    {
        float xCoord = (float)x / _width * _scale + _offsetX;
        float yCoord = (float)y / _height * _scale + _offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }

    public void SetScale(float scale)
    {
        _scale = scale;
    }

    public void SetOffset(float offsetX, float offsetY)
    {
        _offsetX = offsetX;
        _offsetY = offsetY;
    }

    public void SetRandomOffset()
    {
        _offsetX = Random.Range(0f, 999999f);
        _offsetY = Random.Range(0f, 999999f);
    }
}
