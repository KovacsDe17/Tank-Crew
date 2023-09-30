using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The 2D object of the projectile which is shot by a tank
/// </summary>
public class AmmoProjectile : NetworkBehaviour
{
    [SerializeField]
    private float _moveSpeed = 25f;
    [SerializeField]
    private float _damage = 75f;
    [SerializeField]
    private GameObject _explosionEffect;

    [SerializeField]
    private LayerMask _layerMask;

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
        _rigidBody.AddForce(transform.up * _moveSpeed, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Action when the projectile hit something
    /// </summary>
    /// <param name="collision">The object which the projectile collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_layerMask.value & 1 << collision.gameObject.layer) != 0)
        {
            Tank tank = collision.collider.GetComponent<Tank>();
            if(tank != null)
            {
                tank.TakeDamage(_damage);
            }

            //Instantiate(_explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

    }
}
