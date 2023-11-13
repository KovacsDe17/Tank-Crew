using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// This class is responsible for the Relay connection between the Players.
/// </summary>
public class RelayManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static RelayManager Instance { get; private set; }

    public event EventHandler OnRelayClientStarted; //When the client joins the relay

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

        NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientId) =>
        {
            Debug.Log("Client with Id: " + clientId + " has entered the game!");
        };

        NetworkManager.Singleton.OnClientStarted += () =>
        {
            OnRelayClientStarted?.Invoke(this, EventArgs.Empty);
        };
    }

    /// <summary>
    /// Create a new relay.
    /// </summary>
    /// <returns>The join code for the relay.</returns>
    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("JoinCode: " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            GameManager.Instance.StartGame();

            return joinCode;
        } catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    /// <summary>
    /// Join a relay using a join code.
    /// </summary>
    /// <param name="joinCode">The join code of the relay.</param>
    /// <returns>A Task object, that specifies if the action was successful or not.</returns>
    public async Task JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with JoinCode: " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Join a relay using a join code.
    /// </summary>
    /// <param name="joinCodeInputField">The Input Field that has the join code.</param>
    public async void JoinRelay(TMP_InputField joinCodeInputField)
    {
        string joinCode = joinCodeInputField.text;

        await JoinRelay(joinCode);
    }
}
