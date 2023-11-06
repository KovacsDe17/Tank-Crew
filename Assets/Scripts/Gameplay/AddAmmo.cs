using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAmmo : MonoBehaviour
{
    [SerializeField]
    Ammo.AmmoType ammoType;
    [SerializeField]
    int count;

    /// <summary>
    /// Add ammo to the holder.
    /// </summary>
    private void AddAmmoToHolder()
    {
        Debug.Log("Touchy");

        AmmoHolder ammoHolder = GameObject.FindGameObjectWithTag("AmmoHolder").GetComponent<AmmoHolder>();
        if (ammoHolder == null)
            return;

        ammoHolder.AddAmmo(ammoType, count);

        Debug.Log("yaay, ammo added");

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        AddAmmoToHolder();
    }
}
