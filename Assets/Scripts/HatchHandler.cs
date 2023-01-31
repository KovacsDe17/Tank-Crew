using UnityEngine;

/// <summary>
/// The UI handler for the hatch object of the turret.
/// </summary>
public class HatchHandler : MonoBehaviour
{
    public float minDegree, maxDegree;
    public Turret turret;

    private RectTransform _hatchRectTransform;
    private Vector2 _touchPosition;
    private bool _isOpen;
    private UIAmmo _loadedAmmo;

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Assign the RectTransform to the object, and set the rotation to the closed state
    /// </summary>
    private void Initialize()
    {
        _hatchRectTransform = GetComponent<RectTransform>();
        _hatchRectTransform.eulerAngles = new Vector3(0f, 0f, minDegree);

        _isOpen = false;
        _loadedAmmo = null;
    }

    /// <summary>
    /// Set the rotation of the hatch to look at the touch position
    /// </summary>
    public void LookAtFinger()
    {
        _touchPosition = Application.isEditor ? (Vector2) Input.mousePosition : Input.GetTouch(0).position;

        float rotation = GetRotationToPosition(_hatchRectTransform.position, _touchPosition);

        if (rotation < minDegree || rotation > maxDegree)
        {
            return;
        }

        if (CloseToRotation(rotation, minDegree))
            rotation = minDegree;

        if (CloseToRotation(rotation, maxDegree))
            rotation = maxDegree;

        //Set the rotation to make the crank "look at" the touch position
        _hatchRectTransform.rotation = Quaternion.Euler(
            _hatchRectTransform.eulerAngles.x,
            _hatchRectTransform.eulerAngles.y,
            rotation
            );
    }

    /// <summary>
    /// Check if two rotations are close to each other, irrespectively of overlapping
    /// </summary>
    /// <param name="rotationA">The rotation to compare</param>
    /// <param name="rotationB">The rotation to compare to</param>
    /// <returns>Whether the difference of the rotations are in a certain proximity</returns>
    private bool CloseToRotation(float rotationA, float rotationB)
    {
        float proximity = 15f;

        float simple = Mathf.Abs(rotationA - rotationB);
        float added = Mathf.Abs(rotationA - rotationB + 360);
        float subtracted = Mathf.Abs(rotationA - rotationB - 360);

        if (simple <= proximity || added <= proximity || subtracted <= proximity)
            return true;

        return false;
    }

    /// <summary>
    /// Get the absolute rotational value, where the object is set to look at a position
    /// </summary>
    /// <param name="center"></param>
    /// <param name="desiredPosition"></param>
    /// <returns>The rotational value</returns>
    private float GetRotationToPosition(Vector2 center, Vector2 desiredPosition)
    {
        return Mathf.Atan2((desiredPosition.y - center.y), (desiredPosition.x - center.x)) * Mathf.Rad2Deg + 180;
    }

    /// <summary>
    /// Check if the hatch is open
    /// </summary>
    /// <returns>Whether the hatch is open</returns>
    public bool IsOpen()
    {
        return _isOpen;
    }

    /// <summary>
    /// Set the ammunition to be loaded
    /// </summary>
    /// <param name="ammo">The UI element which represents the ammunition</param>
    public void LoadAmmo(UIAmmo ammo)
    {
        _loadedAmmo = ammo;
    }

    /// <summary>
    /// Discard the currently loaded ammunition
    /// </summary>
    public void UnloadAmmo()
    {
        _loadedAmmo = null;
    }

    /// <summary>
    /// If it is possible to shoot, remove the head of the ammunitions UI representation and fire the turret
    /// </summary>
    public void Shoot()
    {
        if (_loadedAmmo == null || !_loadedAmmo.CanBeShot() || IsOpen())
            return;

        _loadedAmmo.RemoveHead();
        turret.Fire();
    }
}
