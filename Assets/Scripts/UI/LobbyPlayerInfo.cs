using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;

public class LobbyPlayerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerRoleText;
    [SerializeField] public Image hostImage;
    [SerializeField] public Button kickButton;

    public void SetupLobbyPlayerInfo(Unity.Services.Lobbies.Models.Player player)
    { 
        
        Lobby joinedLobby = LobbyManager.Instance.GetJoinedLobby();

        /*
        //If the player is the host, hide the kick button
        if (IsHostOfLobby(joinedLobby, player))
        {
            kickButton.gameObject.SetActive(false);
        }
        //If the player is not the host, hide the host image
        else
        {
            hostImage.gameObject.SetActive(false);
        }

        //If not the local player is the host, disable the kick buttons
        if (!joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId))
        {
            kickButton.gameObject.SetActive(true);
            kickButton.onClick.AddListener(() =>
            {
                LobbyManager.Instance.KickPlayer(player);
            });
        }
        else
        {
            kickButton.gameObject.SetActive(false);
        }
        */

        playerNameText.SetText(player.Data["PlayerName"].Value);    //Set PlayerName text to TextMeshPro
        playerRoleText.SetText(player.Data["PlayerRole"].Value);    //Set PlayerRole text to TextMeshPro
    }

    private bool IsHostOfLobby(Lobby lobby, Unity.Services.Lobbies.Models.Player player)
    {
        return player.Id.Equals(lobby.HostId);
    }
}
