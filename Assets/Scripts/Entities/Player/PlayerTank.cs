using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerTank : Entity
{
    public static PlayerTank Instance { get; private set; }    //Singleton instance

    [HideInInspector] public UnityEvent OnPlayerDestroyed;
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
        OnPlayerDestroyed.Invoke();

        //base.Die();
    }
}
