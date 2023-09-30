using UnityEngine;
using TMPro;

public class CreateLobbyMenuUI : MonoBehaviour
{
    [SerializeField] TMP_InputField lobbyNameInputField;
    [SerializeField] TMP_Dropdown mapDropdown;
    [SerializeField] TMP_Dropdown gameModeDropdown;

    public void CreateLobbyByFields()
    {
        LobbyManager.Instance.CreateLobby(
            lobbyNameInputField.text,
            mapDropdown.options[mapDropdown.value].text,
            gameModeDropdown.options[gameModeDropdown.value].text
        );
    }

}
