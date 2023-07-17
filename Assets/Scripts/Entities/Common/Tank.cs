using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The representation of a tank.
/// </summary>
public class Tank : MonoBehaviour
{
    private float _maxHealth = 100f;
    private float _currentHealth = 100f;

    private Slider _healthBar;
    private Turret _turret;
    private MachineGun _machineGun;

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            RestoreHealth(10f);

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            TakeDamage(10f);
    }

    private void Initialize()
    {
        _healthBar = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<Slider>();
        _turret = GetComponentInChildren<Turret>();
        _machineGun = GetComponentInChildren<MachineGun>();
    }

    private void UpdateHealthBar()
    {
        _healthBar.value = _currentHealth / _maxHealth;
    }

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

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
    }

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

    public void Die()
    {
        //TODO: change entity to a destroyed version then apply rotation and position (maybe add effects)
        Debug.Log("Whoahh, am I dead now?!");
    }

    public Turret GetTurret()
    {
        return _turret;
    }

    public MachineGun GetMachineGun()
    {
        return _machineGun;
    }
}