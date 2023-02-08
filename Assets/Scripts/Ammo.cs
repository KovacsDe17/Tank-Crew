using UnityEngine;

/// <summary>
/// This class is responsible for the UI representation of an ammunition
/// </summary>
public class Ammo : MonoBehaviour
{
    public enum AmmoType { Practise, Regular, ArmorPiercing, HighExplosive, Incendiary }

    public GameObject head;
    public AmmoType type;

    private bool _isReady, _isShot;

    private void Awake()
    {
        Initalize();
    }

    /// <summary>
    /// Assign the UIAmmoHandler object and set it to be shootable
    /// </summary>
    private void Initalize()
    {
        _isReady = true;
        _isShot = false;
    }

    /// <summary>
    /// If possible, remove the head of the ammunition
    /// </summary>
    public void Fire()
    {
        if (!_isReady || _isShot)
            return;

        head.SetActive(false);
        _isReady = false;
        _isShot = true;
    }


    public void SetReady(bool ready)
    {
        _isReady = ready;
    }

    /// <summary>
    /// Check if the ammunition can be fired
    /// </summary>
    /// <returns>True if the ammunition can be fired</returns>
    public bool IsReady()
    {
        return _isReady;
    }

    /// <summary>
    /// Check if the ammunition was already fired
    /// </summary>
    /// <returns>True if the ammunition can be fired</returns>
    public bool IsShot()
    {
        return _isShot;
    }
}
