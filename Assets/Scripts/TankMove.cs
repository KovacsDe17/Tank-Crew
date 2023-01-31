using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for the tanks movement
/// </summary>
public class TankMove : MonoBehaviour
{
    public Scrollbar leverLeftScrollbar, leverRightScrollbar;
    public float moveSpeed, turnSpeed;
    private float leftMoveDirection, rightMoveDirection;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    private void Awake()
    {
        InitalizeValues();
    }

    private void Update()
    {
        NormalizeMoveDirections();
        MoveByDirections();
    }

    /// <summary>
    /// Assigns the Rigidbody2D component
    /// </summary>
    private void InitalizeValues()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Move the tank related to the levers handles positions
    /// </summary>
    private void MoveByDirections()
    {
        rigidBody.AddForce(transform.up * (leftMoveDirection + rightMoveDirection) * moveSpeed);
        rigidBody.AddTorque((rightMoveDirection - leftMoveDirection) * turnSpeed);
    }

    /// <summary>
    /// Correct the move direction values of handles to the -1.0-1.0 range
    /// </summary>
    private void NormalizeMoveDirections()
    {
        leftMoveDirection = Normalize(leverLeftScrollbar.value);
        rightMoveDirection = Normalize(leverRightScrollbar.value);
    }

    /// <summary>
    /// Transform the 0.0-1.0 range of the lever handle to a -1.0-1.0 range
    /// </summary>
    /// <param name="value">The value based on the lever handles position</param>
    /// <returns>The normalized value of the lever handles position</returns>
    private float Normalize(float value)
    {
        return (value - 0.5f) * -2f;
    }
}
