using System;
using UnityEngine;

/// <summary>
/// Enable Lobby menus
/// </summary>
public class LobbyMenusEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _joinedLobbyMenu;
    [SerializeField] private GameObject _multiplayerMenu;
    [SerializeField] private GameObject _loadingScreen;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Setup starting information
    /// </summary>
    private void Initialize()
    {
        LobbyManager lobby = LobbyManager.Instance;
        RelayManager relay = RelayManager.Instance;

        lobby.OnJoinedLobby += EnableJoinedLobbyUI;
        lobby.OnGameStartInvoked += OnGameStartInvoked_ChangeUI;

        relay.OnRelayClientStarted += OnClientStarted_ChangeUI;
    }

    /// <summary>
    /// Enable the UI of the Joined Lobby.
    /// </summary>
    private void EnableJoinedLobbyUI(object sender, LobbyManager.LobbyEventArgs lobbyEventArgs)
    {
        _joinedLobbyMenu.transform.localScale = new Vector3(0, 0, 0); //Hide even when enabled
        _joinedLobbyMenu.SetActive(true); //Enable
    }

    /// <summary>
    /// Change UI on Client start.
    /// </summary>
    private void OnClientStarted_ChangeUI(object sender, EventArgs e)
    {
        Debug.Log("Relay Client started, changing UI");

        _joinedLobbyMenu.SetActive(false);
        _multiplayerMenu.SetActive(false);
        Player.Local.BaseUI.SetActive(true); //TODO: swap this

        _loadingScreen.SetActive(false);
    }

    /// <summary>
    /// Change UI on Game start.
    /// </summary>
    private void OnGameStartInvoked_ChangeUI(object sender, EventArgs e)
    {
        Debug.Log("Game Start invoked, changing UI");

        _loadingScreen.SetActive(true);
    }
}
