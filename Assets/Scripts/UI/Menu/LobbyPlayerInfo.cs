using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LobbyModels = Unity.Services.Lobbies.Models;

/// <summary>
/// Holds Player's information in texts.
/// </summary>
public class LobbyPlayerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerRoleText;
    [SerializeField] public Image hostImage;
    [SerializeField] public Button kickButton;

    /// <summary>
    /// Update the Players' information in the lobby.
    /// </summary>
    /// <param name="player"></param>
    public void SetupLobbyPlayerInfo(LobbyModels.Player player)
    {
        string playerName = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
        playerNameText.SetText(playerName); //Set PlayerName text to TextMeshPro

        int playerRoleInt;
        int.TryParse(player.Data[LobbyManager.KEY_PLAYER_ROLE].Value, out playerRoleInt);
        string playerRole = Player.GetPlayerRoleString((Player.PlayerRole) playerRoleInt); //Get PlayerRole name based on PlayerRole
        playerRoleText.SetText(playerRole); //Set PlayerRole text to TextMeshPro
    }
}
