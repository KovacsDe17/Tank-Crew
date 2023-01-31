using UnityEngine;

/// <summary>
/// The 2D object of the projectile which is shot by a tank
/// </summary>
public class AmmoProjectile : MonoBehaviour
{
    public float moveSpeed = 1f;
        
    private float _timeAlive = 5f;
    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        Destroy(gameObject, _timeAlive);   
    }

    /// <summary>
    /// Assign the Rigidbody2D component to the object, and apply force to move it forward
    /// </summary>
    private void Initialize()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);
    }
}
