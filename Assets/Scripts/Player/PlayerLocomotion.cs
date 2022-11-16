using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerLocomotion : MonoBehaviour
{
    private Rigidbody _playerRb;
    private Animator _animator;
    private Vector2 _input;
    
    [Header("Ground Sampling")]
    [SerializeField] private float rayCastsMaxRange = 1.1f;
    [SerializeField] private LayerMask layerMask;
    
    /* Animator Param References: */
    private static readonly int InputX = Animator.StringToHash("input_X");
    private static readonly int InputY = Animator.StringToHash("input_Y");
    private static readonly int Falling = Animator.StringToHash("Falling");
    private static readonly int HasHitGround = Animator.StringToHash("hasHitGround");
    private static readonly int IsInAir = Animator.StringToHash("isInAir");

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
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

    // Method is sampling the layers beneath the player using raycast. The ray originates
    // from the center of the player and should reach a bit below the player. If the casted
    // ray hits the ground (layer mask) then landing animations will be executed. If the ray
    // hits other than the ground player will fall.
    private void SampleGround()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(_playerRb.worldCenterOfMass, 
                Vector3.down, out hitInfo, rayCastsMaxRange, layerMask))
            ProcessLanding();
        else 
            ProcessFalling();
    }

    // Method will execute landing animations (player has just been grounded).
    private void ProcessLanding()
    {
        if (_animator.GetBool(IsInAir))
        {
            _animator.SetTrigger(HasHitGround);
            _animator.ResetTrigger(Falling);
            _animator.SetBool(IsInAir, false);
        }

        _animator.ResetTrigger(HasHitGround);
    }

    // Method will execute falling animations (player is in air.).
    private void ProcessFalling()
    {
        _animator.SetTrigger(Falling);
        _animator.SetBool(IsInAir, true);
    }
}
