using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    private bool _isDriver; //Wether the player is a Driver or a Gunner

    private Lever _leverLeft, _leverRight; //Lever controls from Driver UI
    private Crank _crank; //Crank control from Gunner UI

    public override void OnNetworkSpawn()
    {
        Setup();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (MultiplayerTankAccess.Instance == null) return;

        if (_isDriver)
        {
            if (_leverLeft == null || _leverRight == null) return;

            if(_leverLeft.HasMoved() || _leverRight.HasMoved())
                MultiplayerTankAccess.Instance.MoveTankServerRPC(_leverLeft.GetNormalizedValue(), _leverRight.GetNormalizedValue());
        }
        else
        {
            if (_crank == null) return;

            if(_crank.HasMoved())
                MultiplayerTankAccess.Instance.RotateTurretServerRPC(_crank.GetRotation());
        }
    }

    private void Initialize()
    {
        Player.Local.SetClientId(OwnerClientId); //Set local player
        _isDriver = Player.Local.GetPlayerRole() == Player.PlayerRole.Driver;

        _leverLeft = PlayerControls.Instance.GetLeverLeft();
        _leverRight = PlayerControls.Instance.GetLeverRight();

        _crank = PlayerControls.Instance.GetCrank();

        Debug.Log("Control for " + OwnerClientId + " is owned by " + Player.Local.GetName());
    }


    public void SetupTankControlUI()
    {
        Debug.Log(GetType().Name + " - Player " + Player.Local.GetName() + " is " + (_isDriver?"driver":"gunner"));

        PlayerUI.Instance.GetDriverUI().SetActive(_isDriver);
        PlayerUI.Instance.GetGunnerUI().SetActive(!_isDriver);
    }

    private void Setup()
    {
        if (!IsOwner) return;

        Initialize();
        SetupTankControlUI();
        SetupCamera();
    }

    private void SetupCamera()
    {
        GameObject camera = Camera.main.gameObject;
        FollowPlayer followPlayer = camera.GetComponent<FollowPlayer>();

        followPlayer.SetPlayerTransform();
    }
}
