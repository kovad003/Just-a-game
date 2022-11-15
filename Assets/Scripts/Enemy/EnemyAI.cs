using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float chaseRadius = 10;
    private NavMeshAgent _navMeshAgent;
    private float _distanceToTarget = Mathf.Infinity;
    private bool _isProvoked = false;
    
    
    private void Start()
    {
        // Binding Components:
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
        EngageTarget();
    }

    private void EngageTarget()
    {
        if (_distanceToTarget >= _navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if (_distanceToTarget < _navMeshAgent.stoppingDistance)
        {
           AttackTarget(); 
        }
    }

    private void ChaseTarget()
    {
        // _isProvoked = true;
        _navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        Debug.Log("ATTACKING!" + target.name + "is being attacked.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
