using UnityEngine;

/// <summary>
/// This is responsible for the UI actions of the turret crank
/// </summary>
public class Crank : MonoBehaviour
{
    [SerializeField]
    [Header("Number of predefined steps")]
    private int _subdivision = 24;

    private RectTransform _rectTransform;
    private Vector2 _touchPosition;

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Assign the RectTransform component to the object and set the rotation to zero
    /// </summary>
    private void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    /// <summary>
    /// Rotate the crank to look at the closest touch position
    /// </summary>
    public void LookAtFinger()
    {
        float touchDirection = GetTouchDirection();
        RotateCrankTo(touchDirection);
    }

    /// <summary>
    /// Get the direction of the closest touch (or the mouse, in the Editor) to the handle
    /// </summary>
    /// <returns>The direction in Euler angles. Up is 0, right is 90 degrees</returns>
    private float GetTouchDirection()
    {
        _touchPosition = Application.isEditor ? Input.mousePosition : GetClosestTouchPosition();

        float directionX = _touchPosition.x - _rectTransform.position.x;
        float directionY = _touchPosition.y - _rectTransform.position.y;

        return Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg - 90;
    }

    /// <summary>
    /// Rotate the crank to the desired rotation
    /// </summary>
    /// <param name="rotation">Rotation in Euler angles. Up is 0, right is 90 degrees</param>
    private void RotateCrankTo(float rotation)
    {
        _rectTransform.rotation = Quaternion.Euler(
            _rectTransform.eulerAngles.x,
            _rectTransform.eulerAngles.y,
            SnapAngleToSteps(rotation)
            );
    }

    /// <summary>
    /// Get the 2D position of the closest touch to the handle
    /// </summary>
    /// <returns>The Vector3 coordinates of the touch position in screen points</returns>
    private Vector3 GetClosestTouchPosition()
    {
        Touch closest = Input.GetTouch(0);

        foreach (Touch touch in Input.touches)
        {
            float currentTouchDistance = Vector2.Distance(touch.position, _rectTransform.position);
            float closestTouchDistance = Vector2.Distance(closest.position, _rectTransform.position);

            if (currentTouchDistance < closestTouchDistance)
            {
                closest = touch;
            }
        }

        return closest.position;
    }

    /// <summary>
    /// Snaps the given rotation to calculated steps depending on predefined number of subdivisions (this can be set from the Editor)
    /// </summary>
    /// <param name="rotation">The rotation to snap to a calculated step</param>
    /// <returns>The snapped rotation angle</returns>
    private float SnapAngleToSteps(float rotation)
    {
        float step = 360 / _subdivision;
        float remainder = rotation % step;

        float angle = rotation - remainder;

        return angle;
    }

    public float getRotation()
    {
        return transform.rotation.eulerAngles.z;
    }
}
