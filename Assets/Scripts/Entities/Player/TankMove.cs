using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for the tanks movement
/// </summary>
public class TankMove : MonoBehaviour
{
    public Lever leftLever, rightLever;
    public float moveSpeed, turnSpeed;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    private void Awake()
    {
        InitalizeValues();
    }

    private void Update()
    {
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
        rigidBody.AddForce(transform.up * (leftLever.GetNormalizedValue() + rightLever.GetNormalizedValue()) * moveSpeed);
        rigidBody.AddTorque((rightLever.GetNormalizedValue() - leftLever.GetNormalizedValue()) * turnSpeed);
    }
}
