using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMutliplayerMovement : NetworkBehaviour
{
    private Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButton(0))
        {
            rigidbody.AddTorque(-2f);
        }
    }
}
