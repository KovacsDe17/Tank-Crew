using UnityEngine;

/// <summary>
/// This class is responsible for the movement of the tanks turret
/// </summary>
// TODO: Another way to move the turret?
public class TurretMove : MonoBehaviour
{
    public RectTransform crankRectTransform;
    private float rotationScale = 0.125f;
    private float rotation, prevoiusRotation, rotationDifference, prevoiusRotationDifference;
    private int rotationOverflow;

    private void Awake()
    {
        InitalizeValues();
    }

    void FixedUpdate()
    {
        rotation = Mathf.RoundToInt(crankRectTransform.rotation.eulerAngles.z);
        rotationDifference = rotation - prevoiusRotation;

        //TODO: Reduce rotation jumping by using Lerp or something like that

        //If the signs of the differences are NOT equal and their difference is equal or higher than 270°, check the direction
        if(rotationDifference*prevoiusRotationDifference <= 0 && Mathf.Abs(rotationDifference) >= 270)
        {
            //If the direction is counter-clockwise, add 1 to the overflow
            if (rotationDifference < 0)
                rotationOverflow += 1;
            //If the direction is clockwise, subtract 1 from the overflow
            else if (rotationDifference > 0)
                rotationOverflow -= 1;
        }

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            (rotation + 360 * rotationOverflow) * rotationScale + transform.parent.rotation.eulerAngles.z
            );
        
        prevoiusRotation = rotation;
        prevoiusRotationDifference = rotationDifference;
    }

    /// <summary>
    /// Set the starting rotation and the rotation overflow values of the turret to be 0
    /// </summary>
    private void InitalizeValues()
    {
        rotation = Mathf.RoundToInt(crankRectTransform.rotation.eulerAngles.z) + 360 * rotationOverflow;
        prevoiusRotation = rotation;
        rotationDifference = rotation - prevoiusRotation;
        prevoiusRotationDifference = rotationDifference;
        rotationOverflow = 0;
    }

    //TODO: Break up FixedUpdate code into smaller functions
}
