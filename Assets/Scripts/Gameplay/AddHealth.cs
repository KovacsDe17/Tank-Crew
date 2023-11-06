using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHealth : MonoBehaviour
{
    [SerializeField]
    private float _healthToAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AddHealthToEntity(collision);
    }

    /// <summary>
    /// Add health to the colliding entity.
    /// </summary>
    /// <param name="entity">The entity that collides with this object.</param>
    private void AddHealthToEntity(Collider2D entity)
    {
        Entity tank = entity.GetComponent<Entity>();

        if (tank == null)
            return;

        else tank.RestoreHealth(_healthToAdd);

        Destroy(gameObject);
    }
}
