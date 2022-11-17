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
    [SerializeField] private Rig aimLayer;
    [SerializeField] private GameObject rigLayers;
    [SerializeField] private GameObject holsteredPistol;

    private Camera _mainCamera;
    private Transform _aimingRef;
    private RigBuilder _rigBuilder;
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
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        // Binding Components:
        _rigBuilder = GetComponent<RigBuilder>();
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
        MouseAim(Input.GetMouseButton(1));
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
    private void MouseAim(bool mouseInput)
    {
        if (_animator.GetBool(IsPistolHolstered)) return;
        if (mouseInput)
        {
            // _isAiming = true;
            _animator.SetBool(IsAiming, true);
            aimLayer.weight += Time.deltaTime / aimDuration;
        }

        else
        {
            // _isAiming = false;
            _animator.SetBool(IsAiming, false);
            aimLayer.weight -= Time.deltaTime / aimDuration;
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
            StartCoroutine(DisableRigLayers());
            rigLayers.SetActive(false);
            holsteredPistol.SetActive(true);
            _animator.SetBool(IsPistolHolstered, true);
        }
        else
        {
            _animator.SetTrigger(UnholsterPistol);
            StartCoroutine(EnableRigLayers());
            holsteredPistol.SetActive(false);
            _animator.SetBool(IsPistolHolstered, false);
        }
    }

    // The delay makes the transition better between animation states!
    // Events weren't working really well bc of the rig layers!
    private IEnumerator EnableRigLayers()
    {
        yield return new WaitForSeconds(0.6f);
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = true;
        rigLayers.SetActive(true); // This needs to be here so weapon is only spawned when hands are together!
    }
    private IEnumerator DisableRigLayers()
    {
        yield return new WaitForSeconds(0.1f);
        // The named one will be set to true, the rest will be turned off:
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = i.name == "RigLayerBodyAim";
    }
}
