using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform pointOfInterest;
    [SerializeField] private Transform target;
    [SerializeField] private float chaseRadius = 5;
    [SerializeField] private float distanceToSafeZone = 10.0f;
    private NavMeshAgent _navMeshAgent;
    private float _distanceToTarget = Mathf.Infinity;
    private Animator _animator;
    private EnemyHealth _enemyHealth;
    
    /* Animator */
    private static readonly int CalmDown = Animator.StringToHash("CalmDown");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Attack = Animator.StringToHash("Attack");


    private void Start()
    {
        // Binding Components:
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyHealth = GetComponent<EnemyHealth>();
    }
    
    private void Update()
    {
        SampleTargetDistance();
        EngageTarget();
        DisableAI();
    }

    // Method is sampling the distance between the enemy and the player.
    // This distance affects the behaviour of the enemy. 
    private void SampleTargetDistance()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
    }

    // If the enemy character dies the "AI script" and "NavMesh" components must be disabled to prevent
    // further "activities".
    private void DisableAI()
    {
        // If enemy is alive AI cannot be disabled:
        if (!_enemyHealth.IsDead()) return;
        
        enabled = false;
        _navMeshAgent.enabled = false;
    }

    // This method acts a control relay. Enemy response is based on the distance between enemy and player.
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

    // When it is called, method calms the enemy down. 
    private void StayIdle()
    {
        _animator.SetTrigger(CalmDown);
    }

    // Enemy will move closer to the player if method is called.
    private void ChaseTarget()
    {
        _animator.SetTrigger(Move);
        _navMeshAgent.SetDestination(target.position);
    }

    // Enemy attacks the target when method is called.
    private void AttackTarget()
    {
        _animator.SetTrigger(Attack);
    }

    // For debugging only. Will draw a wire sphere representing the chase radius of the enemy.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
