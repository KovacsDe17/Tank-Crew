using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The casual enemies the player can destroy
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth;

    private float _currentHealth;

    private Slider _healthBar;

    [SerializeField]
    private GameObject _deadEnemy;

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Setup health and health bar
    /// </summary>
    private void Initialize()
    {
        _currentHealth = _maxHealth;
        _healthBar = GetComponentInChildren<Slider>();

        UpdateHealthBar();
    }

    /// <summary>
    /// Decrease the health by the given amount
    /// </summary>
    /// <param name="damage">The amount of points to subtract</param>
    public void TakeDamage(float damage)
    {
        if (damage > _currentHealth)
        {
            Die();
            return;
        } else
        {
            _currentHealth -= damage;
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// Change the enemy to its destroyed version, while maintaining the physics attributes
    /// </summary>
    private void Die()
    {
        _currentHealth = 0;

        Rigidbody2D _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        
        GameObject deadEnemy = Instantiate(_deadEnemy, transform.position, transform.rotation);
        Rigidbody2D rigidbody = deadEnemy.GetComponent<Rigidbody2D>();

        rigidbody.velocity = _rigidbody.velocity;
        rigidbody.angularVelocity = _rigidbody.angularVelocity;

        Destroy(gameObject);
    }

    /// <summary>
    /// Change the health bar according to the current health status
    /// </summary>
    private void UpdateHealthBar()
    {
        _healthBar.value = _currentHealth / _maxHealth;
    }
}
