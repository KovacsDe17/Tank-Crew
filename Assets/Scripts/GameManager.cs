using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is to manage the games main mechanincs
/// </summary>
public class GameManager : MonoBehaviour
{
    private Player _player;

    [SerializeField]
    private TextMeshProUGUI _changeButtonText;

    [SerializeField]
    private GameObject _driverCanvas, _gunnerCanvas;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        SetChangeButtonText(_player.GetType());
        SetCanvasType(_player.GetType());
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
        _player.ChangeType();

        SetChangeButtonText(_player.GetType());
        SetCanvasType(_player.GetType());
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
}
