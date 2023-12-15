using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent; //The Agent which moves the Enemy
    private Enemy _enemy; //The Enemy script attached to this object
    private Transform _playerTransform; //The Transform of the PlayerTank
    [SerializeField] private float _rotationThreshold = 15f; //Threshold for checking the look rotation to face the player in degrees
    private float _maxSpeed;

    private void Start()
    {
        GameManager.Instance.OnSetupComplete += Initialize;
    }

    private void OnEnable()
    {
        Debug.Log("Enemy enabled...");
        //Initialize(this, EventArgs.Empty);
    }

    public void Initialize(object sender, EventArgs e)
    {
        Debug.Log("Initializing Enemy Movement...");

        _agent = GetComponent<NavMeshAgent>();
        _enemy = GetComponentInChildren<Enemy>();
        _playerTransform = PlayerTank.Instance.transform;

        _agent.enabled = false;
        _agent.stoppingDistance = _enemy.GetRange() * 0.75f;
        _agent.enabled = true;

        _maxSpeed = _agent.speed;

        PlayerTank.Instance.OnPlayerDestroyed += (sender, eventArgs) =>
        {
            enabled = false;
        };

        GameManager.Instance.OnGameEnd += (sender, eventArgs) =>
        {
            enabled = false;
        };
    }

    private void Update()
    {
        if (PlayerTank.Instance == null || _enemy == null || _agent == null) return;
        
        if (_enemy.PlayerTankIsSpotted())
            MoveToPlayerProximity();
    }

    /// <summary>
    /// Move the enemy to the specified proximity of the PlayerTank.
    /// </summary>
    /// <param name="proximity">The distance to keep between the entities, in world units.</param>
    private void MoveToPlayerProximity()
    {
        Vector3 target = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
        _agent.SetDestination(target);

        /*
        if(Mathf.RoundToInt(EulerAnglesToPlayer(_playerTransform)) != Mathf.RoundToInt(transform.rotation.eulerAngles.z))
        {
            _agent.speed = 0;
            Debug.Log("EM - NO");
        }
        else
        {
            _agent.speed = _maxSpeed;
            Debug.Log("EM - YES");
        }
        */
        
        /*
        if (!RotationCloseToPlayer(_playerTransform, _rotationThreshold))
        {
            _agent.isStopped = true;
            _agent.updateRotation = false;
            RotateTowardsPlayer(_playerTransform);
        } else
        {
            _agent.updateRotation = true;
            _agent.isStopped = false;
            Vector3 target = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
            _agent.SetDestination(target);
        }
        */
    }

    /// <summary>
    /// Check if this transforms eulerAngle z rotation is within the look rotation to the player plus/minus the threshold.
    /// </summary>
    /// <param name="playerTransform">The Transform of the Player to look at.</param>
    /// <param name="threshold">The plus/minus threshold in eulerAngles.</param>
    /// <returns>True if the rotation is in the threshold.</returns>
    private bool RotationCloseToPlayer(Transform playerTransform, float threshold)
    {
        bool inside = false;

        float angleToPlayer = EulerAnglesToPlayer(playerTransform);

        //float wrappedAngleToPlayer = WrapAngle(angleToPlayer);
        //float wrappedCurrentRotation = WrapAngle(transform.rotation.eulerAngles.z);

        float upperThreshold = angleToPlayer + threshold;
        float lowerThreshold = angleToPlayer - threshold;

        float currentRotation = transform.rotation.eulerAngles.z - 90f;

        if (currentRotation <= upperThreshold && currentRotation >= lowerThreshold) inside = true;

        Debug.Log("Angle: " + currentRotation + "/" + angleToPlayer + " | inside: " + (inside?"yes":"no"));

        return inside;
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

        Vector2 directionTowardsPlayer = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(directionTowardsPlayer.y, directionTowardsPlayer.x) * Mathf.Rad2Deg;

        return angle + rotationOffset;
    }

    /// <summary>
    /// Wraps the given angle to be between 0 and 360 degrees.
    /// </summary>
    /// <param name="angle">The angle to keep between 0 and 360 degrees.</param>
    /// <returns>The angle wrapped between 0 and 360 degrees.</returns>
    private float WrapAngle(float angle)
    {
        if (angle < 0) angle += 360;

        if (angle > 360) angle -= 360;

        return angle;
    }

    /// <summary>
    /// Rotate to face the PlayerTank.
    /// </summary>
    /// <param name="playerTransform">The Transform of the PlayerTank.</param>
    private void RotateTowardsPlayer(Transform playerTransform)
    {
        //Getting the target rotation
        float rotationToPlayer = EulerAnglesToPlayer(playerTransform);
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationToPlayer);

        //Setting up new rotation (and position) based on the target rotation
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _agent.angularSpeed * Time.deltaTime);
        Vector3 position = transform.position;
    }

    /// <summary>
    /// Checks if the angle to face the PlayerTank can be achieved faster when turning clockwise or counterclockwise.
    /// </summary>
    /// <param name="playerTransform">The Transform of the PlayerTank.</param>
    /// <returns>A signed int of 1 depending on which turn direction would be closer.</returns>
    private int AngleCloserToPlayer(Transform playerTransform)
    {
        float angleToPlayer = Vector2.Angle(playerTransform.position, transform.position);

        float wrappedAngleToPlayer = WrapAngle(angleToPlayer);
        float wrappedCurrentRotation = WrapAngle(transform.rotation.eulerAngles.z);

        float clockWise = WrapAngle(wrappedAngleToPlayer - wrappedCurrentRotation); //Angle if we turned clockwise
        float counterClockWise = WrapAngle(wrappedCurrentRotation - wrappedAngleToPlayer); //Angle if we turned counter clockwise

        return (clockWise <= counterClockWise) ? 1 : -1; //Depending on whichever would be closer, return 1 or -1 as a multiplier
    }
}
