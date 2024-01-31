using UnityEngine;

/// <summary>
/// Add ammo to an Ammo Holder.
/// </summary>
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
        AmmoHolder ammoHolder = GameObject.FindGameObjectWithTag("AmmoHolder").GetComponent<AmmoHolder>();
        if (ammoHolder == null)
            return;

        ammoHolder.AddAmmo(ammoType, count);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        AddAmmoToHolder();
    }
}
