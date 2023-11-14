using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minified map based on the generated TileMap, holding information of the start and end points of the game.
/// </summary>
public class Minimap : MonoBehaviour
{
    private RawImage _rawImage; //Raw Image to hold the map texture
    private RectTransform _rectTransform; //Rect Transform of the Raw Image
    [SerializeField] private RectTransform _startPointMarker; //Marker of the start point, child of this object
    [SerializeField] private RectTransform _endPointMarker; //Marker of the end point, child of this object

    /// <summary>
    /// Setup the component variables.
    /// </summary>
    private void Initialize()
    {
        if(_rawImage == null)
            _rawImage = gameObject.GetComponent<RawImage>();

        if(_rectTransform == null)
            _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Set the markers of the endpoints on the minimap.
    /// </summary>
    /// <param name="mapWidth">The width of the original map, which the minimap is based on.</param>
    /// <param name="endPoints">The start and end points of the game.</param>
    public void SetEndPointMarkers(int mapWidth, TileMapGeneration.EndPoints endPoints)
    {
        Initialize();

        float markerImageScale = (_rectTransform.rect.width / mapWidth);

        _startPointMarker.anchoredPosition = (Vector2) endPoints.playerSpawnPoint * markerImageScale;
        _endPointMarker.anchoredPosition = (Vector2) endPoints.objectivePoint * markerImageScale;
    }

    /// <summary>
    /// Set the texture of the minimap.
    /// </summary>
    /// <param name="texture">The texture to apply on the minimap.</param>
    public void SetTexture(Texture2D texture)
    {
        Initialize();

        _rawImage.texture = texture;
    }
}
