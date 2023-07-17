using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The front machine gun of the players tank
/// </summary>
public class MachineGun : MonoBehaviour
{
    [SerializeField]
    private Transform _barrelEnd;
    [SerializeField]
    private LineRenderer _shootTrail;
    [SerializeField]
    private float _damage = 5f;

    private bool _isShooting = false;
    private bool _isHoldingTrigger = false;

    private float _shootDistance = 5f;
    private float _roundPerMinute = 600f;

    [SerializeField]
    private LayerMask _layerMask;

    public void FixedUpdate()
    {
        if(_isHoldingTrigger && !_isShooting)
            StartCoroutine(Shoot()); 
    }

    /// <summary>
    /// Action when the trigger is being held
    /// </summary>
    public void OnHoldTrigger()
    {
        _isHoldingTrigger = true;
    }

    /// <summary>
    /// Action when the trigger is released
    /// </summary>
    public void OnReleaseTrigger()
    {
        _isHoldingTrigger = false;
    }

    /// <summary>
    /// Shoot continously to a specified distance using a given RPM
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shoot()
    {
        _isShooting = true;

        SetRandomAngle(-5f, 5f);

        RaycastHit2D hit = Physics2D.Raycast(_barrelEnd.position, _barrelEnd.up.normalized, _shootDistance, _layerMask);

        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
            }

            DrawShootTrail(hit.point);
        } else
        {
            Vector2 shootDirection = _barrelEnd.up.normalized * _shootDistance;
            Vector2 shootPosition = (Vector2) _barrelEnd.position + shootDirection;
            DrawShootTrail(shootPosition);
        }

        float flashTime = 0.1f;
        yield return new WaitForSeconds(flashTime);

        DisableShootTrail();

        float waitBetweenRounds = 60 / _roundPerMinute;
        yield return new WaitForSeconds(waitBetweenRounds);
        _isShooting = false;

        yield return null;
    }

    /// <summary>
    /// Generate an angle value between the given minimum and maximum values (both inclusive)
    /// </summary>
    /// <param name="from">The minimum angle</param>
    /// <param name="to">The maximum angle</param>
    private void SetRandomAngle(float from, float to)
    {
        float randomAngle = Random.Range(from, to);
        _barrelEnd.localEulerAngles = new Vector3 (0, 0, randomAngle);
    }

    /// <summary>
    /// Draw the shooting effect from the end of the barrel to the desired position
    /// </summary>
    /// <param name="shootPosition">The end position of the effect</param>
    private void DrawShootTrail(Vector2 shootPosition)
    {
        _shootTrail.gameObject.SetActive(true);
        _shootTrail.SetPosition(0, _barrelEnd.position);
        _shootTrail.SetPosition(1, shootPosition);
    }

    /// <summary>
    /// Disable the shooting effect
    /// </summary>
    private void DisableShootTrail()
    {
        _shootTrail.gameObject.SetActive(false);
    }

    /// <summary>
    /// Rotate the attached object to look at a direction based on a percentile value
    /// </summary>
    /// <param name="percentage">0 means minimum, 1 means maximum</param>
    public void LookAt(float percentage)
    {
        float from = -60;
        float to = 60;

        Vector3 rotation = new Vector3(0, 0, (to-from) * percentage + from);

        transform.localRotation = Quaternion.Euler(rotation);
    }
}
