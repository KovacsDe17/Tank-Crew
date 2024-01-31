using System;
using UnityEngine;

/// <summary>
/// The enemy the players can destroy
/// </summary>
public class Enemy : Unit
{
    private Transform _playerTransform; //The transform of the player

    [SerializeField] LayerMask _obscureVisionLayerMask; //Define what obscures the vision of this enemy
    [SerializeField] private float _range = 12f; //The range where the enemy can see and shoot
    [SerializeField] private bool _canSeePlayer = true; //If there are no obstacles between the enemy and the player
    [SerializeField] private bool _playerTankIsSpotted = false; //If the enemy has seen the player once

    #region Main MonoBehaviour Functions
    private void Start()
    {
        SetEnabling();
    }

    private void Update()
    {
        if(_playerTransform != null)
            CheckIfCanSeePlayer(_playerTransform);
    }

    #endregion

    #region Enemy Functions

    /// <summary>
    /// Setup variables.
    /// </summary>
    public void Initialize(bool isTank)
    {
        _playerTransform = PlayerTank.Instance.transform;
        _canSeePlayer = false;
        _playerTankIsSpotted = false;

        if(isTank)
            GetComponentInParent<EnemyMovement>().Initialize(this, EventArgs.Empty);
        
        GetComponent<EnemyTurret>().Initialize(this, EventArgs.Empty);

        if(isTank)
            AudioManager.Instance.AttachConstantSound(AudioManager.Sound.Tank_Exhaust, transform, 0.5f);
    }

    /// <summary>
    /// Disable the Enemy script and re-enable on Game Start event.
    /// </summary>
    private void SetEnabling()
    {
        GameManager.Instance.OnPlayerSpawn += (sender, eventArgs) =>
        {
            enabled = true;
        };
    }

    /// <summary>
    /// Check if this Enemy can see the Player and set the variable accordingly.
    /// </summary>
    /// <param name="playerTransform">The Transform component of the Player's Tank.</param>
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
    }

    /// <summary>
    /// Calculate the distance from the Player.
    /// </summary>
    /// <returns>The distance in world units.</returns>
    public float DistanceFromPlayer(Transform playerTransform)
    {
        return Vector2.Distance(transform.position, playerTransform.position);
    }

    /// <summary>
    /// Drop Pick Ups right before death
    /// </summary>
    public override void Die()
    {
        DropPickUps();
        GameplaySync.Instance.AddDestroyedEnemyServerRPC();

        EnemyTurret enemyTurret = GetComponent<EnemyTurret>();
        if (enemyTurret != null)
            enemyTurret.CleanUp();

        base.Die();
    }

    /// <summary>
    /// Drop random pick ups, such as ammo or health
    /// </summary>
    private void DropPickUps()
    {
        //TODO: Implement, based on random number
    }

    #endregion

    #region Getters and setters

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


    public Transform GetPlayerTransform()
    {
        return _playerTransform;
    }

    #endregion
}
