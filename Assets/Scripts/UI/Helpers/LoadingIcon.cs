using System;
using System.Collections;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An image that is playing it's animation automatically when enabling it.
/// </summary>
public class LoadingIcon : MonoBehaviour
{
    [SerializeField] float _speed;
    private Animator _animator;
    private Image _image;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        //_animator.enabled = false;

        _image = GetComponent<Image>();
        //_image.enabled = false;
    }

    /*
    public void Enable(object sender, EventArgs e)
    {
        _image.enabled = true;
        _animator.enabled = true;
        _animator.Play("Spin");
    }

    public void Disable(object sender, EventArgs e)
    {
        _image.enabled = false;
        _animator.enabled = false;
    }
    */

    private void Update()
    {
        
    }

    IEnumerator SpinLogo()
    {
        transform.Rotate(-Vector3.forward * (_speed * Time.deltaTime));
        yield return null;
    }

    private void OnEnable()
    {
        StartCoroutine(SpinLogo());
        //_animator.Play("Spin");
    }
}
