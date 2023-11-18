using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages the games main mechanincs
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    //Singleton instance

    public event EventHandler OnGameStart; //Event for starting the game
    public event EventHandler OnGameEnd; //Event for winning the game
    public event EventHandler OnPlayerSpawn; //Event for spawning the Player
    public event EventHandler OnPause; //Event for pause
    public event EventHandler OnResume; //Event for resume

    public Transform ParentTransformEnemies; //Parent for Enemies in the game hierarchy
    public Transform ParentTransformPickUps; //Parent for PickUps in the game hierarchy
    public Transform ParentTransformStaticObjects;  //Parent for Static objects in the game hierarchy
    [SerializeField] private Slider _playerHealthBar; //Health Bar for the player

    private float _timeOfStart;
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

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            InvokeOnGameEnd(true);
        }
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
        Debug.Log("Game Started!");
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
            timeElapsed = Time.time - _timeOfStart,
            enemiesDestroyed = _enemiesDestroyed,
            shotsFired = _shotsFired
        });
    }

    public void InvokeOnPlayerSpawn()
    {
        Debug.Log("OnPlayerSpawned invoked!");
        OnPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }

    public Slider GetPlayerHealthBar()
    {
        return _playerHealthBar;
    }

    public class EndGameEventArgs : EventArgs
    {
        public bool won;
        public float timeElapsed;
        public int enemiesDestroyed;
        public int shotsFired;
    }
}
