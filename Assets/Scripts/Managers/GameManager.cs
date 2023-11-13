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
    public event EventHandler OnGameOverLose; //Event for losing the game
    public event EventHandler OnGameOverWin; //Event for winning the game
    public event EventHandler OnPlayerSpawn; //Event for spawning the Player
    public event EventHandler OnPause; //Event for pause
    public event EventHandler OnResume; //Event for resume

    public Transform ParentTransformEnemies; //Parent for Enemies in the game hierarchy
    public Transform ParentTransformPickUps; //Parent for PickUps in the game hierarchy
    public Transform ParentTransformStaticObjects;  //Parent for Static objects in the game hierarchy
    [SerializeField] private Slider _playerHealthBar; //Health Bar for the player

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
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// When the Player wins
    /// </summary>
    public void OnGameWin()
    {
        OnGameOverWin?.Invoke(this, EventArgs.Empty);
        Debug.Log("Congratulations! You won!");
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
}
