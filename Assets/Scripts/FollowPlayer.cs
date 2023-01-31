using UnityEngine;

/// <summary>
/// Camera to follow the players position. Catch up time can be set for the camera movement, where 0 means instant snapping
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    public float catchUpTime; //TODO: use this for the smooth camera movement

    private Transform _playerTransform;

    private void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = GetPlayerPosition();
    }

    private void Update()
    {
        transform.position = GetPlayerPosition();
    }

    /// <summary>
    /// Get the players position in world coordinates
    /// </summary>
    /// <returns>The Vector3 world coordinates of the player, with the z value of the camera</returns>
    private Vector3 GetPlayerPosition()
    {
        return new Vector3(
            _playerTransform.position.x,
            _playerTransform.position.y,
            transform.position.z
            );
    }
}
