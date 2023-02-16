using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [SerializeField]
    private Transform _barrelEnd;
    [SerializeField]
    private LineRenderer _shootTrail;

    private bool isShooting = false;

    private float _shootDistance = 5f;
    private float _roundPerMinute = 600f;

    public void OnHoldTrigger()
    {
        if(!isShooting)
            StartCoroutine(Shoot(_shootDistance, _roundPerMinute));
    }

    private IEnumerator Shoot(float shootDistance, float roundPerMinute)
    {
        isShooting = true;

        SetRandomAngle(-10f, 10f);

        RaycastHit2D hit = Physics2D.Raycast(_barrelEnd.position, _barrelEnd.up, shootDistance);

        if (hit)
        {
            Debug.Log("We shot " + hit.transform.name);

            DrawShootTrail(hit.point);
        } else
        {
            Vector2 shootPosition = _barrelEnd.up * shootDistance;
            DrawShootTrail(shootPosition);
        }

        float timeToWait = 60 / roundPerMinute;
        yield return new WaitForSeconds(timeToWait);

        DisableShootTrail();

        isShooting = false;

        yield return null;
    }

    private void SetRandomAngle(float from, float to)
    {
        float randomAngle = Random.Range(from, to);
        _barrelEnd.localEulerAngles = new Vector3 (0, 0, randomAngle);
    }

    private void DrawShootTrail(Vector2 shootPosition)
    {
        _shootTrail.gameObject.SetActive(true);
        _shootTrail.SetPosition(0, _barrelEnd.position);
        _shootTrail.SetPosition(1, shootPosition);
    }

    private void DisableShootTrail()
    {
        _shootTrail.gameObject.SetActive(false);
    }

    public void LookAt(float percentage)
    {
        float from = -60;
        float to = 60;

        Vector3 rotation = new Vector3(0, 0, (to-from) * percentage + from);

        transform.localRotation = Quaternion.Euler(rotation);
    }
}
