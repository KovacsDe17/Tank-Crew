using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

/// <summary>
/// This class is responsible for the usage of the common Tank of the Players
/// </summary>
public class MultiplayerTankAccess : NetworkBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static MultiplayerTankAccess Instance { get; private set; }
    private TankMove _tankMove; //Handler for the movement of the tank
    private TurretRotation _turretRotation; //Handler for the rotation of the turret

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        GameManager.Instance.OnPlayerSpawn += Setup;
    }

    /// <summary>
    /// Set the playerTanks transform, the movement and the turret rotation
    /// </summary>
    private void Initialize()
    {
        Transform playerTank = PlayerTank.Instance.transform;

        _tankMove = playerTank.GetComponent<TankMove>();
        _turretRotation = playerTank.GetComponentInChildren<TurretRotation>();
    }

    /// <summary>
    /// Server RPC for the movement.
    /// </summary>
    /// <param name="leverLeftNormalized">Value of the left lever.</param>
    /// <param name="leverRightNormalized">Value of the right lever.</param>
    [ServerRpc(RequireOwnership = false)]
    public void MoveTankServerRPC(float leverLeftNormalized, float leverRightNormalized)
    {
        _tankMove.MoveByDirections(leverLeftNormalized, leverRightNormalized);
    }

    /// <summary>
    /// Server RPC for the turret rotation.
    /// </summary>
    /// <param name="rotation">Value of the crank rotation.</param>
    [ServerRpc(RequireOwnership = false)]
    public void RotateTurretServerRPC(float rotation)
    {
        _turretRotation.RotateTurret(rotation);
    }

    private void Setup(object sender, EventArgs e)
    {
        if (!IsOwner) return;

        Initialize();
    }
}
