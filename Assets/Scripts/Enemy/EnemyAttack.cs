using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerHealth _target;
    [SerializeField] private float damage = 40.0f;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _target = FindObjectOfType<PlayerHealth>();
    }

    public void AttackHitEvent()
    {
        if (_target == null)
        {
            _target.TakeDamage(damage);
            Debug.Log("A zombie has hit you!");
        }
    }
}
