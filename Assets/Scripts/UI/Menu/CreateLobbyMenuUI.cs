using UnityEngine;
using TMPro;

/// <summary>
/// This class is responsible for creating a lobby through UI.
/// </summary>
public class CreateLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private TMP_InputField _mapSeedInputField;
    [SerializeField] private TMP_Dropdown _gameModeDropdown;

    /// <summary>
    /// Create a lobby by the previously given parameters.
    /// </summary>
    public void CreateLobbyByFields()
    {
        string seed = _mapSeedInputField.text;

        if (_mapSeedInputField.text.Equals(""))
            seed = "0";

        LobbyManager.Instance.CreateLobby(
            _lobbyNameInputField.text,
            _gameModeDropdown.options[_gameModeDropdown.value].text,
            int.Parse(seed).ToString()
        );
    }

}
