using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanAndZoom : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector2 _initialPosition;
    private Vector2 _diffFromCenter;

    private float _zoomOutMin = 1f;
    private float _zoomOutMax = 10f;
    private float _zoomSpeed = 0.001f;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.anchoredPosition;
        _diffFromCenter = Vector2.zero;
    }

    private void Update()
    {
        
    }

    private Vector2 GetTouchPos(int touchIndex)
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#else
        return Input.GetTouch(touchIndex).position;
#endif
    }

    public void SetDiffFromCenter()
    {
        _diffFromCenter = _rectTransform.anchoredPosition - GetTouchPos(0);
    }

    public void FollowPointer()
    {
        _rectTransform.anchoredPosition = GetTouchPos(0) + _diffFromCenter;
    }

    private void Zoom(float increment)
    {
        float zoomedScale = Mathf.Clamp(_rectTransform.localScale.x + increment, _zoomOutMin, _zoomOutMax);

        _rectTransform.localScale = Vector3.one * zoomedScale;
    }

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

    public void ResetMap()
    {
        _rectTransform.anchoredPosition = _initialPosition;
        _rectTransform.localScale = Vector3.one;
    }
}
