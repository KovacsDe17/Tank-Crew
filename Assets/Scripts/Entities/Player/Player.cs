using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

/// <summary>
/// The player representation
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Local { get; private set; } //The player on this device

    public enum PlayerRole { Driver, Gunner };
    public const string DRIVER_ROLE = "Driver";
    public const string GUNNER_ROLE = "Gunner";

    [Header("Main Properties")]
    [SerializeField] private string _name;
    [SerializeField] private PlayerRole _role;
    [SerializeField] private ulong _clientId;

    [Header("Controls")]
    [SerializeField] private Crank _crank;
    [SerializeField] private Lever _leverLeft;
    [SerializeField] private Lever _leverRight;

    [Header("UI")]
    [SerializeField] public GameObject BaseUI;
    [SerializeField] public GameObject DriverUI;
    [SerializeField] public GameObject GunnerUI;
    [SerializeField] public GameObject LoadingScreen;

    private void Awake()
    {
        SetLocalPlayer();
    }

    private void Start()
    {
        Initialize();
    }

    private void SetLocalPlayer()
    {
        if (Local != null && Local != this)
        {
            Destroy(this);
        }
        else
        {
            Local = this;
        }

        Local.SetName("Player_" + UnityEngine.Random.Range(10, 100));
    }

    private void Initialize()
    {
        //Not in use
        //_type = PlayerType.Driver;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public string GetName()
    {
        return _name;
    }

    public void SetClientId(ulong clientId)
    {
        _clientId = clientId;
    }

    public ulong GetClientId()
    {
        return _clientId;
    }

    public PlayerRole GetPlayerRole()
    {
        return _role;
    }

    public static string GetPlayerRoleString(PlayerRole playerRole)
    {
        return (playerRole == PlayerRole.Driver) ? DRIVER_ROLE : GUNNER_ROLE;
    }

    

    /// <summary>
    /// Change the playerType between driver and gunner
    /// </summary>
    public void ChangeRole()
    {
        if (_role == PlayerRole.Driver)
        {
            _role = PlayerRole.Gunner;
        } else
        {
            _role = PlayerRole.Driver;
        }
    }

    public Crank GetCrank()
    {
        return _crank;
    }

    public Lever GetLeverLeft()
    {
        return _leverLeft;
    }
    public Lever GetLeverRight()
    {
        return _leverRight;
    }
}
