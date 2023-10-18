using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

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

    private void SetupEvents()
    {
        LobbyManager lobby = LobbyManager.Instance;

        lobby.OnJoinedLobbyUpdate += UpdateJoinedLobbyUI;
        lobby.OnLeaveLobby += DisableJoinedLobbyUI;
        lobby.OnKickedFromLobby += DisableJoinedLobbyUI;
    }

    private void DisableJoinedLobbyUI(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void UpdateJoinedLobbyUI(object sender, LobbyManager.LobbyEventArgs lobbyEventArgs)
    {
        if (transform.localScale.x == 0) //If it is hidden
            transform.localScale = Vector3.one; //Unhide it

        UpdateLobbyInfo(lobbyEventArgs.Lobby);
        UpdatePlayerInfo(lobbyEventArgs.Lobby);
        UpdateHostButtons(lobbyEventArgs.Lobby);
    }

    public void UpdateLobbyInfo(Lobby joinedLobby)
    {
        _lobbyName.SetText(joinedLobby.Name);
        _lobbyCode.SetText("Code: " + joinedLobby.LobbyCode);
        _map.SetText(joinedLobby.Data[LobbyManager.KEY_GAME_MAP].Value);
        _gameMode.SetText(joinedLobby.Data[LobbyManager.KEY_GAME_MODE].Value);
    }

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

    private bool IsHostOfLobby(Lobby lobby, string playerId)
    {
        return lobby.HostId.Equals(playerId);
    }
}
