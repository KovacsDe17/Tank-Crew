using Unity.Netcode;
using UnityEngine;

/// <summary>
/// This class is responsible for handling the turret object of the tank
/// </summary>
public class Turret : MonoBehaviour
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
}
