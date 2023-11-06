using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Join a Lobby by lobby code, using UI
/// </summary>
public class JoinLobbyByCodeUI : MonoBehaviour
{
    [SerializeField] private LobbyManager lobby;
    [SerializeField] private InputField inputField;

    /// <summary>
    /// Join a lobby using an Input Field.
    /// </summary>
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
