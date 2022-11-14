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
    private bool _isAiming = false;

    /* Animator Param References: */
    private static readonly int IsPistolHolstered = Animator.StringToHash("isPistolHolstered");
    private static readonly int HolsterPistol = Animator.StringToHash("HolsterPistol");
    private static readonly int UnholsterPistol = Animator.StringToHash("UnholsterPistol");

    /**************************************************************************************************************/
    // Start is called before the first frame update
    private void Start()
    {
        // Handling Camera:
        _mainCamera = Camera.main;
        if (_mainCamera != null)
            _aimingRef = _mainCamera.transform.Find("REF_AimLookAt");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Binding Components:
        _rigBuilder = GetComponent<RigBuilder>();
        _animator = GetComponent<Animator>();
        
        // Binding Important Fields:
        _timeOfLastShot = Time.time;
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
        StartCoroutine(Shoot(Input.GetMouseButtonDown(0), 
            _animator.GetBool(IsPistolHolstered), _isAiming));
        
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
            _isAiming = true;
            aimLayer.weight += Time.deltaTime / aimDuration;
        }

        else
        {
            _isAiming = false;
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }
    }
    
    // LMB triggers this method. A timer is checking the elapsed time between shots.
    // Recoil is generated accordingly.
    private IEnumerator Shoot(bool mouseBtnDown, bool isGunHolstered, bool isAiming)
    {
        if (!mouseBtnDown) yield break;
        if (isGunHolstered) yield break;
        if (!isAiming) yield break;

        var timeOfCurrentShot = Time.time;
        if (!(_timeOfLastShot + 0.1f <= timeOfCurrentShot)) yield break;
        
        _timeOfLastShot = Time.time;
        var aimAt = _aimingRef.transform;
        aimAt.position = RecoilUpward(aimAt.position, 8f * Time.fixedDeltaTime);
        yield return new WaitForSeconds(0.1f);
        aimAt.position = RecoilDownward(aimAt.position, 8f * Time.fixedDeltaTime);
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
    
    // Methods are used to generate weapon recoil when player is shooting.
    // They can be kept static.
    private static Vector3 RecoilUpward(Vector3 vector, float y)
    {
        vector.y += y;
        return vector;
    }
    private static Vector3 RecoilDownward(Vector3 vector, float y)
    {
        vector.y -= y;
        return vector;
    }
}
