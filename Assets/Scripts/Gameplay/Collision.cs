using UnityEngine;

/// <summary>
/// Checks collision by 2D trigger.
/// </summary>
public class Collision : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("yihhaa");
    }
}
