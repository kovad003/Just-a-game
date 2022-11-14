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

    private static readonly int IsPistolHolstered = Animator.StringToHash("isPistolHolstered");
    private static readonly int HolsterPistol = Animator.StringToHash("HolsterPistol");
    private static readonly int UnholsterPistol = Animator.StringToHash("UnholsterPistol");

    /**************************************************************************************************************/
    // Start is called before the first frame update
    void Start()
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
    }
    
    // FixedUpdate() must be used bc player has physics and rigidbody.
    void FixedUpdate()
    {
        float playerCamera = _mainCamera.transform.rotation.eulerAngles.y;
        // Camera will blend in (on the y-axis) from current rotation towards the camera's rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.Euler(0, playerCamera, 0), turnSpeed * Time.fixedDeltaTime );
        
        Debug.Log("_aimingRef.transform.position = " + _aimingRef.transform.position);
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        HolsterGun(KeyCode.H);
        MouseAim(Input.GetMouseButton(1));
        Shoot(Input.GetMouseButtonDown(0), Input.GetMouseButtonUp(0));
        
    }
    /**************************************************************************************************************/
    private void MouseAim(bool mouseInput)
    {
        if (mouseInput)
        {
            aimLayer.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            aimLayer.weight -= Time.deltaTime / aimDuration;
        }
    }
    
    private void HolsterGun(KeyCode key)
    {
        if (!Input.GetKeyUp(key)) return;
        if (_animator.GetBool(IsPistolHolstered) == false)
        {
            _animator.SetTrigger(HolsterPistol);
            StartCoroutine(DisableRigBuilder());
            rigLayers.SetActive(false);
            holsteredPistol.SetActive(true);
            _animator.SetBool(IsPistolHolstered, true);
        }
        else
        {
            _animator.SetTrigger(UnholsterPistol);
            StartCoroutine(EnableRigBuilder());
            holsteredPistol.SetActive(false);
            _animator.SetBool(IsPistolHolstered, false);
        }
    }

    private void Shoot(bool mouseBtnDown, bool mouseBtnUp)
    {
        if (mouseBtnDown)
        {
            Debug.Log("LMB pressed");
            _aimingRef.transform.position = IncrementAxisY(_aimingRef.transform.position, 8f * Time.fixedDeltaTime);

        }

        if (mouseBtnUp) // Probably better to make a coroutine here
        {
            Debug.Log("LMB released");
            _aimingRef.transform.position = DecrementAxisY(_aimingRef.transform.position, 8f * Time.fixedDeltaTime);
        }
    }

    // The delay makes the transition better between animation states!
    // Events weren't working really well bc of the rig layers!
    IEnumerator EnableRigBuilder()
    {
        yield return new WaitForSeconds(0.6f);
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = true;
        rigLayers.SetActive(true); // This needs to be here so weapon is only spawned when hands are together!
    }
    IEnumerator DisableRigBuilder()
    {
        yield return new WaitForSeconds(0.1f);
        // The named one will be set to true, the rest will be turned off:
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = i.name == "RigLayerBodyAim";
    }
    
    // Methods are used to generate weapon recoil when player is shooting.
    Vector3 IncrementAxisY(Vector3 vector, float y)
    {
        vector.y += y;
        return vector;
    }
    Vector3 DecrementAxisY(Vector3 vector, float y)
    {
        vector.y -= y;
        return vector;
    }
}
