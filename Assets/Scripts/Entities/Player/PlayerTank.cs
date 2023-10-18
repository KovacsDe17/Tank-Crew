using System;
using UnityEngine;

public class PlayerTank : Entity
{
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

    public override void Die()
    {
        Debug.Log("The Player has Died!");
        OnPlayerDestroyed?.Invoke(this, EventArgs.Empty);

        //base.Die();
    }
}
