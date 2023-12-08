using Unity.Netcode;
using UnityEngine;

/// <summary>
/// This class is responsible for handling the turret object of the tank
/// </summary>
public class Turret : NetworkBehaviour
{
    public GameObject projectilePrefab;
    public Transform barrelEnd;

    /// <summary>
    /// Create a projectile object at the end of the barrel and let it go forward
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void FireProjectileServerRPC()
    {
        GameObject projectile = Instantiate(projectilePrefab, barrelEnd.position, barrelEnd.rotation);
        projectile.GetComponent<NetworkObject>().Spawn(true);
    }

    /// <summary>
    /// Create a projectile object at the end of the barrel and let it go forward
    /// </summary>
    /// <param name="damage">Damage applied to the hit entity</param>
    [ServerRpc(RequireOwnership = false)]
    public void FireProjectileServerRPC(float damage)
    {
        Debug.Log("Firing projectile using ServerRPC");
        GameObject projectile = Instantiate(projectilePrefab, barrelEnd.position, barrelEnd.rotation);
        projectile.GetComponent<AmmoProjectile>().SetDamage(damage);
        projectile.GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (transform.parent == null)
            Destroy(gameObject);
    }
}
