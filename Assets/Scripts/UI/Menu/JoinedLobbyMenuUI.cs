using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for the UI of the joined lobby.
/// </summary>
public class JoinedLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _lobbyCode;
    [SerializeField] private TextMeshProUGUI _map;
    [SerializeField] private TextMeshProUGUI _gameMode;

    [SerializeField] private List<Button> _hostButtons;

    [SerializeField] private Transform _playerInfoHolder;
    [SerializeField] private GameObject _playerInfoPrefab;

    private void Start()
    {
        SetupEvents();
    }

    /// <summary>
    /// Setup the lobby events.
    /// </summary>
    private void SetupEvents()
    {
        LobbyManager lobby = LobbyManager.Instance;

        lobby.OnJoinedLobbyUpdate += UpdateJoinedLobbyUI;
        lobby.OnLeaveLobby += DisableJoinedLobbyUI;
        lobby.OnKickedFromLobby += DisableJoinedLobbyUI;
    }

    /// <summary>
    /// Disable this GameObject.
    /// </summary>
    private void DisableJoinedLobbyUI(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Update information on the joined lobby UI.
    /// </summary>
    private void UpdateJoinedLobbyUI(object sender, LobbyManager.LobbyEventArgs lobbyEventArgs)
    {
        if (transform.localScale.x == 0) //If it is hidden
            transform.localScale = Vector3.one; //Unhide it

        UpdateLobbyInfo(lobbyEventArgs.Lobby);
        UpdatePlayerInfo(lobbyEventArgs.Lobby);
        UpdateHostButtons(lobbyEventArgs.Lobby);
    }

    /// <summary>
    /// Update the info regarding the joined lobby.
    /// </summary>
    /// <param name="joinedLobby">The joined lobby.</param>
    public void UpdateLobbyInfo(Lobby joinedLobby)
    {
        _lobbyName.SetText(joinedLobby.Name);
        _lobbyCode.SetText("Code: " + joinedLobby.LobbyCode);
        _map.SetText(joinedLobby.Data[LobbyManager.KEY_GAME_MAP].Value);
        _gameMode.SetText(joinedLobby.Data[LobbyManager.KEY_GAME_MODE].Value);
    }

    /// <summary>
    /// Update the list of Players and their information in the joined lobby.
    /// </summary>
    /// <param name="joinedLobby">The joined lobby.</param>
    public void UpdatePlayerInfo(Lobby joinedLobby)
    {
        //Delete existing children
        for (int i = 0; i < _playerInfoHolder.childCount; i++)
        {
            Destroy(_playerInfoHolder.GetChild(i).gameObject);
        }

        //Add a new PlayerInfoPanel child for each player and set their fields
        foreach (Unity.Services.Lobbies.Models.Player player in joinedLobby.Players)
        {
            LobbyPlayerInfo playerInfoPanel = Instantiate(_playerInfoPrefab, _playerInfoHolder).GetComponent<LobbyPlayerInfo>();

            string playerName = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
            string playerRole = player.Data[LobbyManager.KEY_PLAYER_ROLE].Value;

            playerInfoPanel.SetupLobbyPlayerInfo(player);

            bool isHost = IsHostOfLobby(joinedLobby, player.Id);
            bool iamHost = IsHostOfLobby(joinedLobby, AuthenticationService.Instance.PlayerId);

            if (!isHost)
            {
                if (iamHost)
                {
                    playerInfoPanel.kickButton.gameObject.SetActive(true);
                    playerInfoPanel.kickButton.onClick.AddListener(() =>
                    {
                        LobbyManager.Instance.KickPlayer(player);
                    });

                }

                playerInfoPanel.hostImage.gameObject.SetActive(false);
            } else
            {
                playerInfoPanel.kickButton.gameObject.SetActive(false);
                playerInfoPanel.hostImage.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Set the host buttons in the lobby UI visible for the host and hide it for the client
    /// </summary>
    private void UpdateHostButtons(Lobby joinedLobby)
    {
        bool isHost = joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId);

        //If the player is the host, enable buttons
        foreach (Button button in _hostButtons)
        {
            if (button.gameObject.activeInHierarchy == !isHost)
                button.gameObject.SetActive(isHost);
        }
    }

    /// <summary>
    /// Check if the player is the host of the lobby.
    /// </summary>
    /// <param name="lobby">The lobby to search in.</param>
    /// <param name="playerId">The player to check.</param>
    /// <returns>True if the given player is the host of the lobby.</returns>
    private bool IsHostOfLobby(Lobby lobby, string playerId)
    {
        return lobby.HostId.Equals(playerId);
    }
}
