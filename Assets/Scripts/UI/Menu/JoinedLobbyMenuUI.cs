using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinedLobbyMenuUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyName;
    [SerializeField] TextMeshProUGUI lobbyCode;
    [SerializeField] TextMeshProUGUI map;
    [SerializeField] TextMeshProUGUI gameMode;

    [SerializeField] Transform playerInfoHolder;
    [SerializeField] GameObject playerInfoPrefab;

    public void UpdateLobbyInfo(Lobby joinedLobby)
    {
        lobbyName.SetText(joinedLobby.Name);
        lobbyCode.SetText("Code: " + joinedLobby.LobbyCode);
        map.SetText(joinedLobby.Data["Map"].Value);
        gameMode.SetText(joinedLobby.Data["GameMode"].Value);
    }

    public void UpdatePlayerInfo(Lobby joinedLobby)
    {
        //Delete existing children
        for (int i = 0; i < playerInfoHolder.childCount; i++)
        {
            Destroy(playerInfoHolder.GetChild(i).gameObject);
        }

        //Add a new PlayerInfoPanel child for each player and set their fields
        foreach (Unity.Services.Lobbies.Models.Player player in joinedLobby.Players)
        {
            LobbyPlayerInfo playerInfoPanel = Instantiate(playerInfoPrefab, playerInfoHolder).GetComponent<LobbyPlayerInfo>();

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

    private bool IsHostOfLobby(Lobby lobby, string playerId)
    {
        return lobby.HostId.Equals(playerId);
    }
}
