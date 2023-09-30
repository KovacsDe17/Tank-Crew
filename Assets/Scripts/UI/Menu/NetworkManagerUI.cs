using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class connects the buttons of the Multiplayer Start Menu to the corresponding action of the Network Manager.
/// </summary>
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    [SerializeField] private GameObject pauseButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            CommonBehavior();
        });

        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            CommonBehavior();
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            CommonBehavior();
        });
    }

    private void CommonBehavior()
    {
        gameObject.SetActive(false);
        pauseButton.SetActive(true);
    }
}
