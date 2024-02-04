using System;
using UnityEngine;

/// <summary>
/// Manages UI elements.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private LoadingIcon _loadingIcon;

    [SerializeField] private GameObject _joinedLobbyMenu;
    [SerializeField] private GameObject _multiplayerMenu;
    [SerializeField] private GameObject _loadingScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SubscribeToEvents();
        TryGetUIElements();
    }

    private void TryGetUIElements()
    {
        try
        {
            _loadingIcon = FindObjectOfType<LoadingIcon>(true);
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning("Loading Icon not found!\n" + e.ToString());
        }
    }

    /// <summary>
    /// Subscribe to LobbyManager and RelayManager events.
    /// </summary>
    private void SubscribeToEvents()
    {
        LobbyManager lobby = LobbyManager.Instance;
        RelayManager relay = RelayManager.Instance;

        lobby.OnJoinedLobby += EnableJoinedLobbyUI;

        GameManager.Instance.OnSetupComplete += OnClientStarted_ChangeUI;

        GameplaySync.Instance.NumberOfPlayersInGame.OnValueChanged += (p, n) => Debug.Log("Number of players in game changed from " + p + " to " + n);
        GameplaySync.Instance.NumberOfPlayersInLobby.OnValueChanged += (p, n) => Debug.Log("Number of players in lobby changed from " + p + " to " + n);
    }

    public void ShowLoadingIcon()
    {
        _loadingIcon.gameObject.SetActive(true);
    }

    public void HideLoadingIcon()
    {
        _loadingIcon.gameObject.SetActive(false);
    }

    public bool LoadingIconIsActive()
    {
        return _loadingIcon.gameObject.activeSelf;
    }


    /// <summary>
    /// Enable the UI of the Joined Lobby.
    /// </summary>
    private void EnableJoinedLobbyUI(object sender, LobbyManager.LobbyEventArgs lobbyEventArgs)
    {
        _joinedLobbyMenu.transform.localScale = new Vector3(0, 0, 0); //Hide even when enabled
        _joinedLobbyMenu.SetActive(true); //Enable
    }

    /// <summary>
    /// Change UI on Client start.
    /// </summary>
    private void OnClientStarted_ChangeUI(object sender, EventArgs e)
    {
        _joinedLobbyMenu.SetActive(false);
        _multiplayerMenu.SetActive(false);
        PlayerUI.Instance.GetBaseUI().SetActive(true); //TODO: swap this

        Debug.Log("With " + Player.Local.GetName() + " connected, there are " + GameplaySync.Instance.GetPlayerCount() + " players on the server.");
    }
}
