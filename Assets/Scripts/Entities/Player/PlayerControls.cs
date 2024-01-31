using UnityEngine;

/// <summary>
/// The local player's controls.
/// </summary>
public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance { get; private set; }

    [SerializeField] private Crank _crank;
    [SerializeField] private Lever _leverLeft;
    [SerializeField] private Lever _leverRight;

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
