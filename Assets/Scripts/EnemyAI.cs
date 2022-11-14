using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float chaseRadius = 10;
    private NavMeshAgent _navMeshAgent;
    private float _distanceToTarget = Mathf.Infinity;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        // Binding Components:
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (chaseRadius <= _distanceToTarget)
            _navMeshAgent.SetDestination(target.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
