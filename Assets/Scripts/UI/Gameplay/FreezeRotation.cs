using UnityEngine;

/// <summary>
/// Freeze z rotation of the object attached.
/// </summary>
public class FreezeRotation : MonoBehaviour
{
    public void Update()
    {
        if(transform.rotation.z != 0)
            transform.rotation = Quaternion.identity;
    }
}
