using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAmmo : MonoBehaviour
{
    [SerializeField]
    Ammo.AmmoType ammoType;
    [SerializeField]
    int count;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Touchy");

        AmmoHolder ammoHolder = GameObject.FindGameObjectWithTag("AmmoHolder").GetComponent<AmmoHolder>();
        if (ammoHolder == null)
            return;

        ammoHolder.AddAmmo(ammoType, count);

        Debug.Log("yaay, ammo added");

        Destroy(gameObject);

    }
}
