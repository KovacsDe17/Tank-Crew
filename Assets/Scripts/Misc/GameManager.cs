using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class manages the games main mechanincs
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        } else
        {
            Instance = this;
        }
    }

    [SerializeField] private List<Player> _players;

    [SerializeField]
    private TextMeshProUGUI _changeButtonText;

    [SerializeField]
    private GameObject _driverCanvas, _gunnerCanvas;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach(Player player in _players)
        {
            SetChangeButtonText(player.GetPlayerType());
            SetCanvasType(player.GetPlayerType());
        }
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }   
    
    public void ResumeTime()
    {
        Time.timeScale = 1;
    }

    public void ChangePlayerType()
    {
        //TODO: Make a request for the other player and change only when it is accepted


        /*
        _player.ChangeType();

        SetChangeButtonText(_player.GetPlayerType());
        SetCanvasType(_player.GetPlayerType());
        */
    }

    private void SetChangeButtonText(Player.PlayerType type)
    {
        string typeToChangeTo = type == Player.PlayerType.Driver ? "gunner" : "driver";
        _changeButtonText.text = "Change to " + typeToChangeTo;
    }

    private void SetCanvasType(Player.PlayerType type)
    {
        if(type == Player.PlayerType.Driver)
        {
            _driverCanvas.SetActive(true);
            _gunnerCanvas.SetActive(false);
        }
        else
        {
            _gunnerCanvas.SetActive(true);
            _driverCanvas.SetActive(false);
        }
    }

    public Player getLocalPlayer()
    {
        //TODO: Find another way...
        return _players[0];
    }

    public Player getPlayerByName(string name)
    {
        foreach(Player player in _players)
        {
            if (player.name.Equals(name))
            {
                return player;
            }
        }

        return null;
    }

    public Player getPlayerByType(Player.PlayerType type)
    {
        foreach (Player player in _players)
        {
            if (player.GetPlayerType() == type)
            {
                return player;
            }
        }

        return null;
    }
}
