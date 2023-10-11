using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class MultiplayerTankAccess : NetworkBehaviour
{
    public static MultiplayerTankAccess Instance { get; private set; }

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
        Initialize();
    }

    private void Initialize()
    {
        Transform playerTank = PlayerTank.Instance.transform;

        _tankMove = playerTank.GetComponent<TankMove>();
        _turretRotation = playerTank.GetComponentInChildren<TurretRotation>();
    }

    private TankMove _tankMove;
    private TurretRotation _turretRotation;

    [ServerRpc(RequireOwnership = false)]
    public void MoveTankServerRPC(float leverLeftNormalized, float leverRightNormalized)
    {
        _tankMove.MoveByDirections(leverLeftNormalized, leverRightNormalized);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RotateTurretServerRPC(float rotation)
    {
        _turretRotation.RotateTurret(rotation);
    }
}
