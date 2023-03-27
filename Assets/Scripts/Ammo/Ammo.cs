using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This class is responsible for the UI representation of an ammunition
/// </summary>
public class Ammo : MonoBehaviour
{
    public enum AmmoType { Practise, Regular, ArmorPiercing, HighExplosive, Incendiary }

    private Dictionary<AmmoType, Color> typeColors = new Dictionary<AmmoType, Color> 
    {
        {AmmoType.Practise, Color.green},
        {AmmoType.Regular, Color.blue},
        {AmmoType.ArmorPiercing, Color.grey},
        {AmmoType.HighExplosive, Color.black},
        {AmmoType.Incendiary, Color.red},
    };

    [SerializeField]
    private GameObject _head;
    [SerializeField]
    private AmmoType _type;
    [SerializeField]
    private Image _stripe;

    private AmmoHolder _ammoHolder;

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
        UpdateStripeColor();

        _isReady = true;
        _isShot = false;

        _ammoHolder = GameObject.FindGameObjectWithTag("AmmoHolder").GetComponent<AmmoHolder>();
    }

    private void UpdateStripeColor()
    {
        _stripe.color = typeColors.GetValueOrDefault(_type);
    }

    public void SetType(AmmoType type)
    {
        _type = type;

        UpdateStripeColor();

        //TODO: UpdateFunctionality();
    }

    /// <summary>
    /// If possible, remove the head of the ammunition
    /// </summary>
    public void Fire()
    {
        if (!_isReady || _isShot)
            return;

        _head.SetActive(false);
        _isReady = false;
        _isShot = true;
        _ammoHolder.RemoveFromHolder(this);
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
