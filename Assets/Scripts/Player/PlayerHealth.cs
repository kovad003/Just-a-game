using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100.0f;
    private Animator _animator;
    private static readonly int Die = Animator.StringToHash("Die");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        Debug.Log("Damage taken by player: " + damage);
        if (hitPoints <= 0.0f)
        {
            Debug.Log("You have died!");
            _animator.SetTrigger(Die);
        }
    }
}
