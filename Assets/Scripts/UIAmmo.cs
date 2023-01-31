using UnityEngine;

/// <summary>
/// This class is responsible for the UI representation of an ammunition
/// </summary>
public class UIAmmo : MonoBehaviour
{
    public enum AmmoType { Practise, Regular, ArmorPiercing, HighExplosive }

    public GameObject ammoHead;
    private UIAmmoHandler ammoHandler;

    private bool canBeShot;
    public AmmoType type;

    private void Awake()
    {
        Initalize();
    }

    /// <summary>
    /// Assign the UIAmmoHandler object and set it to be shootable
    /// </summary>
    private void Initalize()
    {
        ammoHandler = GetComponent<UIAmmoHandler>();
        canBeShot = true;
    }

    /// <summary>
    /// If possible, remove the head of the ammunition
    /// </summary>
    public void RemoveHead()
    {
        if (!canBeShot || !ammoHandler.IsReady())
            return;

        ammoHead.SetActive(false);
        canBeShot = false;
    }

    /// <summary>
    /// Get the singleton handler object
    /// </summary>
    /// <returns>The handler object</returns>
    public UIAmmoHandler GetHandler()
    {
        return ammoHandler;
    }

    /// <summary>
    /// Check if the ammunition can be fired
    /// </summary>
    /// <returns>Whether the ammunition can be fired</returns>
    public bool CanBeShot()
    {
        return canBeShot;
    }
}
