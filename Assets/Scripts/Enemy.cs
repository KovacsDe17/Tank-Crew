using System;
using UnityEngine;
using UnityEngine.UI;

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

    private void Initialize()
    {
        _currentHealth = _maxHealth;
        _healthBar = GetComponentInChildren<Slider>();

        UpdateHealthBar();
    }

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

    private void UpdateHealthBar()
    {
        _healthBar.value = _currentHealth / _maxHealth;
    }
}
