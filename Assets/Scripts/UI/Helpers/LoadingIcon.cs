using UnityEngine;
using System.Collections;

/// <summary>
/// An image that is playing it's animation automatically when enabling it.
/// </summary>
public class LoadingIcon : MonoBehaviour
{
    [SerializeField] float _speed;

    IEnumerator SpinLogo()
    {
        transform.Rotate(-Vector3.forward * (_speed * Time.deltaTime));
        yield return null;
    }

    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        StartCoroutine(SpinLogo());
    }
}
