using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100.0f;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    private bool _isDead = false;
    
    /* Animator */
    private static readonly int DieZombie = Animator.StringToHash("Die");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void TakeDamage(float damageTaken)
    {
        hitPoints -= damageTaken;
        if (hitPoints <= 0.0f)
        {
            hitPoints = 0.0f;
            // Destroy(gameObject);
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;

        _animator.SetTrigger(DieZombie);
        _isDead = true;
        _navMeshAgent.enabled = false;
    }
}
