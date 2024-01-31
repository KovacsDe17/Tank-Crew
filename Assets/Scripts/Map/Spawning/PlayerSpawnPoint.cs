using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        SpawnPlayer();
    }

    /// <summary>
    /// Move the PlayerTank to the position of this Transform then destroy this GameObject.
    /// </summary>
    private void SpawnPlayer()
    {
        PlayerTank.Instance.transform.position = transform.position;

        PlayerTank.Instance.transform.GetOrAddComponent<SoundOnMove>();
    }
}
