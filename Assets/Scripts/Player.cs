using UnityEngine;

/// <summary>
/// Class for the player representation
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
        _type = PlayerType.Driver;
    }

    public PlayerType GetType()
    {
        return _type;
    }

    public void ChangeType()
    {
        if (_type == PlayerType.Driver)
            _type = PlayerType.Gunner;
        else
            _type = PlayerType.Driver;
    }
}
