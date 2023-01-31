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
    public void Fire()
    {
        Instantiate(projectilePrefab, barrelEnd.position, barrelEnd.rotation);
    }
}
