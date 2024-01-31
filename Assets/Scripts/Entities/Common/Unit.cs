using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The representation of an entity.
/// </summary>
public class Unit : Entity
{
    internal Turret _turret;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    /// <summary>
    /// Setup health and health bar
    /// </summary>
    private void Initialize()
    {
        _turret = GetComponentInChildren<Turret>();
    }

    /// <summary>
    /// Change the Entity to its destroyed version, while maintaining the physics attributes
    /// </summary>
    public override void Die()
    {
        Debug.Log("Entity died!");

        if (IsServer)
        {
            GameObject destroyedEntity;

            if (DestroyedEntityIsFromScene())
            {
                destroyedEntity = _destroyedEntity;
                _destroyedEntity.transform.position = transform.position;
                _destroyedEntity.transform.rotation = transform.rotation;
            }
            else
            {
                destroyedEntity = Instantiate(_destroyedEntity, transform.position, transform.rotation);
                destroyedEntity.GetComponent<NetworkObject>().Spawn();
            }

            if (_turret != null)
            {
                Transform deadEntityTurret = destroyedEntity.GetComponentInChildren<Turret>().transform;
                deadEntityTurret.rotation = _turret.transform.rotation;
            }
        }

        base.Die();
    }

    public Turret GetTurret()
    {
        return _turret;
    }
}