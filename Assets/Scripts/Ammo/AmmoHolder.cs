using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoHolder : MonoBehaviour
{
    [SerializeField]
    private RectTransform _ammoPrefab;

    [SerializeField]
    private static Dictionary<Ammo.AmmoType, int> _ammoCount;

    [SerializeField]
    private List<AmmoPlace> _places;

    [SerializeField]
    private Animator _animator;

    public void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _ammoCount = new Dictionary<Ammo.AmmoType, int>()
        {
            { Ammo.AmmoType.Practise, 2},
            { Ammo.AmmoType.Incendiary, 4}
        };

        UpdateHolders();
    }

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

    [System.Serializable]
    internal class AmmoPlace
    {
        [SerializeField]
        internal RectTransform rectTransform;
        [SerializeField]
        internal Ammo.AmmoType type;

        internal Ammo ammoHeld;

        internal bool canShow;

        [SerializeField]
        internal TextMeshProUGUI countInfoText;

        internal void GenerateAmmo(RectTransform ammoPrefab, Transform parent)
        {
            if (_ammoCount.GetValueOrDefault(type, 0) <= 0)
                return;

            RectTransform ammoRect = Instantiate(ammoPrefab, parent);

            ammoRect.anchorMin = rectTransform.anchorMin;
            ammoRect.anchorMax = rectTransform.anchorMax;
            ammoRect.anchoredPosition = rectTransform.anchoredPosition;

            ammoHeld = ammoRect.GetComponent<Ammo>();
            ammoHeld.SetType(type);

            if (_ammoCount.ContainsKey(type))
            {
                _ammoCount[type] = _ammoCount[type] - 1;

                countInfoText.SetText(_ammoCount[type].ToString());
            }
        }
    }
}
