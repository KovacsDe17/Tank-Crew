using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnMove : MonoBehaviour
{
    private AudioSource _audioSource;
    private Rigidbody2D _rigidbody;

    private float _topSpeed = 2.5f;
    private float _maxPitch = 1.5f;

    void Start()
    {
        _audioSource = AudioManager.Instance.AttachConstantSound(AudioManager.Sound.Tank_Exhaust, transform, 2f);
        _rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(_rigidbody != null && _rigidbody.velocity.magnitude > 0)
        {
            float speed = _rigidbody.velocity.magnitude;
            _audioSource.pitch = 1f + ((_maxPitch - 1f) * speed/_topSpeed);
        }
    }
}
