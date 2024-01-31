using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the states of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    //Singleton instance

    public event EventHandler OnGameStart; //Event for starting the game
    public event EventHandler OnGameEnd; //Event for winning the game
    public event EventHandler OnPlayerSpawn; //Event for spawning the Player
    public event EventHandler OnSetupComplete; //Event for multiplayer setup being completed
    public event EventHandler OnPause; //Event for pause
    public event EventHandler OnResume; //Event for resume

    public Transform ParentTransformEnemies; //Parent for Enemies in the game hierarchy
    public Transform ParentTransformPickUps; //Parent for PickUps in the game hierarchy
    public Transform ParentTransformStaticObjects;  //Parent for Static objects in the game hierarchy
    [SerializeField] private Slider _playerHealthBar; //Health Bar for the player

    private float _timeOfStart;
    private float _elapsedTime;
    private int _enemiesDestroyed;
    private int _shotsFired;

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

    private void Start()
    {
        GameplaySync.Instance.IsDead.OnValueChanged += (p, n) =>
        {
            if(n == true)
            {
                Debug.Log("Player has died, calling InvokeOnEndGame(false)!");
                InvokeOnGameEnd(false);
            }
        };

        GameplaySync.Instance.HasWonGame.OnValueChanged += (p, n) =>
        {
            if (n == true)
            {
                Debug.Log("Player has won, calling InvokeOnEndGame(true)!");
                InvokeOnGameEnd(true);
            }
        };

        GameplaySync.Instance.ShotsFired.OnValueChanged += (p, n) => _shotsFired = n;
        GameplaySync.Instance.EnemiesDestroyed.OnValueChanged += (p, n) => _enemiesDestroyed = n;
        GameplaySync.Instance.TimeElapsed.OnValueChanged += (p, n) => _elapsedTime = n;
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseTime()
    {
        Time.timeScale = 0;
        OnPause?.Invoke(this, EventArgs.Empty);
    }   
    
    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeTime()
    {
        Time.timeScale = 1;
        OnResume?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void StartGame()
    {
        _timeOfStart = Time.time;
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Invoke event when the game ends.
    /// </summary>
    /// <param name="won">Wether ot not the Player won the game.</param>
    public void InvokeOnGameEnd(bool won)
    {
        OnGameEnd?.Invoke(this, new EndGameEventArgs {
            won = won,
            timeElapsed = _elapsedTime,
            enemiesDestroyed = _enemiesDestroyed,
            shotsFired = _shotsFired
        });

        AudioManager.Instance.MuteCategory(AudioManager.Category.IngameSound, true);
        AudioManager.Instance.PlayMusic(AudioManager.Music.MainTheme);
    }

    public void InvokeOnPlayerSpawn()
    {
        Debug.Log("OnPlayerSpawned invoked!");
        OnPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeOnSetupComplete()
    {
        Debug.Log("OnSetupComplete invoked!");
        OnSetupComplete?.Invoke(this, EventArgs.Empty);
    }

    public Slider GetPlayerHealthBar()
    {
        return _playerHealthBar;
    }

    public float GetTimeOfStart()
    {
        return _timeOfStart;
    }

    public class EndGameEventArgs : EventArgs
    {
        public bool won;
        public float timeElapsed;
        public int enemiesDestroyed;
        public int shotsFired;
    }
}
