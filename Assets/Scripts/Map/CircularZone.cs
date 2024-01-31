using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A round zone that is centered at the objective point.
/// </summary>
public class CircularZone : MonoBehaviour
{
    private Zone _currentZone;
    private Queue<Zone> zoneQueue;
    [SerializeField] private List<Zone> zoneList;

    private CircleCollider2D _circleCollider;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        zoneQueue = new Queue<Zone>(zoneList.OrderByDescending(zone => zone.Radius));
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateCurrentZone();
    }

    public void UpdateCurrentZone()
    {
        _currentZone.OnEnter?.Invoke();

        _currentZone = zoneQueue.Dequeue();
        Resize();
    }

    public void InitializeZone()
    {
        _currentZone = zoneQueue.Dequeue();
        Resize();
    }

    public List<Zone> GetZones()
    {
        return zoneList.OrderByDescending(zone => zone.Radius).ToList();
    }

    private void Resize()
    {
        if(zoneQueue.Count == 0)
        {
            _circleCollider.enabled = false;
            return;
        }

        _circleCollider.radius = _currentZone.Radius;
    }

    [System.Serializable]
    public struct Zone
    {
        public float Radius;
        public UnityEvent OnEnter;
    }
}
