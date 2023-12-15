using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The representation of a tank.
/// </summary>
public class Entity : NetworkBehaviour
{
    //[SerializeField] private float _maxHealth = 100f;
    //private float _currentHealth = 100f;

    [SerializeField] private NetworkVariable<float> _maxHealth = new NetworkVariable<float>(100f);
    private NetworkVariable<float> _currentHealth = new NetworkVariable<float>(100f);

    [SerializeField] internal Slider _healthBar;
    [SerializeField] internal GameObject _destroyedEntity;
    internal Turret _turret;

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

        _healthBar.value = _currentHealth.Value / _maxHealth.Value;

        Debug.Log("Entity healthbar: " + _currentHealth.Value + "/" + _maxHealth.Value);
    }

    /// <summary>
    /// Change the health bar according to the current health status
    /// </summary>
    private void UpdateHealthBar(float currentHealth)
    {
        if (_healthBar == null)
        {
            Debug.LogWarning("Health Bar for the Entity is not set!");
            return;
        }

        _healthBar.value = currentHealth / _maxHealth.Value;

        Debug.Log("Entity healthbar: " + currentHealth + "/" + _maxHealth.Value);
    }

    public float GetCurrentHealth()
    {
        return _currentHealth.Value;
    }

    /// <summary>
    /// Decrease the health by the given amount
    /// </summary>
    /// <param name="damage">The amount of points to subtract</param>
    public void TakeDamage(float damage)
    {
        if(_currentHealth.Value <= damage)
        {
            _currentHealth.Value = 0f;
            Die();
        } else
        {
            _currentHealth.Value -= damage;
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
        if ((_currentHealth.Value + health) > _maxHealth.Value)
        {
            _currentHealth.Value = _maxHealth.Value;
        }
        else
        {
            _currentHealth.Value += health;
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// Change the Entity to its destroyed version, while maintaining the physics attributes
    /// </summary>
    public virtual void Die()
    {
        Debug.Log("Enemy died!");

        if (IsServer)
        {
            GameObject destroyedEntity;
            Rigidbody2D _rigidbody = gameObject.GetComponent<Rigidbody2D>();

            if (DestroyedEntityIsFromScene())
            {
                destroyedEntity = _destroyedEntity;
                _destroyedEntity.transform.position = transform.position;
                _destroyedEntity.transform.rotation = transform.rotation;
            }
            else
            {
                destroyedEntity = Instantiate(_destroyedEntity, transform.position, transform.rotation);
                destroyedEntity.GetComponent<NetworkObject>().Spawn();
            }

            if (_turret != null)
            {
                Transform deadEntityTurret = destroyedEntity.GetComponentInChildren<Turret>().transform;
                deadEntityTurret.rotation = _turret.transform.rotation;
            }

            Rigidbody2D rigidbody = destroyedEntity.GetComponent<Rigidbody2D>();
            rigidbody.velocity = _rigidbody.velocity;
            rigidbody.angularVelocity = _rigidbody.angularVelocity;


            //TODO: way to ugly, make this prettier next time!
            Transform root;
            if (transform.name == "EnemyTank")
                root = gameObject.transform.parent;
            else
                root = transform;

            root.GetComponent<NetworkObject>().Despawn();
        }
    }

    public Turret GetTurret()
    {
        return _turret;
    }

    public void SetHealthBar(Slider healthBar)
    {
        _healthBar = healthBar;

        _currentHealth.OnValueChanged += (p, n) => UpdateHealthBar(n);
    }

    internal bool DestroyedEntityIsFromScene()
    {
        return _destroyedEntity.scene.IsValid();
    }
}