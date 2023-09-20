using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyByCodeUI : MonoBehaviour
{
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private InputField inputField;

    public void JoinLobbyByInput()
    {
        try
        {
            lobby.JoinLobby(inputField.text);
        }
        catch (LobbyServiceException e)
        {
            //TODO: Create error panel and send output there
            Debug.Log(e);
        }
    }
}
