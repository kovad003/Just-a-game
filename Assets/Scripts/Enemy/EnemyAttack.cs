using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerHealth _target;
    [SerializeField] private float damage = 40.0f;

    private void Start()
    {
        _target = FindObjectOfType<PlayerHealth>();
    }

    public void AttackHitEvent()
    {
        if (_target == null)
        {
            _target.TakeDamage(damage);
        }
    }
}
