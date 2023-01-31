using UnityEngine;

/// <summary>
/// This class handles the UI representations of the ammunition objects
/// </summary>
public class UIAmmoHandler : MonoBehaviour
{

    public HatchHandler hatchHandler;
    public float baseRotation, loadRotation;
    public RectTransform chamberRectTransform, point;

    private Vector2 originalPosition, touchPosition, chamberPosition;
    private RectTransform ammoRectTransform;
    private bool isReady;
    private UIAmmo uiAmmo;

    void Awake()
    {
        InitializeValues();
    }

    /// <summary>
    /// Assign the RectTransform object and set its original values
    /// </summary>
    private void InitializeValues()
    {
        ammoRectTransform = GetComponent<RectTransform>();
        originalPosition = ammoRectTransform.position;

        hatchHandler = FindObjectOfType<HatchHandler>();
        //chamberRectTransform = hatchHandler.GetComponent<RectTransform>();
        chamberPosition = new Vector2(
            chamberRectTransform.position.x - (chamberRectTransform.rect.width / 2),
            chamberRectTransform.position.y
            );
        uiAmmo = GetComponent<UIAmmo>();

        isReady = false;
        hatchHandler.UnloadAmmo();

        baseRotation = 90;
        loadRotation = 270;
    }

    /// <summary>
    /// Handle the grabbing event
    /// </summary>
    public void OnGrab()
    {
        //Get the touch position (mouse if in editor)
        if (Application.isEditor)
            touchPosition = Input.mousePosition;
        else
            touchPosition = GetClosestTouchPosition();

        if (CloseOnYAxis())
            Debug.Log("Y: " + Mathf.Abs(chamberPosition.y - ammoRectTransform.position.y));

        if (CloseOnXAxis()) { 
            Debug.Log("X: " + (Mathf.Abs(chamberPosition.x - (ammoRectTransform.position.x + 600))));

        }

        Vector3 ammoPos = new Vector3(
            ammoRectTransform.position.x,
            ammoRectTransform.position.y,
            ammoRectTransform.position.z
            );

        Vector3 ammoEndPos = new Vector3(
            ammoRectTransform.position.x + 600,
            ammoRectTransform.position.y,
            ammoRectTransform.position.z
            );

        Debug.DrawLine(ammoPos, ammoEndPos, Color.red);

        SetAmmoRotation(loadRotation);

        point.position = chamberPosition;

        SetAmmoPosition(touchPosition);
    }

    /// <summary>
    /// Snap the UI ammunitions position relative to the hatches position
    /// </summary>
    /// <param name="hatchPosition">The position of the hatch</param>
    private void SnapToLoadPosition(Vector2 hatchPosition)
    {
        float x, y;
        x = hatchPosition.x;

        if (hatchPosition.y - touchPosition.y < 100)
        {
            y = hatchPosition.y - 20;
            isReady = true;
            hatchHandler.LoadAmmo(uiAmmo);
        }
        else
        {
            y = touchPosition.y;
            isReady = false;
            hatchHandler.UnloadAmmo();
        }

        Vector2 calculatedPosition = new Vector2(x, y);
        SetAmmoPosition(calculatedPosition);
    }

    /// <summary>
    /// Check if the UI ammunition is close to the chamber on the Y axis
    /// </summary>
    /// <returns>Whether the object is within the specified proximity regarding the Y axis</returns>
    private bool CloseOnYAxis()
    {
        float proximity = 30f;

        return Mathf.Abs(chamberPosition.y - ammoRectTransform.position.y) <= proximity;
    }


    /// <summary>
    /// Check if the UI ammunition is close to the chamber on the X axis
    /// </summary>
    /// <returns>Whether the object is within the specified proximity regarding the X axis</returns>
    private bool CloseOnXAxis()
    {
        float proximity = 30f;
        //TODO: add width as offset
        return Mathf.Abs(chamberPosition.x - (ammoRectTransform.position.x + 600)) <= proximity;
    }

    /// <summary>
    /// Get the position of the closest touch to the UI ammunition
    /// </summary>
    /// <returns>The position of the closest touch</returns>
    private Vector2 GetClosestTouchPosition()
    {
        Touch closest = Input.GetTouch(0);

        foreach (Touch touch in Input.touches)
        {
            float currentTouchDistance = Vector2.Distance(touch.position, chamberRectTransform.position);
            float closestTouchDistance = Vector2.Distance(closest.position, chamberRectTransform.position);

            if (currentTouchDistance < closestTouchDistance)
            {
                closest = touch;
            }
        }

        return closest.position;
    }

    /// <summary>
    /// Set the position of the UI ammunition object
    /// </summary>
    /// <param name="position">The desired position</param>
    private void SetAmmoPosition(Vector2 position)
    {
        ammoRectTransform.anchoredPosition = position;
    }

    /// <summary>
    /// Set the rotation of the UI ammunition object
    /// </summary>
    /// <param name="rotation">The desired rotation</param>
    private void SetAmmoRotation(float rotation)
    {
        ammoRectTransform.rotation = Quaternion.Euler(ammoRectTransform.rotation.eulerAngles.x, ammoRectTransform.rotation.eulerAngles.y, rotation);
    }

    /// <summary>
    /// Handle the release event
    /// </summary>
    public void OnRelease()
    {
        if (!isReady)
        {
            SetAmmoRotation(baseRotation);
            SetAmmoPosition(originalPosition);
        }

        if (!uiAmmo.CanBeShot())
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Check if the UI ammunition is in the chamber and the hatch is closed
    /// </summary>
    /// <returns>Wheter the UI ammunition is ready</returns>
    public bool IsReady()
    {
        return isReady;
    }
}
