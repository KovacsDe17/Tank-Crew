using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private Lobby hostLobby; //The lobby that the local player creates
    private Lobby joinedLobby; //The lobby that the local player joins in
    
    private float heartbeatTimer; //Heartbeat, so the lobby won't be deleted automatically after 30 seconds
    private float lobbyUpdateTimer; //Timer for auto-updating lobby data
    
    private string playerName; //Name of the local player
    private string playerRole; //Role of the local player
    
    [SerializeField] public List<Button> hostButtons;
    [SerializeField] JoinedLobbyMenuUI joinedLobbyMenu;

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

        playerName = "TestPlayer_" + UnityEngine.Random.Range(10, 99);
        playerRole = "TestRole";
        Debug.Log(playerName);
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
                float lobbyUpdateTimerMax = 1.1f; //Can't go below 1 call per second!
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
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)},
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public, map)}
                }
            };

            if (lobbyName.Equals(""))
                lobbyName = playerName + "\'s lobby";

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = hostLobby;
            
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
                Debug.Log(lobby.Name + " (" + lobby.Players.Count + "/" + lobby.MaxPlayers + ") - GameMode: \"" + lobby.Data["GameMode"].Value + "\"");
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

            SetupJoinedLobbyMenu();

            Debug.Log("Joined lobby by code \"" + lobbyCode + "\"!");

            PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void JoinLobby(TMPro.TMP_InputField inputField)
    {
        //TODO: make InputField uppercase in UI
        //Join a lobby by LobbyCode using an inputField
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
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)},
                        { "PlayerRole", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerRole)}
                    }
        };
    }

    public void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby \"" + lobby.Name + "\" (GameMode: \"" + lobby.Data["GameMode"].Value + "\", Map: \"" + lobby.Data["Map"].Value + "\"):");

        //Not the same Player class as in the project!
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            bool isHost = lobby.HostId == player.Id;

            //Host is marked with an asterisk (*)
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value + (isHost?"*":""));
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
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)} //Here we only update the GameMode (so the Map stays the same)
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
        string gameMode = hostLobby.Data["GameMode"].Value == "Team Deathmatch" ? "Capture The Flag" : "Team Deathmatch";

        UpdateLobbyGameMode(gameMode);
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName; //Update local variable

            //Update LobbyService Player name
            await Lobbies.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {

                Data = new Dictionary<string, PlayerDataObject>
                {   
                    //Only the specified parameters get updated, the others stay unmodified
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }
            });


        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void UpdatePlayerNameRandom()
    {
        string randomName = "RandomPlayer_" + UnityEngine.Random.Range(10, 99);

        UpdatePlayerName(randomName);
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

    //TODO: Make this an RPC, so it updates for the new host?
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
    }
}
