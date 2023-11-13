using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public const string KEY_GAME_MODE = "GameMode";
    public const string KEY_GAME_MAP = "Map";
    public const string KEY_START_GAME = "StartGame_RelayCode";
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_ROLE = "PlayerRole";

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler OnGameStartInvoked;
    public event EventHandler OnLeaveLobby;
    public event EventHandler OnKickedFromLobby;

    /// <summary>
    /// Data to pass through Lobby events
    /// </summary>
    public class LobbyEventArgs : EventArgs
    {
        public Lobby Lobby;
    }

    private Lobby _hostLobby; //The lobby that the local player creates (and the host joins)
    private Lobby _joinedLobby; //The lobby that the local player joins in
    private Unity.Services.Lobbies.Models.Player _hostOfJoinedLobby; //Reference for the host of the lobby
    
    private float _heartbeatTimer; //Heartbeat, so the lobby won't be deleted automatically after 30 seconds
    private float _lobbyUpdateTimer; //Timer for auto-updating lobby data
    
    [SerializeField] public List<Button> hostButtons;
    [SerializeField] private GameObject _multiplayerMenu;

    public static LobbyManager Instance { get; private set; }

    #region Main Script Functions
    private void Awake()
    {
        SetupSingletonInstance();
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync(); //Initializing the UnityServices for later purposes

        await SignIn();

        SetupEvents();
    }

    private void SetupEvents()
    {
        OnLeaveLobby += UpdateLobbyInfoOnLeave;
        OnKickedFromLobby += UpdateLobbyInfoOnLeave;
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates(1.5f);
    }

    #endregion

    #region Lobby Related Functions

    /// <summary>
    /// Signs in the player anonymously.
    /// </summary>
    /// <returns>A Task if it was successful.</returns>
    private static async Task SignIn()
    {
        AuthenticationService.Instance.SignedIn += () => //Subscribing to "Signed In" action
        {
            Debug.Log("Signed in with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Signing in anonymously (using a generated ID)

        Debug.Log("Name: " + Player.Local.GetName() + " role: " + ((int)Player.Local.GetPlayerRole()).ToString()); //Printing out the name of the local player and it's role
    }

    /// <summary>
    /// Send a heartbeat to the LobbyService, so the lobby won't get deleted until everyone leaves it.
    /// </summary>
    private async void HandleLobbyHeartbeat()
    {
        if (_hostLobby != null) //If we have a host lobby, then check the timer and set it according to the heartbeat ping
        {
            _heartbeatTimer -= Time.deltaTime;
            if(_heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;  //Currently it's 15 seconds, but can be higher, up to 30 seconds
                _heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);   //Send the heartbeat ping
            }
        }
    }

    /// <summary>
    /// Polls for an update after every X seconds for the joined lobby.
    /// </summary>
    /// <param name="pollAfterTime">Specified time between each poll, given in seconds.</param>
    private async void HandleLobbyPollForUpdates(float pollAfterTime)
    {
        if (_joinedLobby != null)   //If there is a lobby that we are in, set the timer for the poll
        {
            await PollUpdateForJoinedLobbyByTimer(pollAfterTime); //Update Poll Timer and poll according to the time
        }
    }

    /// <summary>
    /// Polls an update for the Joined Lobby after every period of time set as the <paramref name="pollTimer"/>.
    /// </summary>
    /// <param name="pollTimer">The time which an update is being polled after, in seconds.</param>
    /// <returns>A Task if the action was successful.</returns>
    private async Task PollUpdateForJoinedLobbyByTimer(float pollTimer)
    {
        _lobbyUpdateTimer -= Time.deltaTime;
        if (_lobbyUpdateTimer < 0f) //If the timer is up, 
        {
            float lobbyUpdateTimerMax = pollTimer; //Can't go below 1 call per second!
            _lobbyUpdateTimer = lobbyUpdateTimerMax;

            await UpdateJoinedLobby();  //Poll an update
            CheckGameStarted();
        }
    }

    /// <summary>
    /// Check if the host initiated the Game Start.
    /// </summary>
    /// <returns>True if the host started the game.</returns>
    private bool IsGameStartInvoked()
    {
        //If the host initiated the game start, this value changes from "0" to the relay code
        return (_joinedLobby != null && _joinedLobby.Data[KEY_START_GAME] != null && _joinedLobby.Data[KEY_START_GAME].Value != "0");
    }

    /// <summary>
    /// Check if Host started the game, and if so, connect the client to the host with relay, delete Joined Lobby reference and start the game by the GameManager
    /// </summary>
    private async void CheckGameStarted()
    {
        if (IsGameStartInvoked())
        {

            //The host automatically joines the relay, only the client needs to connect
            if (!IsLobbyHost()) //Not the host = the client
            {
                Debug.Log("Invoking Game Start...");
                //OnGameStartInvoked?.Invoke(this, EventArgs.Empty);

                await RelayManager.Instance.JoinRelay(_joinedLobby.Data[KEY_START_GAME].Value);
            }

            _joinedLobby = null; //Delete the joined lobby reference

            //GameManager.Instance.StartGame();   //Start the game
        }
    }

    /// <summary>
    /// Polls an update for the joined lobby.
    /// </summary>
    /// <returns>A Task if the operation was successful.</returns>
    private async Task UpdateJoinedLobby()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);

            _joinedLobby = lobby;

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { Lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            OnKickedFromLobby?.Invoke(this, EventArgs.Empty);

            Debug.Log(e);
        }
    }

    /// <summary>
    /// Create a lobby.
    /// </summary>
    /// <param name="lobbyName">The name of the lobby.</param>
    /// <param name="gameMode">The game mode in which the game starts in.</param>
    /// <param name="map">The map used in the game.</param>
    public async void CreateLobby(string lobbyName, string gameMode, string map)
    {
        try
        {
            int maxPlayers = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                //Making the lobby private, so it won't be listed
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode)},
                    { KEY_GAME_MAP, new DataObject(DataObject.VisibilityOptions.Public, map)},
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };

            if (lobbyName.Equals(""))
                lobbyName = Player.Local.GetName() + "\'s lobby";

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            _hostLobby = lobby;
            _joinedLobby = lobby;
            _hostOfJoinedLobby = GetHostOfLobby(lobby);

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { Lobby = _joinedLobby });

            Debug.Log("Lobby created with name " + lobby.Name + "(ID: " + lobby.Id + ") , and has " + lobby.MaxPlayers + " players max. The lobby code is \"" + lobby.LobbyCode + "\"");
            
            PrintPlayers(_hostLobby);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Join an existing lobby.
    /// </summary>
    /// <param name="lobbyCode">The Lobby Code which is used to connect to a lobby.</param>
    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            //Join a lobby by LobbyCode
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            _joinedLobby = lobby;
            _hostOfJoinedLobby = GetHostOfLobby(lobby);

            CheckRoles();

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { Lobby = _joinedLobby });

            Debug.Log("Joined lobby by code \"" + lobbyCode + "\"!");


            PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Join a lobby by LobbyCode using an inputField.
    /// </summary>
    /// <param name="inputField">The inputField which is used for LobbyCode input.</param>
    public void JoinLobby(TMPro.TMP_InputField inputField)
    {
        JoinLobby(inputField.text);
        inputField.SetTextWithoutNotify("");
    }

    /// <summary>
    /// Retrieve a Player model for the lobby based on the Local Player.
    /// </summary>
    /// <returns>A Player model with name and role.</returns>
    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, Player.Local.GetName())},
                        { KEY_PLAYER_ROLE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int) Player.Local.GetPlayerRole()).ToString())}
                    }
        };
    }

    /// <summary> 
    ///Update the Game Mode of the host lobby to the specified GameMode.
    /// </summary>
    /// <param name="gameMode">The Game Mode to switch to.</param>
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>
                {
                    //Only the specified parameters get updated, the others stay unmodified 
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode)} //Here we only update the GameMode (so the Map stays the same)
                }
            });

            _hostLobby = lobby;
            _joinedLobby = lobby;

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { Lobby = lobby });

            PrintPlayers(_hostLobby);

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Update a single piece of data of the player in the lobby.
    /// </summary>
    /// <param name="key">The key of the data to update.</param>
    /// <param name="value">The value of the specified key.</param>
    /// <returns></returns>
    private async Task UpdatePlayerData(string key, string value)
    {
        try
        {
            //Update LobbyService Player name
            Lobby lobby = await Lobbies.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {   
                    //Only the specified parameters get updated, the others stay unmodified
                    { key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value)}
                }
            });

            _joinedLobby = lobby;

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { Lobby = lobby });

        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Update the Player's data in the lobby.
    /// </summary>
    /// <param name="player">The player with the new data.</param>
    private async void UpdatePlayerData(Player player)
    {
        try
        {
            //Update LobbyService Player name
            Lobby lobby = await Lobbies.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {   
                    //Only the specified parameters get updated, the others stay unmodified
                    { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, player.GetName())},
                    { KEY_PLAYER_ROLE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)player.GetPlayerRole()).ToString())}
                }
            });

            _joinedLobby = lobby;
            _hostOfJoinedLobby = GetHostOfLobby(lobby);

            OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { Lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Leave the Joined Lobby. If the Host leaves, the other joined player becomes the host. 
    /// </summary>
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            OnLeaveLobby?.Invoke(this, EventArgs.Empty);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    /// <summary>
    /// Kick the non-host player from the lobby.
    /// </summary>
    public async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, _joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    /// <summary>
    /// Kick a specified player from the lobby.
    /// </summary>
    /// <param name="player">The player to be kicked out from the lobby.</param>
    public async void KickPlayer(Unity.Services.Lobbies.Models.Player player)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, player.Id);

            OnKickedFromLobby?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Leave the currently joined lobby and set the UI accordingly.
    /// </summary>
    private void UpdateLobbyInfoOnLeave(object sender, EventArgs e)
    {
        _joinedLobby = null;
        _hostLobby = null;
        _hostOfJoinedLobby = null;
    }

    /// <summary>
    /// Start the game from the lobby. Only works for the host.
    /// </summary>
    public async void InvokeGameStart()
    {
        OnGameStartInvoked?.Invoke(this, EventArgs.Empty);

        if (IsLobbyHost())
        {
            try
            {
                Debug.Log("Starting Game");

                string relayCode = await RelayManager.Instance.CreateRelay();

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id,
                    new UpdateLobbyOptions()
                    {
                        Data = new Dictionary<string, DataObject> {
                            { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                        }
                    }
                    );

                _joinedLobby = lobby;

            } catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
    }

    #endregion

    #region Helper Functions

    /// <summary>
    /// Sets the singleton instance. Deletes this object if there is already another of this type.
    /// </summary>
    private void SetupSingletonInstance()
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


    private void CheckRoles()
    {
        if (_hostOfJoinedLobby.Id == AuthenticationService.Instance.PlayerId) return; //Only apply for the non-host player!

        //If the roles are the same with the host, change this players role
        int playerRoleOfHost;
        int.TryParse(_joinedLobby.Players[0].Data[KEY_PLAYER_ROLE].Value, out playerRoleOfHost);
        if (Player.Local.GetPlayerRole() == (Player.PlayerRole)playerRoleOfHost)
        {
            ChangeRole();
        }
    }

    public void ChangeRole()
    {
        Player.Local.ChangeRole(); //Change locally
        UpdatePlayerData(Player.Local); //Change in the lobby
    }

    public void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby \"" + lobby.Name + "\" (GameMode: \"" + lobby.Data[KEY_GAME_MODE].Value + "\", Map: \"" + lobby.Data[KEY_GAME_MAP].Value + "\"):");

        //Not the same Player class as in the project!
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            bool isHost = lobby.HostId == player.Id;

            //Host is marked with an asterisk (*)
            Debug.Log(player.Id + " " + player.Data[KEY_PLAYER_NAME].Value + (isHost?"*":""));
        }
    }

    #endregion

    #region Getters and Setters

    public Lobby GetJoinedLobby()
    {
        return _joinedLobby;
    }

    public Lobby GetHostLobby()
    {
        return _hostLobby;
    }
    private bool IsLobbyHost()
    {
        if (_joinedLobby != null && _joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId))
        {
            return true;
        }

        return false;
    }
    private Unity.Services.Lobbies.Models.Player GetHostOfLobby(Lobby lobby)
    {
        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            if (player.Id.Equals(lobby.HostId)) //If the hostId and the player's ID are the same, that player is the host
                return player;
        }

        return lobby.Players[0]; //By default, return the first player (In most cases that player is the host)
    }

    #endregion

    #region Unused but Later-Might-Use

    /// <summary>
    /// Print players of the joined lobby
    /// </summary>
    public void PrintPlayers()
    {
        PrintPlayers(_joinedLobby);
    }

    public async void UpdatePlayerNameRandom()
    {
        string randomName = "RandomPlayer_" + UnityEngine.Random.Range(10, 99);

        await UpdatePlayerData(KEY_PLAYER_NAME, randomName);
    }

    public async void MigrateLobbyHost()
    {
        try
        {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions
            {
                //TODO: reference the other player using a function like GetOtherPlayer()
                HostId = _joinedLobby.Players[1].Id
            });

            PrintPlayers(_hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //TODO: Do we even need GameMode?
    public void SwitchLobbyGameMode()
    {
        //If we don't have a host lobby yet, just return
        if (_hostLobby == null) return;

        //Switch between TDM and CTF modes
        string gameMode = _hostLobby.Data[KEY_GAME_MODE].Value == "Team Deathmatch" ? "Capture The Flag" : "Team Deathmatch";

        UpdateLobbyGameMode(gameMode);
    }

    #endregion
}
