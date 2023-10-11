using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeHealth : MonoBehaviour
{
    [SerializeField]
    private float _healthToTake;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity tank = collision.GetComponent<Entity>();

        if (tank == null)
            return;

        else tank.TakeDamage(_healthToTake);

        Destroy(gameObject);
    }
}
