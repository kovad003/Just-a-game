using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [SerializeField] private float hitPoints = 100.0f;
    private Animator _animator;
    private EnemyAI _enemyAI;
    private bool _isDead;
    
    // Animator Hash
    private static readonly int DieZombie = Animator.StringToHash("Die");

    /* METHODS: */
    private void Start()
    {
        // Binding Fields:
        _animator = GetComponent<Animator>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    /// Method informs other classes when enemy dies.
    public bool IsDead()
    {
        return _isDead;
    }

    /// PlayerWeapon.cs class calls this public method to decrease enemy's hit points.
    public void TakeDamage(float damageTaken)
    {
        _enemyAI.OnDamageTaken();
        
        hitPoints -= damageTaken;
        if (hitPoints <= 0.0f)
        {
            hitPoints = 0.0f;
            Die();
        }
    }

    /// Method executes the enemy's death sequence. Method cannot be called on a dead enemy.
    private void Die()
    {
        // Condition prevents "Die() loop" on dead zombies:
        if (_isDead) return;
        _animator.SetTrigger(DieZombie);
        _isDead = true;
        
        GetComponent<EnemyAudio>().PlayDeathSfx();
        
        // Silencing active sounds:
        try
        {
            GameObject child = transform.Find("AUDIO_Cancel on death").gameObject;
            child.GetComponent<AudioSource>().enabled = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
        }
    }
    
    public void OnCollapse(AnimationEvent animationEvent)
    {
        GetComponent<EnemyAudio>().PlayCollapseSfx();
    }
}
