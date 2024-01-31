using UnityEngine;

/// <summary>
/// The player representation
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// The player on this device
    /// </summary>
    public static Player Local { get; private set; } 

    public enum PlayerRole { Driver, Gunner };
    public const string DRIVER_ROLE = "Driver";
    public const string GUNNER_ROLE = "Gunner";

    [SerializeField] private string _name;
    [SerializeField] private PlayerRole _role;
    [SerializeField] private ulong _clientId;

    private void Awake()
    {
        SetLocalPlayerRandom();
    }

    /// <summary>
    /// Set a random name for the Player
    /// </summary>
    private void SetLocalPlayerRandom()
    {
        if (Local != null && Local != this)
        {
            Destroy(this);
        }
        else
        {
            Local = this;
        }

        Local.SetName("Player_" + Random.Range(10, 100));
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
}
