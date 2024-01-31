using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class represents the spawn place of the UI Ammos.
/// </summary>
public class AmmoHolder : MonoBehaviour
{
    [SerializeField]
    private RectTransform _ammoPrefab; //Prefab for the instantiation of new ammos

    [SerializeField]
    private static Dictionary<Ammo.AmmoType, int> _ammoCount; //Dictionary for how many ammos are left of each Ammo Type

    [SerializeField]
    private List<AmmoPlace> _places; //Ammo places

    [SerializeField]
    private Animator _animator; //Animator for the ammos

    public void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Setup ammo counts, their places and update the holders.
    /// </summary>
    private void Initialize()
    {
        _ammoCount = new Dictionary<Ammo.AmmoType, int>()
        {
            { Ammo.AmmoType.Practise, 2},
            { Ammo.AmmoType.ArmorPiercing, 4}
        };

        SetupAmmoPlaces();
        UpdateHolders();
    }

    /// <summary>
    /// Update the count for each Ammo Type
    /// </summary>
    private void SetupAmmoPlaces()
    {
        foreach(AmmoPlace place in _places)
        {
            place.countInfoText = place.rectTransform.transform.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Add a number of ammos of a type.
    /// </summary>
    /// <param name="ammoType">The type of the ammo.</param>
    /// <param name="count">The number of ammos to add.</param>
    public void AddAmmo(Ammo.AmmoType ammoType, int count)
    {
        if (_ammoCount.ContainsKey(ammoType))
        {
            _ammoCount[ammoType] = _ammoCount[ammoType] + count;
        } else
        {
            _ammoCount.Add(ammoType, count);
        }

        UpdateHolders();
    }

    /// <summary>
    /// Update holders. If there are no ammos held, generate them.
    /// </summary>
    private void UpdateHolders()
    {
        foreach(AmmoPlace holder in _places)
        {
            if(holder.ammoHeld == null)
            {
                holder.GenerateAmmo(_ammoPrefab, gameObject.transform);
            }
        }
    }

    /// <summary>
    /// Remove ammo from the holder.
    /// </summary>
    /// <param name="ammo">THe ammo to remove from the holder.</param>
    public void RemoveFromHolder(Ammo ammo)
    {
        foreach (AmmoPlace holder in _places)
        {
            if (holder.ammoHeld == ammo)
            {
                holder.ammoHeld = null;
                holder.GenerateAmmo(_ammoPrefab, gameObject.transform);
            }
        }
    }

    /// <summary>
    /// A place where an ammo can be placed.
    /// </summary>
    [System.Serializable]
    internal class AmmoPlace
    {
        [SerializeField]
        internal RectTransform rectTransform; //Rect Transform of the place
        [SerializeField]
        internal Ammo.AmmoType type; //Type of the ammo being held

        internal Ammo ammoHeld; //The actual ammo

        internal bool canShow; //To show the ammo or not

        internal TextMeshProUGUI countInfoText; //The count text of the ammo type

        /// <summary>
        /// Generate an ammo in the holder.
        /// </summary>
        /// <param name="ammoPrefab">The ammo prefab to generate from.</param>
        /// <param name="parent">The parent to generate under.</param>
        internal void GenerateAmmo(RectTransform ammoPrefab, Transform parent)
        {
            //If there are no ammos left of this type, return
            if (_ammoCount.GetValueOrDefault(type, 0) <= 0)
                return;

            //Create new ammo object and set its properties
            RectTransform ammoRect = Instantiate(ammoPrefab, parent);

            ammoRect.anchorMin = rectTransform.anchorMin;
            ammoRect.anchorMax = rectTransform.anchorMax;
            ammoRect.anchoredPosition = rectTransform.anchoredPosition;

            ammoHeld = ammoRect.GetComponent<Ammo>();
            ammoHeld.SetType(type);

            //Reduce the number of ammos of type left by one
            if (_ammoCount.ContainsKey(type))
            {
                _ammoCount[type] = _ammoCount[type] - 1;

                countInfoText.SetText(_ammoCount[type].ToString());
            }
        }
    }
}
