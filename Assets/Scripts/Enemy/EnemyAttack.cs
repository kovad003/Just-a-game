using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [SerializeField] private float damage = 40.0f;
    
    /* HIDDEN FIELDS: */
    private PlayerHealth _target;
    
    private void Start()
    {
        // Binding Fields:
        _target = FindObjectOfType<PlayerHealth>();
    }

    // Event Function. Zombie Attack animation includes the AttackHitEvent. The event triggers this method directly.
    // No need for method call!
    public void AttackHitEvent()
    {
        if (_target == null) return;
        _target.TakeDamage(damage);
    }
}
