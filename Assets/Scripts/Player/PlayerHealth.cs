using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [SerializeField] private float hitPoints = 100.0f;
    
    /* HIDDEN FIELDS: */
    // None.

    /* METHODS: */
    /// Method administers the damage taken by the player.
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0.0f)
            GetComponent<DeathHandler>().HandleDeath();
    }
}
