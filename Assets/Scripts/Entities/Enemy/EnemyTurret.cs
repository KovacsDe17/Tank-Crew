using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof (Enemy))]
public class EnemyTurret : MonoBehaviour
{
     private Enemy _enemy; //The Enemy script on this object
    [SerializeField] private Turret _turret; //The turret which rotates
    [SerializeField] private float _rotationSpeed = 15f; //How fast the turret can rotate
    [SerializeField] private float _reloadTime = 3f; //Seconds to pass between each shot
    [SerializeField] private float _damage = 25f; //Seconds to pass between each shot
    [SerializeField] private bool _reloaded = true; //Wether the turret has reloaded

    private void Start()
    {
        GameManager.Instance.OnSetupComplete += Initialize;
    }

    private void OnEnable()
    {
        Debug.Log("EnemyTurret enabled...");
        //Initialize(this, EventArgs.Empty);
    }


    private void Update()
    {
        if (PlayerTank.Instance == null || _enemy == null) return;

        AimAndShootAtPlayer(_enemy.GetPlayerTransform());
    }

    /// <summary>
    /// Setup variables
    /// </summary>
    public void Initialize(object sender, EventArgs e)
    {
        Debug.Log("Initializing Enemy Turret...");

        _enemy = gameObject.GetComponent<Enemy>();

        GameManager.Instance.OnGameStart += (sender, eventArgs) =>
        {
            enabled = true;
        };

        PlayerTank.Instance.OnPlayerDestroyed += (sender, eventArgs) =>
        {
            enabled = false;
        };

        GameManager.Instance.OnGameEnd += (sender, eventArgs) =>
        {
            enabled = false;
        };
    }

   

    /// <summary>
    /// If the player is in the range of this turret and the player can be seen, aim and shoot at it.
    /// </summary>
    /// <param name="playerTransform">The Transform of the Player.</param>
    public void AimAndShootAtPlayer(Transform playerTransform)
    {
        if (playerTransform == null) return;

        //If the player is in range, and can be seen, look at it, and shoot
        if (_enemy.DistanceFromPlayer(playerTransform) <= _enemy.GetRange() && _enemy.CanSeePlayer())
        {
            AimAtPlayer(playerTransform);
            ShootAtPlayer(playerTransform);
        }
    }

    /// <summary>
    /// Aims the turret at the given Player Transform.
    /// </summary>
    /// <param name="playerTransform">The Player Transform to aim at.</param>
    private void AimAtPlayer(Transform playerTransform)
    {
        //Getting the target rotation
        float rotationToPlayer = EulerAnglesToPlayer(playerTransform);
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationToPlayer);

        //Setting up new rotation (and position) based on the target rotation
        Quaternion rotation = Quaternion.RotateTowards(_turret.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        Vector3 position = _turret.transform.position;

        //Applying the new rotation (and position) values to the turret
        _turret.transform.SetPositionAndRotation(position, rotation);
    }

    /// <summary>
    /// Shoot when aiming at the Player.
    /// </summary>
    /// <param name="playerTransform">The Transform of the Player.</param>
    private void ShootAtPlayer(Transform playerTransform)
    {
        if (!_reloaded) return;

        float angle = _turret.transform.eulerAngles.z;
        float targetAngle = EulerAnglesToPlayer(playerTransform);
        float limit = 2f;

        //TODO: counter for RPM
        //If looking at player, shoot
        if (EulerAngleCloseTo(angle, targetAngle, limit))
        {
            Shoot();
            StartCoroutine(Reload());
        }
    }

    /// <summary>
    /// Calculate the rotation on the Z axis, when facing the player.
    /// </summary>
    /// <param name="playerTransform">The Transform of the Player.</param>
    /// <returns></returns>
    private float EulerAnglesToPlayer(Transform playerTransform)
    {
        //Offset for rotation in euler angles
        const float rotationOffset = -90;

        Vector2 directionTowardsPlayer = playerTransform.position - _turret.transform.position;
        float angle = Mathf.Atan2(directionTowardsPlayer.y, directionTowardsPlayer.x) * Mathf.Rad2Deg;
        
        return angle + rotationOffset;
    }

    /// <summary>
    /// Normalizes the given euler angle to be between 0 and 360 degrees.
    /// </summary>
    /// <param name="angle">The euler angle to be normalized.</param>
    /// <returns>The angle as a normalized value.</returns>
    private float NormalizeEulerAngle(float angle)
    {
        if (angle < 0) angle += 360;

        if (angle > 360) angle -= 360;

        return angle;
    }

    /// <summary>
    /// Check if the given angle is in the limit range of the specified target angle.
    /// </summary>
    /// <param name="angle">The angle to check.</param>
    /// <param name="targetAngle">The target angle which the angle is checked against.</param>
    /// <param name="limit">The range applied to the target angle. Mind that it applies to both sides, meaning a +/- approach.</param>
    /// <returns>True if the angle is within the range of the target angle +/- the limit.</returns>
    private bool EulerAngleCloseTo(float angle, float targetAngle, float limit)
    {
        if(NormalizeEulerAngle(angle) <= NormalizeEulerAngle(targetAngle)+limit && NormalizeEulerAngle(angle) >= NormalizeEulerAngle(targetAngle) - limit)
            return true;

        return false;
    }

    private void Shoot()
    {
        Debug.Log("Shoot called by Enemy");
        _turret.FireProjectileServerRPC(_damage);

        _reloaded = false;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(_reloadTime);

        _reloaded = true;
    }
}
