using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
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

    private Lobby hostLobby; //The lobby that the local player creates
    private Lobby joinedLobby; //The lobby that the local player joins in
    private Unity.Services.Lobbies.Models.Player hostOfJoinedLobby; //Reference for the host of the lobby
    
    private float heartbeatTimer; //Heartbeat, so the lobby won't be deleted automatically after 30 seconds
    private int heartbeatCounter; //Number of heartbeats before deleting a lobby
    private int heartbeatMaxCount; //Max number of heartbeats before deleting a lobby
    private float lobbyUpdateTimer; //Timer for auto-updating lobby data
    
    [SerializeField] public List<Button> hostButtons;
    [SerializeField] JoinedLobbyMenuUI joinedLobbyMenu;
    [SerializeField] GameObject multiplayerMenu;

    public static LobbyManager Instance { get; private set; }
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

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Name: " + Player.Local.GetName() + " role: " + ((int)Player.Local.GetPlayerRole()).ToString());
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {
        //TODO: specify max time (or max heartbeat count) to be alive or delete the whole heartbeat timer
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.5f; //Can't go below 1 call per second!
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                try
                {
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                
                    joinedLobbyMenu.UpdatePlayerInfo(joinedLobby);
                    UpdateHostButtons();

                    joinedLobby = lobby;

                }catch(LobbyServiceException e)
                {
                    Debug.Log(e);
                    joinedLobby = null;
                }
            }

            CheckRoles();

            //If the host initiated the game start, this value changes from "0" to the relay code
            if (joinedLobby.Data[KEY_START_GAME] != null && joinedLobby.Data[KEY_START_GAME].Value != "0")
            {
                //The host is automatically joined to the relay, there's no need to invoke joining for the host
                if (!IsLobbyHost())
                {
                    RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                }   
                
                joinedLobby = null;

                if(multiplayerMenu.activeInHierarchy)
                    multiplayerMenu.SetActive(false);

                //TODO: Run only once for the host (now it runs constantly!)

                if (!Player.Local.LoadingScreen.activeInHierarchy)
                {
                    Player.Local.LoadingScreen.SetActive(true);
                    Debug.Log(GetType().Name + " - LoadingScreen set to " + (Player.Local.LoadingScreen.activeInHierarchy ? "active" : "inactive"));
                }
            }
        }
        else
        {
            //If there is no joined lobby and the joined lobby menu is active, hide it
            if(joinedLobbyMenu.gameObject.activeInHierarchy)
                joinedLobbyMenu.gameObject.SetActive(false);
        }
    }

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

            hostLobby = lobby;
            joinedLobby = lobby;
            hostOfJoinedLobby = GetHostOfLobby(lobby);
            
            SetupJoinedLobbyMenu();

            Debug.Log("Lobby created with name " + lobby.Name + "(ID: " + lobby.Id + ") , and has " + lobby.MaxPlayers + " players max. The lobby code is \"" + lobby.LobbyCode + "\"");
            
            PrintPlayers(hostLobby);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void CreateLobby()
    {
        CreateLobby("MyLobby", "Team Deathmatch", "de_dust2");
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25, //List only 25 lobbies
                Filters = new List<QueryFilter> { //Add filters
                    //Filter for available slots: Only show the ones with more than (Greater Than) 0 available slots
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder> { //Set results in order
                    //true=ascending, false=descending; then which field to be ordered by (this one lists the oldest created lobby first)
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach(Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " (" + lobby.Players.Count + "/" + lobby.MaxPlayers + ") - GameMode: \"" + lobby.Data[KEY_GAME_MODE].Value + "\"");
            }
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            //List lobbies
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            //Joint the very first lobby (by ID)
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            joinedLobby = lobby;
            hostOfJoinedLobby = GetHostOfLobby(lobby);

            SetupJoinedLobbyMenu();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

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
            joinedLobby = lobby;
            hostOfJoinedLobby = GetHostOfLobby(lobby);

            CheckRoles();

            SetupJoinedLobbyMenu();

            Debug.Log("Joined lobby by code \"" + lobbyCode + "\"!");


            PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void CheckRoles()
    {
        if (hostOfJoinedLobby.Id == AuthenticationService.Instance.PlayerId) return; //Only apply for the non-host player!

        //If the roles are the same with the host, change this players role
        int playerRoleOfHost;
        int.TryParse(joinedLobby.Players[0].Data[KEY_PLAYER_ROLE].Value, out playerRoleOfHost);
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

    /// <summary>
    /// Join a lobby by LobbyCode using an inputField
    /// </summary>
    /// <param name="inputField">The inputField which is used for LobbyCode input</param>
    public void JoinLobby(TMPro.TMP_InputField inputField)
    {
        JoinLobby(inputField.text);
        inputField.SetTextWithoutNotify("");
    }

    public async void QuickJoinLobby()
    {
        try
        {
            //Join a random lobby
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            joinedLobby = lobby;

            SetupJoinedLobbyMenu();

            Debug.Log("Quick Join was successful! Joined lobby \"" + lobby.Name + "\".");
        } catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

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

    //Print players of the joined lobby
    public void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    //Update the GameMode of the host lobby to the specified GameMode
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>
                {
                    //Only the specified parameters get updated, the others stay unmodified 
                    { KEY_GAME_MODE, new DataObject(DataObject.VisibilityOptions.Public, gameMode)} //Here we only update the GameMode (so the Map stays the same)
                }
            });

            PrintPlayers(hostLobby);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void SwitchLobbyGameMode()
    {
        //If we don't have a host lobby yet, just return
        if (hostLobby == null) return;

        //Switch between TDM and CTF modes
        string gameMode = hostLobby.Data[KEY_GAME_MODE].Value == "Team Deathmatch" ? "Capture The Flag" : "Team Deathmatch";

        UpdateLobbyGameMode(gameMode);
    }

    private async Task UpdatePlayerData(string key, string data)
    {
        try
        {
            //Update LobbyService Player name
            Lobby lobby = await Lobbies.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {   
                    //Only the specified parameters get updated, the others stay unmodified
                    { key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, data)}
                }
            });

            joinedLobby = lobby;
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void UpdatePlayerData(Player player)
    {
        try
        {
            //Update LobbyService Player name
            Lobby lobby = await Lobbies.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {   
                    //Only the specified parameters get updated, the others stay unmodified
                    { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, player.GetName())},
                    { KEY_PLAYER_ROLE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, ((int)player.GetPlayerRole()).ToString())}
                }
            });

            joinedLobby = lobby;
            hostOfJoinedLobby = GetHostOfLobby(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerNameRandom()
    {
        string randomName = "RandomPlayer_" + UnityEngine.Random.Range(10, 99);

        await UpdatePlayerData(KEY_PLAYER_NAME, randomName);
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            UpdateHostButtons();
            LeaveJoinedLobby();
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(Unity.Services.Lobbies.Models.Player player)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, player.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            await Lobbies.Instance.DeleteLobbyAsync(hostLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void UpdateHostButtons()
    {
        bool isHost = joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId);

        //If the player is the host, enable buttons
        foreach (Button button in hostButtons)
        {
            if(button.gameObject.activeInHierarchy == !isHost)
                button.gameObject.SetActive(isHost);
        }
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    public Lobby GetHostLobby()
    {
        return hostLobby;
    }
    private bool IsLobbyHost()
    {
        if(joinedLobby != null && joinedLobby.HostId.Equals(AuthenticationService.Instance.PlayerId))
        {
            return true;
        }

        return false;
    }
    private Unity.Services.Lobbies.Models.Player GetHostOfLobby(Lobby lobby)
    {
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            if (player.Id.Equals(lobby.HostId)) //If the hostId and the player's ID are the same, that player is the host
                return player;
        }

        return lobby.Players[0]; //By default, return the first player (In most cases that player is the host)
    }

    private void SetupJoinedLobbyMenu()
    {
        joinedLobbyMenu.UpdateLobbyInfo(joinedLobby);
        joinedLobbyMenu.UpdatePlayerInfo(joinedLobby);
        joinedLobbyMenu.gameObject.SetActive(true);
    }

    private void LeaveJoinedLobby()
    {
        joinedLobbyMenu.gameObject.SetActive(true);
        joinedLobby = null;
        hostLobby = null;
        hostOfJoinedLobby = null;
    }

    public async void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                Debug.Log("Starting Game");

                string relayCode = await RelayManager.Instance.CreateRelay();

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id,
                    new UpdateLobbyOptions()
                    {
                        Data = new Dictionary<string, DataObject> {
                            { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                        }
                    }
                    );

                joinedLobby = lobby;
            } catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
