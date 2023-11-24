using System;
using System.Threading.Tasks;
using Unity.Netcode;
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        AssignHealthBar();
    }

    private void AssignHealthBar()
    {
        SetHealthBar(GameManager.Instance.GetPlayerHealthBar());
    }

    public override async void Die()
    {
        Debug.Log("The Player has Died!");
        OnPlayerDestroyed?.Invoke(this, EventArgs.Empty);

        //base.Die();

        await Task.Delay(2000); //Wait for two seconds

        GameManager.Instance.InvokeOnGameEnd(false);
    }
}
