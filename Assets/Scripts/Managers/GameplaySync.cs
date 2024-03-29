using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Syncronizes elements of the game for the multiplayer.
/// </summary>
public class GameplaySync : NetworkBehaviour
{
    public static GameplaySync Instance { get; private set; }

    public NetworkVariable<bool> IsDead = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> HasWonGame = new NetworkVariable<bool>(false);
    public NetworkVariable<float> TimeElapsed = new NetworkVariable<float>(0);
    public NetworkVariable<int> EnemiesDestroyed = new NetworkVariable<int>(0);
    public NetworkVariable<int> ShotsFired = new NetworkVariable<int>(0);
    public NetworkVariable<int> NumberOfPlayersInLobby = new NetworkVariable<int>(0);
    public NetworkVariable<int> NumberOfPlayersInGame = new NetworkVariable<int>(0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetTimerServerRPC()
    {
        TimeElapsed.Value = Time.time - GameManager.Instance.GetTimeOfStart();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddDestroyedEnemyServerRPC()
    {
        if (IsServer)
        {
            Debug.Log("Enemy destroyed, increasing EnemiesDestroyed");
            EnemiesDestroyed.Value++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddShotFiredServerRPC()
    {
        if (IsServer)
        {
            Debug.Log("Shot fired, increasing ShotsFired");
            ShotsFired.Value++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerCountServerRPC()
    {
        if (IsServer)
        {
            NumberOfPlayersInGame.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
    }

    public int GetPlayerCount()
    {
        UpdatePlayerCountServerRPC();

        return NumberOfPlayersInGame.Value;
    }

    public bool EveryoneJoinedGame()
    {
        return NumberOfPlayersInLobby.Value == NumberOfPlayersInGame.Value;
    }
}
