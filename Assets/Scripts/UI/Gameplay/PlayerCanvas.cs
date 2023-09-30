using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] GameObject GunnerUI;
    [SerializeField] GameObject DriverUI;

    private void Awake()
    {
        ChangeUIBasedOnType();
    }

    private void ChangeUIBasedOnType()
    {
        bool isGunner = Player.Local.GetPlayerRole() == Player.PlayerRole.Gunner;

        GunnerUI.SetActive(isGunner);
        DriverUI.SetActive(isGunner);
    }
}
