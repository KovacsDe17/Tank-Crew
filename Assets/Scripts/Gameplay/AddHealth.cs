using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealth : MonoBehaviour
{
    [SerializeField]
    private float _healthToAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tank tank = collision.GetComponent<Tank>();

        if (tank == null)
            return;

        else tank.RestoreHealth(_healthToAdd);

        Destroy(gameObject);
    }
}
