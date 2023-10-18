using UnityEngine;
using TMPro;

public class CreateLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private TMP_Dropdown gameModeDropdown;

    public void CreateLobbyByFields()
    {
        LobbyManager.Instance.CreateLobby(
            lobbyNameInputField.text,
            mapDropdown.options[mapDropdown.value].text,
            gameModeDropdown.options[gameModeDropdown.value].text
        );
    }

}
