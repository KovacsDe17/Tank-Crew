using UnityEngine;

/// <summary>
/// Makes the object to follow the players position. Catch up time can be set for the camera movement, where 0 means instant snapping
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private float _catchUpSpeed = 10f; //TODO: use this for the smooth camera movement
    [SerializeField]
    private Vector3 _offset;

    private Transform _playerTransform;

    private void Awake()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        Follow();
    }

    private void Initialize()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Follow() 
    {
        Vector3 desiredPosition = _playerTransform.position + _offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, _catchUpSpeed * Time.deltaTime);

        transform.position = smoothPosition;
    }
}
