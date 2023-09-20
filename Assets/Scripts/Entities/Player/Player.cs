using UnityEngine;

/// <summary>
/// The player representation
/// </summary>
public class Player : MonoBehaviour
{
    public static bool IsInitialized = false;
    public enum PlayerType { Driver, Gunner};

    [SerializeField] private string _name;
    [SerializeField] private PlayerType _type;
    [SerializeField] private Crank _crank;
    [SerializeField] private Levers _levers;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        //Not in use
        //_type = PlayerType.Driver;
        IsInitialized = true;
    }

    public void setName(string name)
    {
        _name = name;
    }

    public string getName()
    {
        return _name;
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

    public Crank getCrank()
    {
        return _crank;
    }

    public Levers getLevers()
    {
        return _levers;
    }

    [System.Serializable]
    public struct Levers
    {
        [SerializeField] Lever left;
        [SerializeField] Lever right;
    }
}
