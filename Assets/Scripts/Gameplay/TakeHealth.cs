using UnityEngine;

/// <summary>
/// Reduces health of the given entity.
/// </summary>
public class TakeHealth : MonoBehaviour
{
    [SerializeField]
    private float _healthToTake;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TakeHealthFromEntity(collision);
        AudioManager.Instance.PlaySound(AudioManager.Sound.Landmine_Explosion);
    }

    /// <summary>
    /// Take health from the entity.
    /// </summary>
    /// <param name="entity">The entity that collides with this object.</param>
    private void TakeHealthFromEntity(Collider2D entity)
    {
        if (entity == null) return;

        Entity tank = entity.GetComponent<Entity>();

        if (tank == null) return;

        else tank.TakeDamage(_healthToTake);

        Destroy(gameObject);
    }
}
