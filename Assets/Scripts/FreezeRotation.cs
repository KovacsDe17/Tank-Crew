using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    public void Update()
    {
        if(transform.rotation.z != 0)
            transform.rotation = Quaternion.identity;
    }
}
