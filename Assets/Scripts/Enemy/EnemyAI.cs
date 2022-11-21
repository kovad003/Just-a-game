using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [SerializeField] private Transform pointOfInterest;
    [SerializeField] private Transform self;
    [SerializeField] private Transform target;
    [SerializeField] [Range(0.0f, 30.0f)] private float chaseRadius = 10.0f;
    [SerializeField] [Range(0.0f, 10.0f)] private float turnSpeed = 5.0f;
   
    /* HIDDEN FIELDS */
    private NavMeshAgent _navMeshAgent;
    private EnemyAudio _enemyAudio;
    private Animator _animator;
    private EnemyHealth _enemyHealth;
    private float _distanceToTarget = Mathf.Infinity;
    private bool _isProvoked;
    
    // Animator Hash
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    private void Start()
    {
        // Binding Components:
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyAudio = GetComponent<EnemyAudio>();
    }
    
    private void Update()
    {
        SampleTargetDistance();
        EngageTarget();
        DisableAI();
    }

    /// Method is sampling the distance between the enemy and the player.
    /// This distance affects the behaviour of the enemy. 
    private void SampleTargetDistance()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
    }

    /// If the enemy character dies the "AI script" and "NavMesh" components must be disabled to prevent
    /// further "activities".
    private void DisableAI()
    {
        // If enemy is alive AI cannot be disabled:
        if (!_enemyHealth.IsDead()) return;
        
        enabled = false;
        _navMeshAgent.enabled = false;
    }

    /// This method acts a control relay. Enemy response is based on the distance between enemy and player.
    private void EngageTarget()
    {
        // FaceTarget();
        if (_distanceToTarget >= chaseRadius)
        {
            StayIdle();
            _enemyAudio.PlayIdleSfx();
        }

        /*_distanceToTarget >= _navMeshAgent.stoppingDistance && */
        if (_distanceToTarget < chaseRadius || _isProvoked)
        {
            FaceTarget();
            ChaseTarget();
            _enemyAudio.PlayChaseSfx();
        }

        if (_distanceToTarget < _navMeshAgent.stoppingDistance)
        {
            FaceTarget();
            AttackTarget();
            _enemyAudio.PlayAttackSfx();
        }
    }

    /// When it is called, method calms the enemy down. 
    private void StayIdle()
    {
        _animator.ResetTrigger(Move);
        _animator.ResetTrigger(Attack);
        _animator.SetBool(IsIdle, true);
        _navMeshAgent.SetDestination(self.position);
        
    }

    /// Enemy will move closer to the player if method is called.
    private void ChaseTarget()
    {
        _animator.SetBool(IsIdle, false);
        _animator.SetTrigger(Move);
        // Other triggers must be reset:
        _animator.ResetTrigger(Attack);
        _navMeshAgent.SetDestination(target.position);
    }

    /// Enemy attacks the target when method is called.
    private void AttackTarget()
    {
        _animator.SetBool(IsIdle, false);
        // Other triggers must be reset:
        _animator.SetTrigger(Attack);
        _animator.ResetTrigger(Move);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime *turnSpeed);
    }
    
    /// Method administers the damage taken by the enemy character.
    public void OnDamageTaken()
    {
        StartCoroutine(GetProvoked());
    }
    
    /// Method Temporarily 
    private IEnumerator GetProvoked()
    {
        // Before Yield:
        _isProvoked = true;
        yield return new WaitForSeconds(1.0f);
        // Continue:
        _isProvoked = false;
    }
}
