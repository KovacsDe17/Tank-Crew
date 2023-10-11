using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The enemies the player can destroy
/// </summary>
public class Enemy : Entity
{
    private Transform _playerTransform; //The transform of the player

    [SerializeField] LayerMask _obscureVisionLayerMask; //Define what obscures the vision of this enemy
    [SerializeField] private float _range = 12f; //The range where the enemy can see and shoot
    [SerializeField] private bool _canSeePlayer = true; //If there are no obstacles between the enemy and the player
    [SerializeField] private bool _playerTankIsSpotted = false; //If the enemy has seen the player once

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        CheckIfCanSeePlayer(_playerTransform);
    }

    private void Initialize()
    {
        _playerTransform = PlayerTank.Instance.transform;

        _canSeePlayer = false;
        _playerTankIsSpotted = false;

        enabled = false;

        GameManager.Instance.OnGameStart.AddListener(() =>
        {
            enabled = true;
        });
    }

    public void CheckIfCanSeePlayer(Transform playerTransform)
    {
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, directionToPlayer, _range, _obscureVisionLayerMask);

        //If we hit something, and it's the player, set _canSeePlayer and _playerTankIsSpotted to true
        if (raycastHit.collider != null && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _canSeePlayer = true;
            _playerTankIsSpotted = true;
        }
        else
            _canSeePlayer = false;

        //Debug.DrawLine(transform.position, playerTransform.position, _canSeePlayer?Color.blue:Color.red);
    }

    /// <summary>
    /// Calculate the distance from the Player.
    /// </summary>
    /// <returns>The distance in world units.</returns>
    public float DistanceFromPlayer(Transform playerTransform)
    {
        return Vector2.Distance(transform.position, playerTransform.position);
    }

    public float GetRange()
    {
        return _range;
    }

    public bool CanSeePlayer()
    {
        return _canSeePlayer;
    }

    public bool PlayerTankIsSpotted()
    {
        return _playerTankIsSpotted;
    }

    /// <summary>
    /// Drop Pick Ups right before death
    /// </summary>
    public override void Die()
    {
        DropPickUps();

        base.Die();
    }

    /// <summary>
    /// Drop random pick ups, such as ammo and health
    /// </summary>
    private void DropPickUps()
    {
        //TODO: Implement, based on random number
    }

    public Transform GetPlayerTransform()
    {
        return _playerTransform;
    }
}