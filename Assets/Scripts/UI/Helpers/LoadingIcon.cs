using System.Collections;
using UnityEngine;

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
        StartCoroutine(SpinLogo());
    }
}
