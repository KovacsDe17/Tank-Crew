using UnityEngine;

/// <summary>
/// The local player's UI handler.
/// </summary>
public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [SerializeField] private GameObject BaseUI;
    [SerializeField] private GameObject DriverUI;
    [SerializeField] private GameObject GunnerUI;
    [SerializeField] private GameObject LoadingScreen;

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

    public GameObject GetBaseUI()
    {
        return BaseUI;
    }

    public GameObject GetDriverUI()
    {
        return DriverUI;
    }

    public GameObject GetGunnerUI()
    {
        return GunnerUI;
    }

    public GameObject GetLoadingScreen()
    {
        return LoadingScreen;
    }
}
