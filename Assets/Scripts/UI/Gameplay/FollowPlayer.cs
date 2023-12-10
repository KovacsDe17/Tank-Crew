using System;
using System.Collections;
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

    [Header("Boundaries")]
    [SerializeField]
    private float _left;
    [SerializeField]
    private float _right;
    [SerializeField]
    private float _top;
    [SerializeField]
    private float _bottom;

    private Transform _playerTransform;

    private bool _cameraIsAtPosition = false;

    private void Start()
    {
        StartCoroutine(WaitForCameraSetup());
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
    /// Set the Player transform
    /// </summary>
    public void SetPlayerTransform()
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
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, catchUpSpeed * Time.deltaTime);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, _left, _right),
            Mathf.Clamp(transform.position.y, _bottom, _top),
            transform.position.z
        );

        if(ApproxEqual(transform.position, desiredPosition))
            _cameraIsAtPosition=true;
        else
            _cameraIsAtPosition=false;
    }

    private IEnumerator WaitForCameraSetup()
    {
        yield return new WaitUntil(() => _cameraIsAtPosition == true);

        AudioManager.Instance.PlayMusic(AudioManager.Music.GameplayCalm);

        GameManager.Instance.InvokeOnSetupComplete();
    }

    private bool ApproxEqual(Vector3 a, Vector3 b)
    {
        Vector2Int a_v2 = new Vector2Int(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y));
        Vector2Int b_v2 = new Vector2Int(Mathf.RoundToInt(b.x), Mathf.RoundToInt(b.y));

        return (a_v2 == b_v2);
    }
}
