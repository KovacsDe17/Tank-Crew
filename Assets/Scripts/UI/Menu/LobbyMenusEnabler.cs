using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyMenusEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _joinedLobbyMenu;
    [SerializeField] private GameObject _multiplayerMenu;
    [SerializeField] private GameObject _loadingScreen;

    void Start()
    {
        LobbyManager lobby = LobbyManager.Instance;
        RelayManager relay = RelayManager.Instance;

        lobby.OnJoinedLobby += EnableJoinedLobbyUI;
        lobby.OnGameStartInvoked += OnGameStartInvoked_ChangeUI;

        relay.OnRelayClientStarted += OnClientStarted_ChangeUI;
    }

    private void EnableJoinedLobbyUI(object sender, LobbyManager.LobbyEventArgs lobbyEventArgs)
    {
        _joinedLobbyMenu.transform.localScale = new Vector3(0, 0, 0); //Hide even when enabled
        _joinedLobbyMenu.SetActive(true); //Enable
    }

    private void OnClientStarted_ChangeUI(object sender, EventArgs e)
    {
        Debug.Log("Relay Client started, changing UI");

        _joinedLobbyMenu.SetActive(false);
        _multiplayerMenu.SetActive(false);
        Player.Local.BaseUI.SetActive(true); //TODO: swap this

        _loadingScreen.SetActive(false);
    }

    private void OnGameStartInvoked_ChangeUI(object sender, EventArgs e)
    {
        Debug.Log("Game Start invoked, changing UI");

        _loadingScreen.SetActive(true);
    }
}
