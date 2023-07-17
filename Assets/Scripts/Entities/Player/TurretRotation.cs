using UnityEngine;

/// <summary>
/// This class is responsible for the movement of the tanks turret
/// </summary>
// TODO: Another way to rotate the turret?
public class TurretRotation : MonoBehaviour
{
    public Crank crank;

    [SerializeField]
    private float _catchUpSpeed = 5f;

    private float _rotationScale = 0.125f; //Rotate 1/8 of the given degrees (1/8 turn on 1 full rotation)
    private float _rotation, _previousRotation, _rotationDifference, _previousRotationDifference;
    private int _rotationOverflow;

    private void Awake()
    {
        InitalizeValues();
    }

    void FixedUpdate()
    {
        UpdateRotationalValues();

        UpdateRotationOverflow();
        CalculateAndSetRotation();

        UpdatePreviousRotationalValues();
    }

    /// <summary>
    /// Update the last known rotation values for calculating the differences between frames
    /// </summary>
    private void UpdatePreviousRotationalValues()
    {
        _previousRotation = _rotation;
        _previousRotationDifference = _rotationDifference;
    }

    /// <summary>
    /// Calculate and set a smooth translation to the desired rotation based on the angle of the crank
    /// </summary>
    private void CalculateAndSetRotation()
    {
        float desiredAngle = (_rotation + 360 * _rotationOverflow) * _rotationScale + transform.parent.rotation.eulerAngles.z;

        Quaternion desiredRotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            desiredAngle
            );

        Quaternion rotation = Quaternion.LerpUnclamped(transform.rotation, desiredRotation, _catchUpSpeed * Time.deltaTime);

        transform.rotation = rotation;
    }

    /// <summary>
    /// Check if the rotation has gone below 0 or above 360 and add or subtract 360 respectively
    /// </summary>
    private void UpdateRotationOverflow()
    {
        //If the signs of the differences are NOT equal and their difference is equal or higher than 270°, check the direction
        if (_rotationDifference * _previousRotationDifference <= 0 && Mathf.Abs(_rotationDifference) >= 270)
        {
            //If the direction is counter-clockwise, add 1 to the overflow
            if (_rotationDifference < 0)
                _rotationOverflow += 1;
            //If the direction is clockwise, subtract 1 from the overflow
            else if (_rotationDifference > 0)
                _rotationOverflow -= 1;
        }
    }

    /// <summary>
    /// Update rotation to equal to rotation of the crank
    /// </summary>
    private void UpdateRotationalValues()
    {
        _rotation = Mathf.RoundToInt(crank.getRotation());
        _rotationDifference = _rotation - _previousRotation;
    }

    /// <summary>
    /// Set the starting rotation and the rotation overflow values of the turret to be 0
    /// </summary>
    private void InitalizeValues()
    {
        _rotation = Mathf.RoundToInt(crank.getRotation()) + 360 * _rotationOverflow;
        _previousRotation = _rotation;
        _rotationDifference = _rotation - _previousRotation;
        _previousRotationDifference = _rotationDifference;
        _rotationOverflow = 0;
    }
}
