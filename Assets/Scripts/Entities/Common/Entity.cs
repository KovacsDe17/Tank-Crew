using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The representation of an entity.
/// </summary>
public class Entity : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<float> net_maxHealth = new NetworkVariable<float>(100f);
    private NetworkVariable<float> net_currentHealth = new NetworkVariable<float>(100f);

    [SerializeField] internal Slider _healthBar;
    [SerializeField] internal GameObject _destroyedEntity;

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

        _healthBar.value = net_currentHealth.Value / net_maxHealth.Value;

        Debug.Log("Entity healthbar: " + net_currentHealth.Value + "/" + net_maxHealth.Value);
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

        _healthBar.value = currentHealth / net_maxHealth.Value;

        Debug.Log("Entity healthbar: " + currentHealth + "/" + net_maxHealth.Value);
    }

    public float GetCurrentHealth()
    {
        return net_currentHealth.Value;
    }

    /// <summary>
    /// Decrease the health by the given amount
    /// </summary>
    /// <param name="damage">The amount of points to subtract</param>
    public void TakeDamage(float damage)
    {
        if(net_currentHealth.Value <= damage)
        {
            net_currentHealth.Value = 0f;
            Die();
        } else
        {
            net_currentHealth.Value -= damage;
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
        if ((net_currentHealth.Value + health) > net_maxHealth.Value)
        {
            net_currentHealth.Value = net_maxHealth.Value;
        }
        else
        {
            net_currentHealth.Value += health;
        }

        UpdateHealthBar();
    }

    /// <summary>
    /// Change the Entity to its destroyed version, while maintaining the physics attributes
    /// </summary>
    public virtual void Die()
    {
        Debug.Log("Entity died!");

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

            Rigidbody2D rigidbody = destroyedEntity.GetComponent<Rigidbody2D>();
            rigidbody.velocity = _rigidbody.velocity;
            rigidbody.angularVelocity = _rigidbody.angularVelocity;


            //TODO: way too ugly, make this prettier next time!
            Transform root;
            if (transform.name == "EnemyTank")
                root = gameObject.transform.parent;
            else
                root = transform;

            root.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void SetHealthBar(Slider healthBar)
    {
        _healthBar = healthBar;

        net_currentHealth.OnValueChanged += (p, n) => UpdateHealthBar(n);
    }

    internal bool DestroyedEntityIsFromScene()
    {
        return _destroyedEntity.scene.IsValid();
    }
}