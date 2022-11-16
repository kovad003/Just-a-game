using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100.0f;
    private Animator _animator;
    private static readonly int Die = Animator.StringToHash("Die");

    private void Start()
    {
        // Binding Fields:
        _animator = GetComponent<Animator>();
    }

    // Method administers the damage taken by the player.
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0.0f)
            _animator.SetTrigger(Die);
    }
}
