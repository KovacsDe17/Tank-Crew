using UnityEngine;
using TMPro;

/// <summary>
/// This class is responsible for creating a lobby through UI.
/// </summary>
public class CreateLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private TMP_Dropdown gameModeDropdown;

    /// <summary>
    /// Create a lobby by the previously given parameters.
    /// </summary>
    public void CreateLobbyByFields()
    {
        LobbyManager.Instance.CreateLobby(
            lobbyNameInputField.text,
            mapDropdown.options[mapDropdown.value].text,
            gameModeDropdown.options[gameModeDropdown.value].text
        );
    }

}
