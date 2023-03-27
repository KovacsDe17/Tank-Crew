using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHolder : MonoBehaviour
{
    [SerializeField]
    private RectTransform _ammoPrefab;

    [System.Serializable]
    internal class AmmoPlace
    {
        [SerializeField]
        internal RectTransform rectTransform;
        [SerializeField]
        internal Ammo.AmmoType type;

        internal Ammo ammoHeld;

        internal void GenerateAmmo(RectTransform ammoPrefab, Transform parent)
        {
            RectTransform ammoRect = Instantiate(ammoPrefab, parent);

            ammoRect.anchorMin = rectTransform.anchorMin;
            ammoRect.anchorMax = rectTransform.anchorMax;
            ammoRect.anchoredPosition = rectTransform.anchoredPosition;

            ammoHeld = ammoRect.GetComponent<Ammo>();
            ammoHeld.SetType(type);
        }
    }

    [SerializeField]
    private List<AmmoPlace> _places;

    public void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
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
}
