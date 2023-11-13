using System;
using UnityEngine;

/// <summary>
/// Tank of the Players
/// </summary>
public class PlayerTank : Entity
{
    /// <summary>
    /// Singleton instance of the Players tank
    /// </summary>
    public static PlayerTank Instance { get; private set; }    //Singleton instance

    [HideInInspector] public EventHandler OnPlayerDestroyed;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AssignHealthBar();
    }

    private void AssignHealthBar()
    {
        SetHealthBar(GameManager.Instance.GetPlayerHealthBar());
    }

    public override void Die()
    {
        Debug.Log("The Player has Died!");
        OnPlayerDestroyed?.Invoke(this, EventArgs.Empty);

        //base.Die();
    }
}
