using System;
using UnityEngine;

/// <summary>
/// Makes the object to follow the players position. Catch up time can be set for the camera movement, where 0 means instant snapping
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private float _catchUpSpeed = 10f;
    [SerializeField]
    private Vector3 _offset;

    private Transform _playerTransform;

    private void Start()
    {
        GameManager.Instance.OnPlayerSpawn += SetPlayerTransform;
    }

    private void FixedUpdate()
    {
        if(_playerTransform != null)
            Follow(_playerTransform, _catchUpSpeed);
    }

    /// <summary>
    /// Set the Player transform
    /// </summary>
    private void SetPlayerTransform(object sender, EventArgs e)
    {
        _playerTransform = PlayerTank.Instance.transform;
    }

    /// <summary>
    /// Move to object to follow the given object
    /// </summary>
    /// <param name="objectToFollow">The object to follow</param>
    /// <param name="catchUpSpeed">Time delay to move</param>
    private void Follow(Transform objectToFollow, float catchUpSpeed) 
    {
        Vector3 desiredPosition = objectToFollow.position + _offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, catchUpSpeed * Time.deltaTime);

        transform.position = smoothPosition;
    }
}
