using UnityEngine;

/// <summary>
/// The player representation
/// </summary>
public class Player : MonoBehaviour
{
    public enum PlayerType { Driver, Gunner};

    [SerializeField]
    private PlayerType _type;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        //Not in use
        //_type = PlayerType.Driver;
    }

    public PlayerType GetPlayerType()
    {
        return _type;
    }

    /// <summary>
    /// Change the playerType between driver and gunner
    /// </summary>
    public void ChangeType()
    {
        if (_type == PlayerType.Driver)
            _type = PlayerType.Gunner;
        else
            _type = PlayerType.Driver;
    }
}
