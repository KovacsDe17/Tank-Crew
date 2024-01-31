using UnityEngine;

public class Singleplayer : MonoBehaviour
{
    [SerializeField] GameObject driverUI;
    [SerializeField] GameObject gunnerUI;

    private void Start()
    {
        SetUI();
    }

    public void SetUI()
    {
        bool isDriver = IsDriver();
        driverUI.SetActive(isDriver);
        gunnerUI.SetActive(!isDriver);
    }

    private bool IsDriver()
    {
        return Player.Local.GetPlayerRole() == Player.PlayerRole.Driver;
    }
}
