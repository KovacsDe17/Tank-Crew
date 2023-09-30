using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for the tanks movement
/// </summary>
public class TankMove : MonoBehaviour
{
    private Lever _leftLever, _rightLever;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _turnSpeed;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    private void Awake()
    {
        InitalizeValues();
    }

    private void FixedUpdate()
    {
        if (_leftLever == null || _rightLever == null) return;

        MoveByDirections(_leftLever.GetNormalizedValue(), _rightLever.GetNormalizedValue());
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
    public void MoveByDirections(float leftLeverNormalized, float rightLeverNormalized)
    {
        rigidBody.AddForce(transform.up * (leftLeverNormalized + rightLeverNormalized) * _moveSpeed);
        rigidBody.AddTorque((rightLeverNormalized - leftLeverNormalized) * _turnSpeed);
    }

    public void SetLevers(Lever leftLever, Lever rightLever)
    {
        _leftLever = leftLever;
        _rightLever = rightLever;
    }
}
