using UnityEngine;

/// <summary>
/// This class enables the zoom and movement of this object.
/// </summary>
public class PanAndZoom : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector2 _initialPosition;
    private Vector2 _diffFromCenter;

    private float _zoomMin = 1f;
    private float _zoomMax = 10f;
    private float _zoomSpeed = 0.001f;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.anchoredPosition;
        _diffFromCenter = Vector2.zero;
    }

    private Vector2 GetTouchPos(int touchIndex)
    {
        #if UNITY_EDITOR
        return Input.mousePosition;
        #else
        return Input.GetTouch(touchIndex).position;
        #endif
    }

    /// <summary>
    /// Set difference from center of this object.
    /// </summary>
    public void SetDiffFromCenter()
    {
        _diffFromCenter = _rectTransform.anchoredPosition - GetTouchPos(0);
    }

    /// <summary>
    /// Follow the pointer.
    /// </summary>
    public void FollowPointer()
    {
        _rectTransform.anchoredPosition = GetTouchPos(0) + _diffFromCenter;
    }

    /// <summary>
    /// Zoom by the specified amount.
    /// </summary>
    /// <param name="increment">The amount to zoom by.</param>
    private void Zoom(float increment)
    {
        float zoomedScale = Mathf.Clamp(_rectTransform.localScale.x + increment, _zoomMin, _zoomMax);

        _rectTransform.localScale = Vector3.one * zoomedScale;
    }

    /// <summary>
    /// Pinching the screen.
    /// </summary>
    public void Pinch()
    {
        if (Input.touchCount < 2) return; //if less than 2 fingers are touching the screen

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;


        Zoom(difference * _zoomSpeed);

    }

    /// <summary>
    /// Reset the map to the original scale and position.
    /// </summary>
    public void ResetMap()
    {
        _rectTransform.anchoredPosition = _initialPosition;
        _rectTransform.localScale = Vector3.one;
    }
}
