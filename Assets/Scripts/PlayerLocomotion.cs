using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Rigidbody _playerRb;
    private Animator _animator;
    private Vector2 _input;
    private float _inAirTimer = 0.0f;

    /* Animator Param References: */
    private static readonly int InputX = Animator.StringToHash("input_X");
    private static readonly int InputY = Animator.StringToHash("input_Y");
    private static readonly int Falling = Animator.StringToHash("Falling");
    private static readonly int HasHitGround = Animator.StringToHash("hasHitGround");
    private static readonly int IsInAir = Animator.StringToHash("isInAir");

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlingInput();
        SampleGround();
    }

    private void HandlingInput()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        _animator.SetFloat(InputX, _input.x);
        _animator.SetFloat(InputY, _input.y);
    }

    private void SampleGround()
    {
        RaycastHit hitInfo;
        float maxRange = 1.1f;
        if (Physics.Raycast(_playerRb.worldCenterOfMass, 
                Vector3.down, out hitInfo, maxRange, layerMask))
        {
            // Debug.Log("You are grounded!");
            if (_animator.GetBool(IsInAir))
            {
                _animator.SetTrigger(HasHitGround);
                _animator.ResetTrigger(Falling);
                _animator.SetBool(IsInAir, false);
            }
            _animator.ResetTrigger(HasHitGround);
            
        }
        else
        {
            Debug.Log("You are falling!");
            _animator.SetTrigger(Falling);
            _animator.SetBool(IsInAir, true);
        }
    }
}
