using TMPro;
using UnityEngine;

/// <summary>
/// This class manages the games main mechanincs
/// </summary>
public class GameManager : MonoBehaviour
{
    private Player _player;

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
        //while(_player == null)
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        SetChangeButtonText(_player.GetPlayerType());
        SetCanvasType(_player.GetPlayerType());
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

        SetChangeButtonText(_player.GetPlayerType());
        SetCanvasType(_player.GetPlayerType());
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
