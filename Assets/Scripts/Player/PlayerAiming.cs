using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 15.0f;
    [SerializeField] private float aimDuration = 0.3f;
    [SerializeField] private GameObject holsteredPistol;

    private Camera _mainCamera;
    private Transform _aimingRef;
    private RigHandler _rigHandler;
    private Animator _animator;
    private float _timeOfLastShot;

    /* Animator Param References: */
    private static readonly int IsPistolHolstered = Animator.StringToHash("isPistolHolstered");
    private static readonly int HolsterPistol = Animator.StringToHash("HolsterPistol");
    private static readonly int UnholsterPistol = Animator.StringToHash("UnholsterPistol");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");

    /**************************************************************************************************************/
    // Start is called before the first frame update
    private void Start()
    {
        // Handling Camera:
        _mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Binding Components:
        _rigHandler = GetComponent<RigHandler>();
        _animator = GetComponent<Animator>();
    }
    
    // FixedUpdate() must be used bc player has physics and rigidbody.
    private void FixedUpdate()
    {
        HandleCamera(); // Must be here. CM cam is also using FU(). It is bc of player aiming.
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        HolsterGun(KeyCode.H);
        Aim(Input.GetMouseButton(1));
    }
    /**************************************************************************************************************/
    private void HandleCamera()
    {
        float playerCamera = _mainCamera.transform.rotation.eulerAngles.y;
        // Camera will blend in (on the y-axis) from current rotation towards the camera's rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.Euler(0, playerCamera, 0), turnSpeed * Time.fixedDeltaTime);
    }
    
    // Holding RMB triggers this method. Player can aim at any target after the
    // the player character assumed aiming position.
    private void Aim(bool mouseInput)
    {
        if (_animator.GetBool(IsPistolHolstered)) return;
        if (mouseInput)
        {
            // _isAiming = true;
            _animator.SetBool(IsAiming, true);
            _rigHandler.AdjustAimLayer(aimDuration, true);
        }

        else
        {
            // _isAiming = false;
            _animator.SetBool(IsAiming, false);
            _rigHandler.AdjustAimLayer(aimDuration, false);
        }
    }
    
    // The release of the "H" key triggers this method. The executed coroutines will affect
    // the Rig Builder component by enabling / disabling the attached Rig Layers. This way
    // the animator will have more control over the movement of the player character.
    private void HolsterGun(KeyCode key)
    {
        if (!Input.GetKeyUp(key)) return;
        if (_animator.GetBool(IsPistolHolstered) == false)
        {
            _animator.SetTrigger(HolsterPistol);
            _rigHandler.RelaxJoints();
            holsteredPistol.SetActive(true);
            _animator.SetBool(IsPistolHolstered, true);
        }
        else
        {
            _animator.SetTrigger(UnholsterPistol);
            _rigHandler.CloseJoints();
            holsteredPistol.SetActive(false);
            _animator.SetBool(IsPistolHolstered, false);
        }
    }
}
