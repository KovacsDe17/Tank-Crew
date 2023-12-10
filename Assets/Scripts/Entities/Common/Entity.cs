using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The representation of a tank.
/// </summary>
public class Entity : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth = 100f;

    [SerializeField] private Slider _healthBar;
    [SerializeField] private GameObject _destroyedEntity;
    private Turret _turret;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    /// <summary>
    /// Setup health and health bar
    /// </summary>
    private void Initialize()
    {
        _turret = GetComponentInChildren<Turret>();

        UpdateHealthBar();
    }

    /// <summary>
    /// Change the health bar according to the current health status
    /// </summary>
    private void UpdateHealthBar()
    {
        if (_healthBar == null) {
            Debug.LogWarning("Health Bar for the Entity is not set!");
            return;
        }

        _healthBar.value = _currentHealth / _maxHealth;
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    /// <summary>
    /// Decrease the health by the given amount
    /// </summary>
    /// <param name="damage">The amount of points to subtract</param>
    public void TakeDamage(float damage)
    {
        if(_currentHealth <= damage)
        {
            _currentHealth = 0f;
            Die();
        } else
        {
            _currentHealth -= damage;
        }

        UpdateHealthBar();

        AudioManager.Instance.PlaySound(AudioManager.Sound.Tank_Hit, transform.position);
    }

    /// <summary>
    /// Increase the health by the given amount
    /// </summary>
    /// <param name="health">The amount of points to add</param>
    public void RestoreHealth(float health)
    {
        if ((_currentHealth + health) > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        else
        {
            _currentHealth += health;
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// Change the Entity to its destroyed version, while maintaining the physics attributes
    /// </summary>
    public virtual void Die()
    {
        Rigidbody2D _rigidbody = gameObject.GetComponent<Rigidbody2D>();

        GameObject deadEnemy = Instantiate(_destroyedEntity, transform.position, transform.rotation);

        if(_turret != null)
        {
            Debug.Log("Entity had a turret, setting its rotation...");
            Transform deadEntityTurret = deadEnemy.GetComponentInChildren<Turret>().transform;
            deadEntityTurret.rotation = _turret.transform.rotation;
        }


        Rigidbody2D rigidbody = deadEnemy.GetComponent<Rigidbody2D>();
        rigidbody.velocity = _rigidbody.velocity;
        rigidbody.angularVelocity = _rigidbody.angularVelocity;

        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    public Turret GetTurret()
    {
        return _turret;
    }

    public void SetHealthBar(Slider healthBar)
    {
        _healthBar = healthBar;
    }
}