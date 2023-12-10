using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for creating a lobby through UI.
/// </summary>
public class CreateLobbyMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private TMP_InputField _mapSeedInputField;
    [SerializeField] private TMP_Dropdown _gameModeDropdown;

    private void Start()
    {
        _gameModeDropdown.ClearOptions();
        _gameModeDropdown.AddOptions(Objective.GetObjectiveTypeNames());
    }

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
            ulong.Parse(seed).ToString()
        );
    }

}
