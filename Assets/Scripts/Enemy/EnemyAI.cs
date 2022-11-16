using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform pointOfInterest;
    [SerializeField] private Transform target;
    [SerializeField] private float chaseRadius = 5;
    private NavMeshAgent _navMeshAgent;
    private float _distanceToTarget = Mathf.Infinity;
    [SerializeField] private float distanceToSafeZone = 10.0f;
    private Animator _animator;
    
    /* Animator */
    private static readonly int CalmDown = Animator.StringToHash("CalmDown");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Attack = Animator.StringToHash("Attack");


    private void Start()
    {
        // Binding Components:
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
        EngageTarget();
    }

    private void EngageTarget()
    {
        // Debug.Log("_distanceToTarget: " + _distanceToTarget);
        if (_distanceToTarget >= distanceToSafeZone)
            // LookAround();
            StayIdle();

        if (_distanceToTarget >= _navMeshAgent.stoppingDistance && _distanceToTarget < distanceToSafeZone)
            ChaseTarget();

        if (_distanceToTarget < _navMeshAgent.stoppingDistance)
            AttackTarget();
    }

    // private void LookAround()
    // {
    //     _navMeshAgent.SetDestination(pointOfInterest.position);
    // }

    private void StayIdle()
    {
        _animator.SetTrigger(CalmDown);
    }

    private void ChaseTarget()
    {
        _animator.SetTrigger(Move);
        _navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        _animator.SetTrigger(Attack);
        // Debug.Log("ATTACKING!" + target.name + "is being attacked.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
